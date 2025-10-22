#region

using UnityEngine;

#endregion

namespace Behaviour.ObjectFeature.ContactModify
{
    /// <summary>
    /// 反作用を起こさないオブジェクトのためのKinetic Rigidbody用コンタクトモディファイア
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class OnlyNonRectiveCollision : ANonReactiveContactModifier
    {
        private int _bodyId;
        protected override int BodyId => _bodyId;

        private void Start()
        {
            var rb = GetComponent<Rigidbody>();
            _bodyId = rb.GetInstanceID();
        }

        protected override bool IsIgnore(int selfId, int otherId, bool isSelfNonReactive, bool isOtherNonReactive)
        {
            var isSelf = selfId == BodyId;
            var isOther = otherId == BodyId;

            // 自分が含まれているか
            var isContainSelf = isSelf || isOther;

            // 自分が含まれず、かつどちらかが非反作用オブジェクトの場合は無視する
            var isIgnore = isContainSelf && !(isSelfNonReactive || isOtherNonReactive);

            return isIgnore;
        }
    }
}