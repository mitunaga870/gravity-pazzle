#region

#endregion

#region

using UnityEngine;

#endregion

namespace Behaviour.ObjectFeature.ContactModify
{
    /// <summary>
    /// 反作用を起こさないオブジェクトを無視するコンタクトモディファイア
    /// </summary>
    public class IgnoreNonRiactiveCollision : ANonReactiveContactModifier
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
            return (isOtherNonReactive && selfId == BodyId) ||
                   (isSelfNonReactive && otherId == BodyId);
        }
    }
}