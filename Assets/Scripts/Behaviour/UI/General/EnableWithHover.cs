#region

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

#endregion

namespace Behaviour.UI.General
{
    /// <summary>
    ///     UIにホバーした際に対象を有効化するコンポーネント
    /// </summary>
    public class EnableWithHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // ホバーした際に有効化する対象のリスト
        [SerializeField]
        private List<GameObject> targets = new();

        private void Start()
        {
            // 初期状態で対象を無効化する
            foreach (var target in targets.Where(target => target != null)) target.SetActive(false);
        }

        /// <summary>
        ///     ホバーした際に有効化する対象を有効化する
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            foreach (var target in targets)
            {
                if (target != null)
                {
                    target.SetActive(true);
                }
            }
        }

        /// <summary>
        ///     ホバーが外れた際に対象を無効化する
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            foreach (var target in targets)
            {
                if (target != null)
                {
                    target.SetActive(false);
                }
            }
        }
    }
}