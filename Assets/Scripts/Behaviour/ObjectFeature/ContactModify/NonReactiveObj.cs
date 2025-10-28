#region

using Behaviour.Controller.Stage;
using UnityEngine;

#endregion

namespace Behaviour.ObjectFeature.ContactModify
{
    /// <summary>
    ///     反作用を起こさないオブジェクトのマーカー用クラス
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class NonReactiveObj : ANonReactiveContactModifier
    {
        private int _bodyId;
        protected override int BodyId => _bodyId;

        // えなーぶるのタイミングだとバグるのでえなーぶるの処理を消す
        protected override void OnEnable()
        {
        }

        private void Start()
        {
            var rb = GetComponent<Rigidbody>();
            _bodyId = rb.GetInstanceID();

            StageDataController.Instance.AddPlayerBodyInstanceID(BodyId);

            var collider = GetComponent<Collider>();
            collider.hasModifiableContacts = true;

            // 一つ目の非反作用オブジェクトの場合、反作用を起こさないオブジェクトの同士の衝突を無視するためにContactModifyEventを登録
            if (StageDataController.Instance.NonReactiveBodyIds.Length == 1)
                Physics.ContactModifyEvent += OnContactModify;
        }

        protected override bool IsIgnore(int selfId, int otherId, bool isSelfNonReactive, bool isOtherNonReactive)
        {
            return isOtherNonReactive && isSelfNonReactive;
        }
    }
}