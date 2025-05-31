#region

using Behaviour.Gravity.Abstract;

#endregion

namespace Behaviour.Gravity
{
    public class SGravBehaviour : AGravBehaviour
    {
        protected override void FixedUpdate()
        {
            GravAffectionContext.OnFixedUpdate();
        }
    }
}