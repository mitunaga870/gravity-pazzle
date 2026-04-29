using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Behaviour.Camera;
using Behaviour.Controller;
using Behaviour.Controller.General;
using Behaviour.Gravity;
using Behaviour.ObjectFeature.RideableObjectBehaviours;
using Behaviour.Player;
using Behaviour.Player.Abstract;
using Behaviour.UI.General;
using Lib.DataClass;
using Lib.Logic;
using Lib.State.Scene;
using UnityEngine;

namespace Behaviour.UI.InGame
{
    public class IngameTutorialUIWrapper : MonoBehaviour
    {
        [SerializeField]
        private TutorialType _tutorialType;

        [Header("チュートリアル状況取得用")]

        [SerializeField]
        private PlayerBehaviour _playerBehaviour;

        [SerializeField]
        private PlayerCam _playerCam;

        [SerializeField]
        private RiderObject _riderObject;

        [SerializeField]
        private GlobalEventController _globalEventController;

        [Header("ハイライト対象")]
        [SerializeField]
        [Tooltip("オブジェクト重力変更チュートリアル対象")]
        private VGravBehaviour _vGravBehaviour;

        [SerializeField]
        [Tooltip("ターゲット重力方向変更チュートリアル対象")]
        private GameObject _directionUI;

        [SerializeField]
        [Tooltip("重力変更の制限UIチュートリアル対象")]
        private GameObject _gravChangeLimitUI;

        [Header("UI")]

        [SerializeField]
        private HighlightController _highlightController;

        [SerializeField]
        private GameObject _instructionWrapper;

        [SerializeField]
        private List<IngameTutorialUI> _tutorialUIList;



        #region private fields
        private TutorialState _currentState { get; set; } = TutorialState.None;

        // UIの最低表示時間(秒)
        private readonly float UI_DISPLAY_TIME_SEC = 3f;

        private SceneStateController _sceneStateController;

        private float _uiDisplayTimer = 0f;


        #endregion

        #region Unity methods

        private void Start()
        {
            Init();
        }

        private void Update()
        {
            ShowCurrentUI();
            CheckTutorial();
        }
        #endregion

        #region private methods

        private void Init()
        {
            _sceneStateController = SceneStateController.Instance;
            if (_sceneStateController == null) throw new NullReferenceException("SceneStateController not found");

            // ハイライトUIを非表示にする
            _highlightController.gameObject.SetActive(false);

            // チュートリアルタイプに応じて初期化
            switch (_tutorialType)
            {
                case TutorialType.First:
                    InitFirstTutorial();
                    break;
                case TutorialType.Second:
                    InitSecondTutorial();
                    break;
            }
        }

        private void InitFirstTutorial()
        {
            // チュートリアル前の動作を禁止する
            _playerBehaviour.Movable = false;
            _playerBehaviour.ChangeableObjGrav = false;
            _playerBehaviour.ChangeableTargetGravDirection = false;

            StartCamTutorial();
        }

        private void InitSecondTutorial()
        {

        }

        private void ShowCurrentUI()
        {
            foreach (var ui in _tutorialUIList)
            {
                ui.UI.SetActive(false);
            }
            
            if (_currentState == TutorialState.None) return;

            GetTutorialUI(_currentState).UI.SetActive(true);
        }

        private void CheckTutorial()
        {
            _uiDisplayTimer += Time.deltaTime;
            if (_uiDisplayTimer < UI_DISPLAY_TIME_SEC) return;

            switch (_currentState)
            {
                case TutorialState.Cam:
                    CheckCamTutorial();
                    break;
                case TutorialState.Move:
                    CheckMoveTutorial();
                    break;
                case TutorialState.Reset:
                    CheckResetTutorial();
                    break;
                case TutorialState.ObjGravChange:
                    CheckObjGravChangeTutorial();
                    break;
                case TutorialState.ResetObjGravChange:
                    CheckResetObjGravChangeTutorial();
                    break;
                case TutorialState.ResetWithGravObj:
                    CheckResetWithGravObjTutorial();
                    break;
                case TutorialState.TargetGravChange:
                    CheckTargetGravChangeTutorial();
                    break;
                case TutorialState.RideGravObj:
                    CheckRideGravObjTutorial();
                    break;
                case TutorialState.GravChangeLimit:
                    CheckGravChangeLimitTutorial();
                    break;
            }
        }

