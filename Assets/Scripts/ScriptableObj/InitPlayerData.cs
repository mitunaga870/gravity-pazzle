#region

using UnityEngine;

#endregion

namespace ScriptableObj
{
    /// <summary>
    ///     初期セーブデータ
    /// </summary>
    [CreateAssetMenu(fileName = "初期セーブデータ", menuName = "ScriptableObj/初期セーブデータ", order = 0)]
    public class InitPlayerData : ScriptableObject
    {
        [Header("初期重力制限")]
        [SerializeField]
        [Min(0.1f)]
        private float operationDuration;

        public float OperationDuration => operationDuration;

        [SerializeField]
        [Min(1)]
        private int maxConcurrentOperations;

        public int MaxConcurrentOperations => maxConcurrentOperations;
    }
}