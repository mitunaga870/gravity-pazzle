#region

#nullable enable
using System;
using Behaviour.Controller.General;
using Behaviour.Controller.General.DontDestoroy;
using Behaviour.Controller.Stage;
using Behaviour.Gravity.Abstract;
using Behaviour.Player.Abstract;
using Lib.Logic;
using Lib.Logic.Gravity;
using Lib.State.Interface.Gravity;
using Lib.State.Scene;
using UnityEngine;

#endregion

namespace Behaviour.Camera
{
    /**
     * プレイヤーに追従するカメラ
     */
    public class PlayerCam : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        // ReSharper disable once InconsistentNaming
#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
        private InputController Input;
#pragma warning restore CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。

        #endregion
        #region Private Fields 
        private const float Threshold = 0.01f;
        private const float Sensitivity = 5f;
        private const float MaxPitch = 60f;
        private const float MinPitch = -85f;

        private Transform? _playerTrans;
        private GravType? _gravType;
        
        private Vector3 _prevPos = Vector3.zero;

        private float _pitch;

        private bool _isMovable = true;

        private bool _hasPlayer;
        private APlayerBehaviour? _playerBehaviour;
        private AGravBehaviour? _playerGravBehaviour;

        private Vector3 _initialOffset;
        private Vector3 _initialForward;
        private Quaternion _initialRotation;
        private GravType _initialGrav;

        #endregion

        #region Public Fields

        // チュートリアル用の状態フィールド
        // カメラが動かされたことがあるか
        public bool IsMoved { get; private set; }

        // チュートリアル用の状態フィールド
        // カメラがリセットされたことがあるか
        public bool IsResetCalled { get; private set; }
        
        // カメラを一時的に動かせなくする
        public void TemporarilyDisableMovement(float duration)
        {
            if (!_isMovable)
                return;

            _isMovable = false;

            // duration秒後に動かせるようにする
            var coroutine = GeneralUtils.DelayCoroutine(duration, () => _isMovable = true);
            StartCoroutine(coroutine);
        }

        [NonSerialized]
        // 次のカメラのプレイヤー追従を減らすための量
        public Vector3 OffsetNextFollow = Vector3.zero;

        #endregion