        private void EndTutorial()
        {
            _currentState = TutorialState.None;
            _uiDisplayTimer = 0f;
            _instructionWrapper.SetActive(false);
        }

        private IngameTutorialUI GetTutorialUI(TutorialState state)
        {
            var tutorialUI = _tutorialUIList.FirstOrDefault(ui => ui.State == state);

            if (tutorialUI == null) throw new NullReferenceException("TutorialUI not found " + state);

            return tutorialUI;
        }

        #endregion

        #region Cam Tutorial

        private void StartCamTutorial()
        {
            _currentState = TutorialState.Cam;
            _uiDisplayTimer = 0f;
        }

        private void CheckCamTutorial()
        {
            if (_playerCam.IsMoved && _playerCam.IsResetCalled)
            {
                EndCamTutorial();
            }
        }

        private void EndCamTutorial()
        {
            // 次は移動チュートリアル
            StartMoveTutorial();
        }

        #endregion

        #region Move Tutorial

        private void StartMoveTutorial()
        {
            _currentState = TutorialState.Move;
            _uiDisplayTimer = 0f;

            // プレイヤーを動けるようにする
            _playerBehaviour.Movable = true;
        }

        private void CheckMoveTutorial()
        {
            if (_playerBehaviour.IsFirstMoved)
            {
                EndMoveTutorial();
            }
        }

        private void EndMoveTutorial()
        {
            StartResetTutorial();
        }

        #endregion

        #region Reset Tutorial

        private void StartResetTutorial()
        {
            _currentState = TutorialState.Reset;
            _uiDisplayTimer = 0f;
        }

        private void CheckResetTutorial()
        {
            if (_globalEventController.IsResetCalled)
            {
                EndResetTutorial();
            }
        }

        private void EndResetTutorial()
        {
            StartObjGravChangeTutorial();
        }

        #endregion

        #region ObjGravChange Tutorial

        private void StartObjGravChangeTutorial()
        {
            _currentState = TutorialState.ObjGravChange;
            _uiDisplayTimer = 0f;

            // ハイライト設定
            _highlightController.gameObject.SetActive(true);
            _highlightController.SetHighlight(_vGravBehaviour.gameObject);

            // プレイヤーをオブジェクト重力変更可能にする
            _playerBehaviour.ChangeableObjGrav = true;
        }

        private void CheckObjGravChangeTutorial()
        {
            if (_vGravBehaviour.IsGravChanged)
            {
                EndObjGravChangeTutorial();
            }
        }
        private void EndObjGravChangeTutorial()
        {
            // 操作を受け付けてから最小1秒はチュートリアルを続ける
            var Coroutine = GeneralUtils.DelayCoroutine(1f, () =>
            {
                // ハイライト解除
                _highlightController.ClearHighlightIfCurrent();
                _highlightController.gameObject.SetActive(false);

                StartResetObjGravChangeTutorial();
            });
            StartCoroutine(Coroutine);
        }

        #endregion

        #region ResetObjGravChange Tutorial

        private void StartResetObjGravChangeTutorial()
        {
            _currentState = TutorialState.ResetObjGravChange;
            _uiDisplayTimer = 0f;

            // ハイライト設定
            _highlightController.gameObject.SetActive(true);
            _highlightController.SetHighlight(_vGravBehaviour.gameObject);
        }

