#region

using UnityEngine;
using UnityEngine.Audio;

#endregion

namespace Behaviour.Controller.General.DontDestoroy
{
    /// <summary>
    ///     AudioMixer を SerializeField で保持するコントローラーの共通ベース
    /// </summary>
    public abstract class AudioMixerControllerBase : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private AudioMixer audioMixer;

        #endregion

        #region Protected Properties

        protected AudioMixer Mixer => audioMixer;

        #endregion
    }
}
