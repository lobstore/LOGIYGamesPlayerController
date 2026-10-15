using UnityEngine;

namespace Ezereal
{
    public class EzerealWheelFrictionController : MonoBehaviour
    {
        [Header("Ezereal References")]
        [SerializeField] EzerealCarController ezerealCarController;

        WheelFrictionCurve driftfLWSidewaysFriction;
        WheelFrictionCurve driftfRWSidewaysFriction;
        WheelFrictionCurve driftrLWSidewaysFriction;
        WheelFrictionCurve driftrRWSidewaysFriction;

        WheelFrictionCurve defaultfLWSidewaysFriction;
        WheelFrictionCurve defaultfRWSidewaysFriction;
        WheelFrictionCurve defaultrLWSidewaysFriction;
        WheelFrictionCurve defaultrRWSidewaysFriction;

        WheelFrictionCurve fLWForwardFriction;
        WheelFrictionCurve fRWForwardFriction;
        WheelFrictionCurve rLWForwardFriction;
        WheelFrictionCurve rRWForwardFriction;

        void Start()
        {
            if (ezerealCarController != null)
            {
                SetForwardFriction();
                SetSidewaysFriction();
            }
            else
            {
                Debug.LogWarning("ezerealWheelFrictionController is missing ezerealCarController. Ignore it or attach one if you want to have friction controls.");
            }

        }

        void SetForwardFriction()
        {
            fLWForwardFriction = new WheelFrictionCurve
            {
                extremumSlip = ezerealCarController.frontLeftWheelCollider.forwardFriction.extremumSlip,
                extremumValue = ezerealCarController.frontLeftWheelCollider.forwardFriction.extremumValue,
                asymptoteSlip = ezerealCarController.frontLeftWheelCollider.forwardFriction.asymptoteSlip,
                asymptoteValue = ezerealCarController.frontLeftWheelCollider.forwardFriction.asymptoteValue,
                stiffness = ezerealCarController.frontLeftWheelCollider.forwardFriction.stiffness
            };

            fRWForwardFriction = new WheelFrictionCurve
            {
                extremumSlip = ezerealCarController.frontRightWheelCollider.forwardFriction.extremumSlip,
                extremumValue = ezerealCarController.frontRightWheelCollider.forwardFriction.extremumValue,
                asymptoteSlip = ezerealCarController.frontRightWheelCollider.forwardFriction.asymptoteSlip,
                asymptoteValue = ezerealCarController.frontRightWheelCollider.forwardFriction.asymptoteValue,
                stiffness = ezerealCarController.frontRightWheelCollider.forwardFriction.stiffness
            };

            rLWForwardFriction = new WheelFrictionCurve
            {
                extremumSlip = ezerealCarController.rearLeftWheelCollider.forwardFriction.extremumSlip,
                extremumValue = ezerealCarController.rearLeftWheelCollider.forwardFriction.extremumValue,
                asymptoteSlip = ezerealCarController.rearLeftWheelCollider.forwardFriction.asymptoteSlip,
                asymptoteValue = ezerealCarController.rearLeftWheelCollider.forwardFriction.asymptoteValue,
                stiffness = ezerealCarController.rearLeftWheelCollider.forwardFriction.stiffness
            };

            rRWForwardFriction = new WheelFrictionCurve
            {
                extremumSlip = ezerealCarController.rearRightWheelCollider.forwardFriction.extremumSlip,
                extremumValue = ezerealCarController.rearRightWheelCollider.forwardFriction.extremumValue,
                asymptoteSlip = ezerealCarController.rearRightWheelCollider.forwardFriction.asymptoteSlip,
                asymptoteValue = ezerealCarController.rearRightWheelCollider.forwardFriction.asymptoteValue,
                stiffness = ezerealCarController.rearRightWheelCollider.forwardFriction.stiffness
            };
        }

