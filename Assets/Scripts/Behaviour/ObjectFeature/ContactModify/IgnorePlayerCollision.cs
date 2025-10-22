#region

using Behaviour.Controller.Stage;
using Unity.Collections;
using UnityEngine;

#endregion

namespace Behaviour.ObjectFeature.ContactModify
{
    /// <summary>
    ///     プレイヤーとの衝突を無視するオブジェクトのクラス
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class IgnorePlayerCollision : MonoBehaviour
    {
        private Rigidbody _rigidbody;

        private void OnEnable()
        {
            _rigidbody = GetComponent<Rigidbody>();

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
                         selfId == _rigidbody.GetInstanceID()) ||
                        (selfId == StageDataController.Instance.PlayerBodyInstanceID &&
                         otherId == _rigidbody.GetInstanceID())
                    )
                    // 衝突を無効化
                    for (var j = 0; j < contact.contactCount; j++)
                        contact.IgnoreContact(j);

                contacts[i] = contact;
            }
        }
    }
}