using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Behaviour.Camera;
using Behaviour.Controller;
using Behaviour.Controller.General;
using Behaviour.Gimmick;
using Behaviour.Gimmick.CheckPoints;
using Behaviour.Gravity;
using Behaviour.ObjectFeature.RideableObjectBehaviours;
using Behaviour.Player;
using Behaviour.Player.Abstract;
using Behaviour.Trigger;
using Behaviour.UI.General;
using Behaviour.UI.InGame.PauseMenu;
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

        [Header("共通チュートリアル状況取得用")]

        [SerializeField]
        private PlayerBehaviour _playerBehaviour;

        [Header("第一チュートリアル状況取得用")]

        [SerializeField]
        private PlayerCam _playerCam;

        [SerializeField]
        private RiderObject _riderObject;

        [SerializeField]
        private GlobalEventController _globalEventController;

        [Header("共通チュートリアル対象")]
        [SerializeField]
        private CheckPoint _checkPoint;

        [Header("第一チュートリアル用ハイライト対象")]
        [SerializeField]
        [Tooltip("オブジェクト重力変更チュートリアル対象")]
        private VGravBehaviour _vGravBehaviour;

        [SerializeField]
        [Tooltip("ターゲット重力方向変更チュートリアル対象")]
        private GameObject _directionUI;

        [SerializeField]
        [Tooltip("重力変更の制限UIチュートリアル対象")]
        private GameObject _gravChangeLimitUI;

        [SerializeField]
        [Tooltip("ポーズチュートリアル対象")]
        private PauseMenuController _pauseMenuController;

        [SerializeField]
        [Tooltip("ゴールチュートリアル対象")]
        private GoalTrigger _goalTrigger;

        [SerializeField]
        [Tooltip("コインチュートリアル対象")]
        private CoinTrigger _coinTrigger;

        [Header("第二チュートリアル状況取得用")]
        [SerializeField]
        private StaticGravArea _staticGravArea;


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
            StartPlayerGravChangeTutorial();
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
                case TutorialState.CheckPoint:
                    CheckCheckPointTutorial();
                    break;
                case TutorialState.Pause:
                    CheckPauseTutorial();
                    break;
                case TutorialState.Coin:
                    CheckCoinTutorial();
                    break;
                case TutorialState.Goal:
                    CheckGoalTutorial();
                    break;
                case TutorialState.PlayerGravChange:
                    CheckPlayerGravChangeTutorial();
                    break;
                case TutorialState.PlayerGravChangeLimit:
                    CheckPlayerGravChangeLimitTutorial();
                    break;
                case TutorialState.ResetPlayerGravChange:
                    CheckResetPlayerGravChangeTutorial();
                    break;
                case TutorialState.GuidePlayerGravCheckPoint:
                    CheckGuidePlayerGravCheckPointTutorial();
                    break;
                case TutorialState.StaticGravArea:
                    CheckStaticGravAreaTutorial();
                    break;
                case TutorialState.StaticGravAreaLimit:
                    CheckStaticGravAreaLimitTutorial();
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

                StartTargetGravChangeTutorial();
            });
            StartCoroutine(Coroutine);
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
            _sceneStateController.ChangeSceneState(SceneState.Pause);
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

            // ハイライト解除
            _highlightController.gameObject.SetActive(false);

            StartResetWithGravObjTutorial();
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
            StartCheckPointTutorial();
        }

        #endregion

        #region Check Point Tutorial
        private void StartCheckPointTutorial()
        {
            _currentState = TutorialState.CheckPoint;
            _uiDisplayTimer = 0f;

            // チェックポイントを表示
            _highlightController.gameObject.SetActive(true);
            _highlightController.SetHighlight(_checkPoint.gameObject, 0.25f);
        }

        private void CheckCheckPointTutorial()
        {
            if (_checkPoint.IsActive)
            {
                EndCheckPointTutorial();
            }
        }
        private void EndCheckPointTutorial()
        {
            // ハイライト解除
            _highlightController.gameObject.SetActive(false);

            StartPauseTutorial();
        }


        #endregion

        #region Pause Tutorial
        private void StartPauseTutorial()
        {
            _currentState = TutorialState.Pause;
            _uiDisplayTimer = 0f;
        }

        private void CheckPauseTutorial()
        {
            if (_pauseMenuController.IsMenuOpened)
                EndPauseTutorial();
        }
        private void EndPauseTutorial()
        {
            StartCoinTutorial();
        }
        #endregion

        #region Coin Tutorial
        private void StartCoinTutorial()
        {
            _currentState = TutorialState.Coin;
            _uiDisplayTimer = 0f;

            // ハイライト設定
            _highlightController.gameObject.SetActive(true);
            _highlightController.SetHighlight(_coinTrigger.gameObject);
        }
        private void CheckCoinTutorial()
        {
            if (_coinTrigger.IsCollected)
                EndCoinTutorial();
        }
        private void EndCoinTutorial()
        {
            // ハイライト解除
            _highlightController.gameObject.SetActive(false);

            StartGoalTutorial();
        }
        #endregion

        #region Goal Tutorial
        private void StartGoalTutorial()
        {
            _currentState = TutorialState.Goal;
            _uiDisplayTimer = 0f;
        }

        private void CheckGoalTutorial()
        {
            if (Input.anyKeyDown)
                EndGoalTutorial();
        }
        private void EndGoalTutorial()
        {
            EndTutorial();
        }

        #endregion

        #region PlayerGravChange Tutorial
        private void StartPlayerGravChangeTutorial()
        {
            _currentState = TutorialState.PlayerGravChange;
            _uiDisplayTimer = 0f;
        }
        private void CheckPlayerGravChangeTutorial()
        {
            if (_playerBehaviour.IsPlayerGravChanged)
                EndPlayerGravChangeTutorial();
        }
        private void EndPlayerGravChangeTutorial()
        {
            StartPlayerGravChangeLimitTutorial();
        }
        #endregion

        #region PlayerGravChangeLimit Tutorial

        private void StartPlayerGravChangeLimitTutorial()
        {
            _currentState = TutorialState.PlayerGravChangeLimit;
            _uiDisplayTimer = 0f;

            // ハイライト設定
            _highlightController.gameObject.SetActive(true);
            _highlightController.SetHighlight(_gravChangeLimitUI.gameObject);

            // 時間を止める
            _sceneStateController.ChangeSceneState(SceneState.Pause);
        }
        private void CheckPlayerGravChangeLimitTutorial()
        {
            if (Input.anyKeyDown)
                EndPlayerGravChangeLimitTutorial();
        }
        private void EndPlayerGravChangeLimitTutorial()
        {
            // 時間を再開
            _sceneStateController.ChangeSceneState(SceneState.InGame);

            // ハイライト解除
            _highlightController.gameObject.SetActive(false);

            StartResetPlayerGravChangeTutorial();
        }
        #endregion

        #region Reset PlayerGravChange Tutorial
        private void StartResetPlayerGravChangeTutorial()
        {
            _currentState = TutorialState.ResetPlayerGravChange;
            _uiDisplayTimer = 0f;
        }
        private void CheckResetPlayerGravChangeTutorial()
        {
            if (_playerBehaviour.IsPlayerGravResetted)
                EndResetPlayerGravChangeTutorial();
        }
        private void EndResetPlayerGravChangeTutorial()
        {
            StartGuidePlayerGravCheckPointTutorial();
        }
        #endregion

        #region Gide Player Grav CheckPoint Tutorial

        private void StartGuidePlayerGravCheckPointTutorial()
        {
            _currentState = TutorialState.GuidePlayerGravCheckPoint;
            _uiDisplayTimer = 0f;
        }
        private void CheckGuidePlayerGravCheckPointTutorial()
        {
            if (_checkPoint.IsActive)
                EndGuidePlayerGravCheckPointTutorial();
        }
        private void EndGuidePlayerGravCheckPointTutorial()
        {
            StartStaticGravAreaTutorial();
        }
        #endregion

        #region StaticGravArea Tutorial
        private void StartStaticGravAreaTutorial()
        {
            _currentState = TutorialState.StaticGravArea;
            _uiDisplayTimer = 0f;
            
            // ハイライト設定
            _highlightController.gameObject.SetActive(true);
            _highlightController.SetHighlight(_staticGravArea.gameObject);
        }
        private void CheckStaticGravAreaTutorial()
        {
            if (_staticGravArea.WasPlayerEntered)
                EndStaticGravAreaTutorial();
        }
        private void EndStaticGravAreaTutorial()
        {
            // ハイライト解除
            _highlightController.gameObject.SetActive(false);

            StartStaticGravAreaLimitTutorial();
        }
        #endregion

        #region StaticGravArea Limit Tutorial
        private void StartStaticGravAreaLimitTutorial()
        {
            _currentState = TutorialState.StaticGravAreaLimit;
            _uiDisplayTimer = 0f;
            
            // ハイライト設定
            _highlightController.gameObject.SetActive(true);
            _highlightController.SetHighlight(_gravChangeLimitUI.gameObject);
        }
        private void CheckStaticGravAreaLimitTutorial()
        {
            EndStaticGravAreaLimitTutorial();
        }
        private void EndStaticGravAreaLimitTutorial()
        {
            // ハイライト解除
            _highlightController.gameObject.SetActive(false);

            EndTutorial();
        }
        #endregion
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
        CheckPoint,
        Pause,
        Coin,
        Goal,
        PlayerGravChange,
        PlayerGravChangeLimit,
        ResetPlayerGravChange,
        GuidePlayerGravCheckPoint,
        StaticGravArea,
        StaticGravAreaLimit,
    }

    public enum TutorialType
    {
        First,
        Second,
    }
}