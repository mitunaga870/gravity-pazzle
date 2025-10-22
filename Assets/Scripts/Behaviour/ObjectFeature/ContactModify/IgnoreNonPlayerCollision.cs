#region

using Behaviour.Controller.Stage;
using Unity.Collections;
using UnityEngine;

#endregion

namespace Behaviour.ObjectFeature.ContactModify
{
    /// <summary>
    ///     プレイヤー以外との衝突を無視するオブジェクトのクラス
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class IgnoreNonPlayerCollision : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        private int _bodyId;

        private void OnEnable()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _bodyId = _rigidbody.GetInstanceID();

            var _collider = GetComponent<Collider>();
            _collider.hasModifiableContacts = true;
            Physics.ContactModifyEvent += OnContactModify;
        }

        private void OnDisable()
        {
            Physics.ContactModifyEvent -= OnContactModify;
        }

        private void OnContactModify(PhysicsScene scene, NativeArray<ModifiableContactPair> contacts)
        {
            for (var i = 0; i < contacts.Length; i++)
            {
                var contact = contacts[i];

                // プレイヤーとの接触か確認
                var otherId = contact.otherBodyInstanceID;
                var selfId = contact.bodyInstanceID;

                if (
                    (otherId == StageDataController.Instance.PlayerBodyInstanceID &&
                     selfId == _bodyId) ||
                    (selfId == StageDataController.Instance.PlayerBodyInstanceID &&
                     otherId == _bodyId)
                )
                {
                }
                else if (otherId == _bodyId || selfId == _bodyId)
                {
                    for (var j = 0; j < contact.contactCount; j++)
                        contact.IgnoreContact(j);
                }

                contacts[i] = contact;
            }
        }
    }
}