        private void CheckResetObjGravChangeTutorial()
        {
            if (_vGravBehaviour.IsGravResetted)
            {
                EndResetObjGravChangeTutorial();
            }
        }
        private void EndResetObjGravChangeTutorial()
        {
            // 一定期間語に重力のリセットを追える
            var Coroutine = GeneralUtils.DelayCoroutine(1f, () =>
            {
                // ハイライト解除
                _highlightController.ClearHighlightIfCurrent();
                _highlightController.gameObject.SetActive(false);

                StartResetWithGravObjTutorial();

            });
            StartCoroutine(Coroutine);
        }

        #endregion

        #region Reset With GravObj Tutorial

        private void StartResetWithGravObjTutorial()
        {
            _currentState = TutorialState.ResetWithGravObj;
            _uiDisplayTimer = 0f;

            // リセットの表示フラグを元に戻す
            _globalEventController.IsResetCalled = false;
        }

        private void CheckResetWithGravObjTutorial()
        {
            if (_globalEventController.IsResetCalled)
            {
                EndResetWithGravObjTutorial();
            }
        }

        private void EndResetWithGravObjTutorial()
        {
            StartTargetGravChangeTutorial();
        }

        #endregion

        #region TargetGravChange Tutorial
        private void StartTargetGravChangeTutorial()
        {
            _currentState = TutorialState.TargetGravChange;
            _uiDisplayTimer = 0f;

            // プレイヤーをターゲット重力方向変更可能にする
            _playerBehaviour.ChangeableTargetGravDirection = true;
        }

        private void CheckTargetGravChangeTutorial()
        {
            if (_playerBehaviour.IsTargetGravChanged)
            {
                EndTargetGravChangeTutorial();
            }
        }
        private void EndTargetGravChangeTutorial()
        {
            _currentState = TutorialState.TargetGravChange_End;

            // ターゲットの表示対象をメイジ
            _highlightController.SetHighlight(_directionUI.gameObject);
            _highlightController.gameObject.SetActive(true);

            // 一定期間語にターゲット重力方向のチュートリアルを追える
            var Coroutine = GeneralUtils.DelayCoroutine(3f, () =>
            {
                // ハイライトを消す
                _highlightController.gameObject.SetActive(false);

                StartRideGravObjTutorial();
            });
            StartCoroutine(Coroutine);
        }

        #endregion

        #region RideGravObj Tutorial
        private void StartRideGravObjTutorial()
        {
            _currentState = TutorialState.RideGravObj;
            _uiDisplayTimer = 0f;
        }
        private void CheckRideGravObjTutorial()
        {
            if (_riderObject.IsRided)
            {
                EndRideGravObjTutorial();
            }
        }
        private void EndRideGravObjTutorial()
        {
            StartGravChangeLimitTutorial();
        }

        #endregion

        #region GravChangeLimit Tutorial
        private void StartGravChangeLimitTutorial()
        {
            _currentState = TutorialState.GravChangeLimit;

            // ハイライト設定
            _highlightController.gameObject.SetActive(true);
            _highlightController.SetHighlight(_gravChangeLimitUI.gameObject);

            // ゲーム時間をとめる
            _sceneStateController.ChangeSceneState(SceneState.Instruction);
        }

        private void CheckGravChangeLimitTutorial()
        {
            // ユーザー入力で終了
            if (Input.anyKeyDown)
            {
                EndGravChangeLimitTutorial();
            }
        }
        private void EndGravChangeLimitTutorial()
        {
            // ゲーム時間を再開
            _sceneStateController.ChangeSceneState(SceneState.InGame);

            EndTutorial();
        }
        
        #endregion
    }

    internal class HighLightObject
    {
    }

    public enum TutorialState
    {
        None,
        Cam,
        Move,
        Reset,
        ObjGravChange,
        ResetObjGravChange,
        ResetWithGravObj,
        TargetGravChange,
        TargetGravChange_End,
        RideGravObj,
        GravChangeLimit,
    }

    public enum TutorialType
    {
        First,
        Second,
    }
}