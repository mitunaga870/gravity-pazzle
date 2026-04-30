using System;
using Behaviour.UI.InGame;
using UnityEngine;

namespace Lib.DataClass
{
    [Serializable]
    public class IngameTutorialUI
    {
        [SerializeField]
        private GameObject _ui;

        [SerializeField]
        private TutorialState _state;

        public GameObject UI => _ui;
        public TutorialState State => _state;
    }
}