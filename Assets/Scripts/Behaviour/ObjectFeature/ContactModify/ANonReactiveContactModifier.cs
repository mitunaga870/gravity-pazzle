#region

using Behaviour.Controller.Stage;
using Unity.Collections;
using UnityEngine;

#endregion

namespace Behaviour.ObjectFeature.ContactModify
{
    /// <summary>
    ///     反作用を起こさないオブジェクトに関するコンタクトモディファイアの抽象クラス
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public abstract class ANonReactiveContactModifier : MonoBehaviour
    {
        protected abstract int BodyId { get; }

        protected virtual void OnEnable()
        {
            var _collider = GetComponent<Collider>();
            _collider.hasModifiableContacts = true;
            Physics.ContactModifyEvent += OnContactModify;
        }

        private void OnDisable()
        {
            Physics.ContactModifyEvent -= OnContactModify;
        }


        /// <summary>
        ///     反応するか無視するかの条件を判定する
        /// </summary>
        protected abstract bool IsIgnore(int selfId, int otherId, bool isSelfNonReactive, bool isOtherNonReactive);

        /// <summary>
        ///     反作用を起こさないオブジェクトとの接触か確認する
        /// </summary>
        /// <param name="selfId"></param>
        /// <param name="otherId"></param>
        /// <returns></returns>
        private (bool, bool) CheckNonReactiveContact(int selfId, int otherId)
        {
            var isSelfNonReactive = false;
            var isOtherNonReactive = false;

            var nonReactiveIds = StageDataController.Instance.NonReactiveBodyIds;

            // 反作用を起こさないオブジェクトとの反応か確認
            foreach (var id in nonReactiveIds)
            {
                if (selfId == id)
                    isSelfNonReactive = true;
                if (otherId == id)
                    isOtherNonReactive = true;

                if (isSelfNonReactive && isOtherNonReactive) break;
            }

            return (isSelfNonReactive, isOtherNonReactive);
        }


        /// <summary>
        ///     isIgnore の条件に基づいて接触を無視するコンタクトモディファイア
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="contacts"></param>
        protected void OnContactModify(PhysicsScene scene, NativeArray<ModifiableContactPair> contacts)
        {
            for (var i = 0; i < contacts.Length; i++)
            {
                var contact = contacts[i];

                // プレイヤーとの接触か確認
                var otherId = contact.otherBodyInstanceID;
                var selfId = contact.bodyInstanceID;

                var (isSelfNonReactive, isOtherNonReactive) = CheckNonReactiveContact(selfId, otherId);

                var ignore = IsIgnore(selfId, otherId, isSelfNonReactive, isOtherNonReactive);

                if (ignore)
                    for (var j = 0; j < contact.contactCount; j++)
                        contact.IgnoreContact(j);

                contacts[i] = contact;
            }
        }
    }
}