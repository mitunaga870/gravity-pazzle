using UnityEngine;

namespace Behaviour.Gimmick.CheckPoints
{
    /// <summary>
    /// チェックポイントのアニメーションのみを管理するクラス
    /// </summary>
    [RequireComponent(typeof(CheckPoint))]
    public class CheckPointAnim : MonoBehaviour
    {
        [SerializeField]
        private Renderer colorBoxRenderer;

        private static readonly Color ActiveColor = Color.green;
        private static readonly Color InactiveColor = Color.red;
        
        private void Start()
        {
            SetInactive();
        }
        
        public void SetActive()
        {
            if (colorBoxRenderer == null) return;
                
            colorBoxRenderer.material.color = ActiveColor;
        }

        public void SetInactive()
        {
            if (colorBoxRenderer == null) return;

            colorBoxRenderer.material.color = InactiveColor;
        }
    }
}