        void SetSidewaysFriction()
        {
            defaultfLWSidewaysFriction = new WheelFrictionCurve
            {
                extremumSlip = ezerealCarController.frontLeftWheelCollider.sidewaysFriction.extremumSlip,
                extremumValue = ezerealCarController.frontLeftWheelCollider.sidewaysFriction.extremumValue,
                asymptoteSlip = ezerealCarController.frontLeftWheelCollider.sidewaysFriction.asymptoteSlip,
                asymptoteValue = ezerealCarController.frontLeftWheelCollider.sidewaysFriction.asymptoteValue,
                stiffness = ezerealCarController.frontLeftWheelCollider.sidewaysFriction.stiffness
            };

            defaultfRWSidewaysFriction = new WheelFrictionCurve
            {
                extremumSlip = ezerealCarController.frontRightWheelCollider.sidewaysFriction.extremumSlip,
                extremumValue = ezerealCarController.frontRightWheelCollider.sidewaysFriction.extremumValue,
                asymptoteSlip = ezerealCarController.frontRightWheelCollider.sidewaysFriction.asymptoteSlip,
                asymptoteValue = ezerealCarController.frontRightWheelCollider.sidewaysFriction.asymptoteValue,
                stiffness = ezerealCarController.frontRightWheelCollider.sidewaysFriction.stiffness
            };

            defaultrLWSidewaysFriction = new WheelFrictionCurve
            {
                extremumSlip = ezerealCarController.rearLeftWheelCollider.sidewaysFriction.extremumSlip,
                extremumValue = ezerealCarController.rearLeftWheelCollider.sidewaysFriction.extremumValue,
                asymptoteSlip = ezerealCarController.rearLeftWheelCollider.sidewaysFriction.asymptoteSlip,
                asymptoteValue = ezerealCarController.rearLeftWheelCollider.sidewaysFriction.asymptoteValue,
                stiffness = ezerealCarController.rearLeftWheelCollider.sidewaysFriction.stiffness
            };

            defaultrRWSidewaysFriction = new WheelFrictionCurve
            {
                extremumSlip = ezerealCarController.rearRightWheelCollider.sidewaysFriction.extremumSlip,
                extremumValue = ezerealCarController.rearRightWheelCollider.sidewaysFriction.extremumValue,
                asymptoteSlip = ezerealCarController.rearRightWheelCollider.sidewaysFriction.asymptoteSlip,
                asymptoteValue = ezerealCarController.rearRightWheelCollider.sidewaysFriction.asymptoteValue,
                stiffness = ezerealCarController.rearRightWheelCollider.sidewaysFriction.stiffness
            };
            driftfLWSidewaysFriction = defaultfLWSidewaysFriction;
            driftfRWSidewaysFriction = defaultfRWSidewaysFriction;
            driftrLWSidewaysFriction = defaultrLWSidewaysFriction;
            driftrRWSidewaysFriction = defaultrRWSidewaysFriction;

        }

        public void StartDrifting(float currentHandbrakeValue)
        {
            if (ezerealCarController != null)
            {

                //driftrLWSidewaysFriction.extremumSlip = 3f * currentHandbrakeValue;
                //driftrRWSidewaysFriction.extremumSlip = 3f * currentHandbrakeValue;
                //driftrLWSidewaysFriction.extremumValue = 0.7f * currentHandbrakeValue;
                //driftrRWSidewaysFriction.extremumValue = 0.7f * currentHandbrakeValue;

                driftrLWSidewaysFriction.stiffness = 1f;
                driftrRWSidewaysFriction.stiffness = 1f;

                ezerealCarController.rearLeftWheelCollider.sidewaysFriction = driftrLWSidewaysFriction;
                ezerealCarController.rearRightWheelCollider.sidewaysFriction = driftrRWSidewaysFriction;
            }
        }

        public void StopDrifting()
        {
            if (ezerealCarController != null)
            {

                ezerealCarController.rearLeftWheelCollider.sidewaysFriction = defaultrLWSidewaysFriction;
                ezerealCarController.rearRightWheelCollider.sidewaysFriction = defaultrRWSidewaysFriction;
                ezerealCarController.frontLeftWheelCollider.sidewaysFriction = defaultfLWSidewaysFriction;
                ezerealCarController.frontRightWheelCollider.sidewaysFriction = defaultfRWSidewaysFriction;
            }
        }
    }

}