        #region Unity Methods
        private void Awake()
        {
            // マウスがはみ出さないようにする
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Start()
        {
            // シリアライズフィールド確認
            if (Input == null)
                Debug.LogError("InputController is not assigned in PlayerCam.");

            // プレイヤーのBehaviourを取得
            _playerBehaviour = StageDataController.Instance.PlayerBehaviour;
            _playerGravBehaviour = StageDataController.Instance.PlayerGravBehaviour;
            _hasPlayer = _playerBehaviour != null;

            if (_hasPlayer)
            {
                var playerTrans = _playerBehaviour!.transform;

                // 初期位置・回転を保存 localにすることでプレイヤーとの相対位置を保存
                _initialOffset = transform.position - playerTrans.position;
                _initialRotation = transform.localRotation;
                _initialForward = playerTrans.forward;
                _initialGrav = _playerGravBehaviour!.GravType;
            }
        }

        private void Update()
        {
            // カメラリセット処理
            ResetCamera();
            
            // プレイヤーの位置が設定されていない場合は何もしない
            if (_playerTrans == null || _gravType == null)
                return;
            
            // 動かせない場合は何もしない
            if (!_isMovable)
                return;
            
            // プレイヤーの位置と重力の種類を取得
            var playerPos = _playerTrans.position;
            var gravType = _gravType.Value;
            
            var mouseY = Mathf.Clamp(
                Input.GetAxis("Mouse Y", SceneState.InGame) * Sensitivity * -1,
                MaxPitch*-1,
                MaxPitch
                );

            // 閾値以上の動きがあった場合にカメラを回転させる
            if (Mathf.Abs(mouseY) > Threshold)
            {
                // 軸をカメラの回転角分だけ回転させる
                var rotatedAxis = transform.right;
                
                // ピッチを積算
                var unClampedPitch = _pitch + mouseY;
                // ピッチを制限
                _pitch = Mathf.Clamp(unClampedPitch, MinPitch, MaxPitch);
                // 超えた場合、その分を引く
                var excessPitch = unClampedPitch - _pitch;
                // 変化量計算
                var deltaPitch = mouseY - excessPitch;

                // カメラの回転を更新
                transform.RotateAround(
                    playerPos,
                    rotatedAxis,
                    deltaPitch
                );

                // チュートリアル用の状態を更新
                IsMoved = true;
            }
            
            // マウスの動きに合わせてカメラを回転させる
            var mouseX = Mathf.Clamp(
                Input.GetAxis("Mouse X", SceneState.InGame) * Sensitivity * -1,
                MaxPitch*-1,
                MaxPitch
                );

            if (Mathf.Abs(mouseX) > Threshold)
            {
                // カメラの回転を更新
                transform.RotateAround(
                    playerPos,
                    GravUtils.GetGravDirectionUnit(gravType),
                    mouseX
                );
            }
        }

        #endregion

        #region Public Methods
        /// <summary>
        ///     プレイヤーの位置と重力方向を指定する
        /// </summary>
        /// <param name="playerTrans"></param>
        /// <param name="gravType"></param>
        public void SetPlayerPosAndGrav(Transform playerTrans, GravType gravType)
        {
            if (_prevPos == Vector3.zero)
                // 初期位置設定
                _prevPos = playerTrans.position;
            
            // 変位量を計算
            var deltaPos = playerTrans.position - _prevPos;
            deltaPos -= OffsetNextFollow;

            // 次回の追従オフセットをリセット
            OffsetNextFollow = Vector3.zero;
            
            // カメラの位置を更新
            transform.position += deltaPos;
                
            _playerTrans = playerTrans;
            _gravType = gravType;
            _prevPos = _playerTrans.position;
        }

        /**
         * カメラの先のオブジェクトを取得する
         */
        public GameObject? GetCameraTarget()
        {
            var camTransform = this.transform;
            var originalPos = camTransform.position;
            var direction = camTransform.forward;


            // カメラの先にあるオブジェクトを取得
            return Physics.Raycast(originalPos, direction, out var hit, 100f, LayerMask.GetMask("Default"))
                ?
                // ヒットしたオブジェクトを返す
                hit.collider.gameObject :
                // ヒットしなかった場合はnullを返す
                null;
        }

        #endregion

        #region Private Methods

        /**
         * カメラを初期位置にリセットする
         */
        private void ResetCamera()
        {
            // リセットキーが押されたらリセットする
            var playerKey = SettingDataController.Instance.PlayerKey;
            if (!Input.GetKeyDown(playerKey.CameraResetKey, SceneState.InGame)) return;

            if (_playerBehaviour == null || _playerGravBehaviour == null) return;

            // プレイヤーの位置と重力方向を取得
            if (!_hasPlayer) return;
            var playerTrans = _playerBehaviour.transform;
            var gravType = _playerGravBehaviour.GravType;

            // カメラの位置と回転をリセット
            transform.position = playerTrans.position + _initialOffset;

            // 重力方向の変化量を出しておく
            var (gravRotateAxis, gravRotateAngle) =
                GravUtils.GetGravToGravRotation(_initialGrav, _playerGravBehaviour.GravType);
            // axisがゼロベクトルの場合は１８０度回転
            if (gravRotateAxis == Vector3.zero)
            {
                gravRotateAxis = GravUtils.GetGravPerpendicularUnit(gravType); // 適当な軸
                gravRotateAngle = 180f;
            }

            // 重力方向に合わせて回転　このときにローカル回転もおかしくなるので、この後にリセットする
            if (gravType != _initialGrav)
                transform.RotateAround(
                    playerTrans.position,
                    gravRotateAxis,
                    gravRotateAngle
                );
            Debug.Log(
                $"Reset Camera Grav Rotate: From {_initialGrav} to {gravType}, Axis: {gravRotateAxis}, Angle: {gravRotateAngle}");
            transform.localRotation = _initialRotation;

            // プレイヤーの初期向きも回転
            var gravedInitialForward = Quaternion.AngleAxis(gravRotateAngle, gravRotateAxis) * _initialForward;
            // プレイヤーの正面向きになるように
            var curForward = playerTrans.forward;
            var quatToInitial = Quaternion.FromToRotation(curForward, gravedInitialForward);
            var angle = GravUtils.GetGravAxisEulerAngle(gravType, quatToInitial.eulerAngles);
            transform.RotateAround(
                playerTrans.position,
                GravUtils.GetGravDirectionUnit(gravType),
                angle
            );

            // ピッチをリセット
            _pitch = 0f;
            
            // リセットキーが押されたことを記録
            IsResetCalled = true;
        }

        #endregion
    }
}
