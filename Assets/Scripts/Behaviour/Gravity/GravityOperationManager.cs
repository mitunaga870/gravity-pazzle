#region

using System;
using System.Collections.Generic;
using Lib.State.Interface.Gravity;
using UnityEngine;

#endregion

namespace Behaviour.Gravity
{
    /// <summary>
    /// 重力方向の操作数と操作時間を制御・管理するマネージャー。
    /// プレイヤーによる可変重力操作に上限と持続時間を設ける。
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class GravityOperationManager : MonoBehaviour
    {
        [SerializeField]
        [Min(0.1f)]
        private float operationDuration = 5f; // 重力操作に与えられた持続時間（秒）

        [SerializeField]
        [Min(1)]
        private int maxConcurrentOperations = 2; // 同時に許容される操作数（プレイヤー自身を含む）

        private static readonly OperationHandle HandleNone = new(OperationHandleKind.None);
        private static readonly OperationHandle HandleManualRevert = new(OperationHandleKind.ManualRevert);
        private static readonly OperationHandle HandleExtended = new(OperationHandleKind.Extended);
        private static readonly OperationHandle HandleNew = new(OperationHandleKind.New);

        private readonly Dictionary<VGravBehaviour, Operation> _operations = new();
        private readonly List<VGravBehaviour> _revertBuffer = new(); // Update ループ内で遅延削除するためのバッファ

        private bool _isReverting; // 自動復帰中フラグ（SetGravAffected の再帰的な登録を防ぐため）
        private float _sharedRemainingTime; // 制限時間

        public static GravityOperationManager Instance { get; private set; }

        /// <summary>
        /// 同時操作可能数
        /// </summary>
        public int MaxConcurrentOperations => maxConcurrentOperations;

        /// <summary>
        /// 操作継続時間（秒）
        /// </summary>
        public float OperationDuration => operationDuration;

        /// <summary>
        /// 現在進行中の操作数
        /// </summary>
        public int ActiveOperationCount => _operations.Count;

        // (現在アクティブな操作数, 同時操作数上限) を通知するイベント。UI のメーターなどで利用可能。
        public event Action<int, int> OnOperationCountChanged;

        // 各操作対象に対して共有タイマーの残り割合を通知するイベント。
        public event Action<VGravBehaviour, float> OnOperationRemainingRatioChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning($"{nameof(GravityOperationManager)}: 重複したインスタンスが生成されたため破棄します。（{gameObject.name}）");
                Destroy(this);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Update()
        {
            if (_operations.Count == 0)
                return;

            var deltaTime = Time.deltaTime;

            // 破棄された対象を事前に取り除く
            _revertBuffer.Clear();
            foreach (var kvp in _operations)
            {
                if (kvp.Value.Target == null)
                {
                    _revertBuffer.Add(kvp.Key);
                }
            }

            if (_revertBuffer.Count > 0)
            {
                foreach (var behaviour in _revertBuffer)
                {
                    if (_operations.Remove(behaviour))
                    {
                        RaiseOperationCountChanged();
                    }
                }

                _revertBuffer.Clear();

                if (_operations.Count == 0)
                {
                    _sharedRemainingTime = 0f;
                    return;
                }
            }

            if (_sharedRemainingTime > 0f)
            {
                // 残り時間を減算（複数操作で共有）
                _sharedRemainingTime -= deltaTime;
                if (_sharedRemainingTime < 0f)
                    _sharedRemainingTime = 0f;
            }

            // 共通タイマーの残り割合を全対象へ通知
            var ratio = operationDuration > 0f
                ? Mathf.Clamp01(_sharedRemainingTime / operationDuration)
                : 0f;

            foreach (var kvp in _operations)
            {
                OnOperationRemainingRatioChanged?.Invoke(kvp.Key, ratio);
            }

            // 時間が残っている間は何もしない
            if (_sharedRemainingTime > 0f)
                return;

            // 制限時間が尽きたら全操作を元に戻す
            _revertBuffer.Clear();
            foreach (var behaviour in _operations.Keys)
            {
                _revertBuffer.Add(behaviour);
            }

            foreach (var behaviour in _revertBuffer)
            {
                if (!_operations.TryGetValue(behaviour, out var operation))
                    continue;

                _operations.Remove(behaviour);
                RaiseOperationCountChanged();

                if (behaviour == null)
                    continue;

                _isReverting = true;
                behaviour.SetGravAffected(operation.OriginalGravType, forceChange: true, registerOperation: false);
                _isReverting = false;
            }

            _revertBuffer.Clear();
            _sharedRemainingTime = 0f;
        }

        /// <summary>
        /// 操作開始前にマネージャーへ問い合わせを行い、カウント開始の準備をする。
        /// </summary>
        public bool TryPrepareOperation(
            VGravBehaviour behaviour,
            GravType requestedType,
            GravType currentType,
            out OperationHandle handle
        )
        {
            handle = HandleNone;

            if (behaviour == null)
                return false;

            if (_isReverting)
                return true;

            if (_operations.TryGetValue(behaviour, out var existing))
            {
                if (requestedType == existing.OriginalGravType)
                {
                    handle = HandleManualRevert;
                    return true;
                }

                // 同じ対象の別方向操作。タイマーは共有なのでリセットせず、要求先だけ更新する
                existing.RequestedGravType = requestedType;
                handle = HandleExtended;
                return true;
            }

            if (_operations.Count >= maxConcurrentOperations)
                return false;

            var wasEmpty = _operations.Count == 0;

            var operation = new Operation(behaviour, currentType, requestedType);
            _operations.Add(behaviour, operation);
            handle = HandleNew;
            RaiseOperationCountChanged();

            if (wasEmpty)
            {
                _sharedRemainingTime = operationDuration; // 最初の操作を開始するときにのみ共有タイマーを初期化（重力操作中のオブジェクトに更に重力操作しても初期化しない）
            }

            var ratio = operationDuration > 0f
                ? Mathf.Clamp01(_sharedRemainingTime / operationDuration)
                : 0f;
            OnOperationRemainingRatioChanged?.Invoke(behaviour, ratio);
            return true;
        }

        /// <summary>
        /// 操作が失敗した際に登録を取り消す。
        /// </summary>
        public void RollbackOperation(VGravBehaviour behaviour, in OperationHandle handle)
        {
            if (!handle.IsNewRegistration)
                return;

            if (_operations.Remove(behaviour))
            {
                RaiseOperationCountChanged();
                if (_operations.Count == 0)
                    _sharedRemainingTime = 0f; // 操作が全て完了したのでタイマーも停止
            }
        }

        /// <summary>
        /// 操作に成功した際の通知。手動リセットなどを受け取ってカウント調整する。
        /// </summary>
        public void NotifySuccessfulChange(VGravBehaviour behaviour, in OperationHandle handle)
        {
            if (handle.IsManualRevert)
            {
                if (_operations.Remove(behaviour))
                {
                    RaiseOperationCountChanged();
                    if (_operations.Count == 0)
                        _sharedRemainingTime = 0f; // 最後の操作が戻ったのでカウントをリセット
                }
            }
        }

        /// <summary>
        ///     指定されたVGravBehaviourに対する操作情報を取得する。
        ///     見つかった場合は true を返し、元の重力タイプと要求された重力タイプを out パラメーターで返す。
        /// </summary>
        public bool GetOperationInfo(
            VGravBehaviour behaviour,
            out GravType originalType,
            out GravType requestedType
        )
        {
            // 指定された挙動に対する操作情報を取得する
            if (_operations.TryGetValue(behaviour, out var operation))
            {
                originalType = operation.OriginalGravType;
                requestedType = operation.RequestedGravType;
                return true;
            }

            // 見つからなかった場合
            originalType = default;
            requestedType = default;
            return false;
        }

        /// <summary>
        /// 外部要因で操作が終了した場合の明示的な削除
        /// </summary>
        public void ForceRemoveOperation(VGravBehaviour behaviour)
        {
            if (_operations.Remove(behaviour))
            {
                RaiseOperationCountChanged();
                if (_operations.Count == 0)
                    _sharedRemainingTime = 0f; // 他操作も無くなったため共有タイマーを停止
            }
        }

        // 自動復帰中かどうか。プレイヤー操作側で循環登録を避けるために公開。
        internal bool IsReverting => _isReverting;

        private void RaiseOperationCountChanged()
        {
            OnOperationCountChanged?.Invoke(_operations.Count, maxConcurrentOperations);
        }

        private sealed class Operation
        {
            public Operation(
                VGravBehaviour target,
                GravType original,
                GravType requested
            )
            {
                Target = target; // 操作対象
                OriginalGravType = original; // 操作前の重力（タイムアウト時に戻す）
                RequestedGravType = requested; // 現在要求されている重力（共有タイマーで監視）
            }

            public VGravBehaviour Target { get; }
            public GravType OriginalGravType { get; }
            public GravType RequestedGravType { get; set; }
        }

        public readonly struct OperationHandle
        {
            internal OperationHandle(OperationHandleKind kind)
            {
                Kind = kind;
            }

            private OperationHandleKind Kind { get; }

            public bool IsNewRegistration => Kind == OperationHandleKind.New;
            public bool IsManualRevert => Kind == OperationHandleKind.ManualRevert;

            public static OperationHandle None => HandleNone;
        }

        internal enum OperationHandleKind
        {
            None,
            New,
            Extended,
            ManualRevert
        }
    }
}