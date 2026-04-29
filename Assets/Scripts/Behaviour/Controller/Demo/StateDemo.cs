using Behaviour.Controller.General;
using Lib.State.Scene;
using UnityEngine;

namespace Behaviour.Controller.Demo
{
    /// <summary>
    /// ステート遷移をテストするためのクラス
    /// </summary>
    public class StateDemo: MonoBehaviour
    {
        private SceneStateController _sceneStateController;
    
        private void Start()
        {
            _sceneStateController = SceneStateController.Instance;
            if (_sceneStateController == null)
                Debug.LogError("SceneStateController not found in StateDemo.");
        }
        
        private void Update()
        {
            // Pでポーズステートとインゲームステートを切り替える
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (_sceneStateController.Context.CurrentState.StateName == SceneState.Pause)
                {
                    _sceneStateController.ChangeSceneState(SceneState.InGame, true);
                }
                else
                {
                    _sceneStateController.ChangeSceneState(SceneState.Pause, true);
                }
            }
        }
    }
}