using Cinemachine;
using UnityEngine;
namespace LOGIYGames
{
    public class CinemachineMobileInputProvider : CinemachineInputProvider
    {
        [SerializeField] DragPointerHandler DragInput;
        [SerializeField] bool IsMobile;
        public override float GetAxisValue(int axis)
        {
            if (IsMobile)
            {
                return MobileGetAxisValue(axis);
            }
            else
            {
                return base.GetAxisValue(axis);

            }

        }

        private float MobileGetAxisValue(int axis)
        {
            if (enabled)
            {

                switch (axis)
                {
                    case 0:
                        return DragInput.InputX;
                    case 1:
                        return DragInput.InputY;

                }
            }
            return 0f;
        }
    }
}