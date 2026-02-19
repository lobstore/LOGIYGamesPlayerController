using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
namespace LOGIYGames
{
    public class CinemachineMobileInputCotroller : InputAxisControllerBase<CinemachineMobileInputCotroller.DragReader>
    {
        
        private void Update()
        {
            if (Application.isPlaying)
            {
                UpdateControllers();
            }
        }
        [Serializable]
        public class DragReader : IInputAxisReader
        {
            public bool invert;
            public float horizicalSensetivity = 1;
            public float verticalSensetivity = 1;
            public DragPointerHandler DragInput;

            public float GetValue(UnityEngine.Object context, IInputAxisOwner.AxisDescriptor.Hints hint)
            {
                if (DragInput is not null)
                    switch (hint)
                    {
                        case IInputAxisOwner.AxisDescriptor.Hints.X:
                            if (!invert)
                            {
                                return DragInput.InputX * horizicalSensetivity;
                            }
                            else
                            {
                                return -DragInput.InputX * horizicalSensetivity;
                            }
                        case IInputAxisOwner.AxisDescriptor.Hints.Y:
                            if (!invert)
                            {
                                return -DragInput.InputY * verticalSensetivity;
                            }
                            else
                            {
                                return DragInput.InputX * verticalSensetivity;
                            }

                        default:
                            return 0;
                    }
                return 0;
            }
        }
    }
}
