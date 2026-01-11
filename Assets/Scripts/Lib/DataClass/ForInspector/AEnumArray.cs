#region

using System;
using UnityEngine;

#endregion

namespace Lib.DataClass.ForInspector
{
    // データを保持するベースクラス
    [Serializable]
    public abstract class AEnumArray<TEnum, TValue>
        where TEnum : Enum
    {
        // 実体はただの配列
        // ★重要：変数名を固定しておき、Drawerから名前でアクセスします
        [SerializeField]
        public TValue[] values;

        // コンストラクタでサイズを確保
        public AEnumArray()
        {
            var length = Enum.GetNames(typeof(TEnum)).Length;
            values = new TValue[length];
        }

        // インデクサ（配列のように [] でアクセスできるようにする機能）
        public TValue this[TEnum key]
        {
            get => values[Convert.ToInt32(key)];
            set => values[Convert.ToInt32(key)] = value;
        }
    }
}