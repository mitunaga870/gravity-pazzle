#region

using Behaviour.Controller.General;
using UnityEngine;

#endregion

namespace Behaviour.ObjectFeature.@interface
{
    public interface IInputBase
    {
        [SerializeField]
        InputController Input { get; set; }
    }
}