#region

using Behaviour.Camera;
using Behaviour.Controller;
using Behaviour.Gravity;
using Behaviour.Player;
using UnityEngine;

#endregion

namespace Behaviour.UI
{
    public class TutorialUIWrapper : MonoBehaviour
    {
        #region SerializeField

        [Header("チュートリアル通過フラグ用の対象")]
        [SerializeField]
        private DemoPlayerBehaviour demoPlayerBehaviour;

        [SerializeField]
        private PlayerCam playerCam;

        [SerializeField]
        private VGravBehaviour vGravBehaviour;

        [SerializeField]
        private GlobalKeyController globalKeyController;

        [Header("UIの表示対象")]
        [SerializeField]
        private GameObject moveAndCamUI;

        [SerializeField]
        private GameObject gravChangeUI;

        [SerializeField]
        private GameObject targetGravChangeUI;

        [SerializeField]
        private GameObject resetUI;

        [Header("その他")]
        [SerializeField]
        private float uiDisplayTime = 3f;

        #endregion

        private float _uiDisplayTimer;

        private TutorialState _currentState;

        #region Unity Methods

        /**
         * シリアライズフィールドのチェックを行い、UIを非表示にする
         */
        private void Start()
        {
            // シリアライズフィールドのチェック
            if (
                demoPlayerBehaviour == null ||
                playerCam == null ||
                vGravBehaviour == null ||
                globalKeyController == null ||
                moveAndCamUI == null ||
                gravChangeUI == null ||
                targetGravChangeUI == null ||
                resetUI == null
            )
            {
                Debug.LogError("One or more serialized fields are not assigned.");
                return;
            }

            // UIを非表示にする
            gravChangeUI.SetActive(false);
            targetGravChangeUI.SetActive(false);
            resetUI.SetActive(false);
            // 移動とカメラのUIを表示
            moveAndCamUI.SetActive(true);

            // 初期状態を設定
            _currentState = TutorialState.MoveAndCam;
        }

        /**
         * チュートリアルの状態遷移
         */
        private void Update()
        {
            // ステータスに応じてUIを表示
            switch (_currentState)
            {
                case TutorialState.MoveAndCam:
                    moveAndCamUI.SetActive(true);
                    gravChangeUI.SetActive(false);
                    targetGravChangeUI.SetActive(false);
                    resetUI.SetActive(false);
                    break;
                case TutorialState.GravChange:
                    moveAndCamUI.SetActive(false);
                    gravChangeUI.SetActive(true);
                    targetGravChangeUI.SetActive(false);
                    resetUI.SetActive(false);
                    break;
                case TutorialState.TargetGravChange:
                    moveAndCamUI.SetActive(false);
                    gravChangeUI.SetActive(false);
                    targetGravChangeUI.SetActive(true);
                    resetUI.SetActive(false);
                    break;
                case TutorialState.Reset:
                    moveAndCamUI.SetActive(false);
                    gravChangeUI.SetActive(false);
                    targetGravChangeUI.SetActive(false);
                    resetUI.SetActive(true);
                    break;
                default:
                    moveAndCamUI.SetActive(false);
                    gravChangeUI.SetActive(false);
                    targetGravChangeUI.SetActive(false);
                    resetUI.SetActive(false);
                    break;
            }

            // チュートリアル状態に応じて条件を満たしている時間を計測
            if (
                (_currentState == TutorialState.MoveAndCam &&
                 demoPlayerBehaviour.IsMoved && playerCam.IsMoved) ||
                (_currentState == TutorialState.GravChange &&
                 vGravBehaviour.IsGravChanged) ||
                (_currentState == TutorialState.TargetGravChange &&
                 demoPlayerBehaviour.IsTargetGravChanged) ||
                (_currentState == TutorialState.Reset &&
                 globalKeyController.IsResetCalled)
            )
                _uiDisplayTimer += Time.deltaTime;

            // 指定時間経過したら次の状態へ遷移
            if (!(_uiDisplayTimer > uiDisplayTime)) return;
            _uiDisplayTimer = 0f; // タイマーをリセット
            _currentState++;
        }

        #endregion
    }

    public enum TutorialState
    {
        MoveAndCam = 0,
        TargetGravChange = 1,
        GravChange = 2,
        Reset = 3
    }
}