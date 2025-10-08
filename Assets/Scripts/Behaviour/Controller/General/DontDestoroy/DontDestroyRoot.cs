#region

using UnityEngine;

#endregion

namespace Behaviour.Controller.General.DontDestoroy
{
    /// <summary>
    ///     DontDestroyOnLoadのルートオブジェクトにアタッチするためのクラス
    /// </summary>
    public class DontDestroyRoot : MonoBehaviour
    {
        private static DontDestroyRoot _instance;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}