using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Ezereal
{
    public class EzerealCarController : MonoBehaviour
    {
        public readonly UnityEvent<bool> EngineStarted = new();
        public readonly UnityEvent GearChanged = new();
        public readonly UnityEvent BrakeChanged = new();

        public AutomaticGears CurrentGear => currentGear;
        public float CurrentSpeed => currentSpeed;
        public float CurrentAccelerationValue => currentAccelerationValue;
        public float CurrentBreakValue => currentBrakeValue;
        public float CurrentHandBreakValue => currentHandbrakeValue;
        public bool IsBraking { get; private set; }
        public bool IsHandBraking { get; private set; }
        public bool IsStarted => isStarted;

        [Header("Ezereal References")]
        [SerializeField] EzerealWheelFrictionController ezerealWheelFrictionController;

        [Header("References")]

        public Rigidbody vehicleRB;
        public WheelCollider frontLeftWheelCollider;
        public WheelCollider frontRightWheelCollider;
        public WheelCollider rearLeftWheelCollider;
        public WheelCollider rearRightWheelCollider;
        WheelCollider[] wheels;

        [SerializeField] Transform frontLeftWheelMesh;
        [SerializeField] Transform frontRightWheelMesh;
        [SerializeField] Transform rearLeftWheelMesh;
        [SerializeField] Transform rearRightWheelMesh;

        [SerializeField] Transform steeringWheel;
        [SerializeField] TMP_Text currentGearTMP_Dashboard;
        [SerializeField] TMP_Text currentSpeedTMP_Dashboard;
        [SerializeField] Slider accelerationSlider;

        [Header("Settings")]
        [SerializeField] bool isStarted;

        public float maxForwardSpeed = 100f; // 100f default
        public float maxReverseSpeed = 30f; // 30f default
        public float horsePower = 1000f; // 100f0 default
        public float brakePower = 2000f; // 2000f default
        public float handbrakeForce = 3000f; // 3000f default
        public float maxSteerAngle = 30f; // 30f default
        public float steeringSpeed = 5f; // 0.5f default
        public float stopThreshold = 1f; // 1f default. At what speed car will make a full stop
        public float decelerationSpeed = 100f;
        public float maxSteeringWheelRotation = 360f; // 360 for real steering wheel. 120 would be more suitable for racing.

        [Header("Drive Type")]
        public DriveTypes driveType = DriveTypes.RWD;

        [Header("Gearbox")]
        [SerializeField] AutomaticGears currentGear = AutomaticGears.Drive;
        [Header("Debug Info")]
        public bool stationary = true;
        [SerializeField] float currentSpeed = 0f;

        [SerializeField] float currentAccelerationValue = 0f;
        [SerializeField] float currentBrakeValue = 0f;
        [SerializeField] float currentHandbrakeValue = 0f;
        [SerializeField] float currentSteerAngle = 0f;
        [SerializeField] float targetSteerAngle = 0f;
        [SerializeField] float FrontLeftWheelRPM = 0f;
        [SerializeField] float FrontRightWheelRPM = 0f;
        [SerializeField] float RearLeftWheelRPM = 0f;
        [SerializeField] float RearRightWheelRPM = 0f;

        [SerializeField] float speedFactor = 0f; // Leave at zero. Responsible for smooth acceleration and near-top-speed slowdown.

        private void Awake()
        {
            wheels = new WheelCollider[]
            {
            frontLeftWheelCollider,
            frontRightWheelCollider,
            rearLeftWheelCollider,
            rearRightWheelCollider,
            };

            if (ezerealWheelFrictionController == null)
            {
                Debug.LogWarning("EzerealWheelFrictionController reference is missing. Ignore or attach one if you want to have friction controls.");
            }

            if (vehicleRB == null)
            {
                Debug.LogError("VehicleRB reference is missing for EzerealCarController!");
            }
            EngineStarted.AddListener((_) =>
            {
                if (!isStarted)
                {
                    frontLeftWheelCollider.motorTorque = 0;
                    frontRightWheelCollider.motorTorque = 0;
                    rearLeftWheelCollider.motorTorque = 0;
                    rearRightWheelCollider.motorTorque = 0;
                }
            });
        }
        private void Start()
        {
            OnStartCar();

        }
        void OnStartCar()
        {
            isStarted = !isStarted;
            EngineStarted.Invoke(isStarted);
        }

        void OnAccelerate(InputValue accelerationValue)
        {
           
            switch (currentGear)
            {
                case AutomaticGears.Reverse:
                    currentAccelerationValue = -accelerationValue.Get<float>();
                    break;
                case AutomaticGears.Neutral:

                    break;
                case AutomaticGears.Drive:
                    currentAccelerationValue = accelerationValue.Get<float>();
                    break;
                default:
                    break;
            }
        }

        void Acceleration()
        {

            if (isStarted)
            {

                switch (driveType)
                {
                    case DriveTypes.RWD:
                        rearLeftWheelCollider.motorTorque = horsePower * currentAccelerationValue;
                        rearRightWheelCollider.motorTorque = horsePower * currentAccelerationValue;

                        break;
                    case DriveTypes.FWD:
                        frontLeftWheelCollider.motorTorque = horsePower * currentAccelerationValue;
                        frontRightWheelCollider.motorTorque = horsePower * currentAccelerationValue;

                        break;
                    case DriveTypes.AWD:
                        frontLeftWheelCollider.motorTorque = horsePower * currentAccelerationValue;
                        frontRightWheelCollider.motorTorque = horsePower * currentAccelerationValue;
                        rearLeftWheelCollider.motorTorque = horsePower * currentAccelerationValue;
                        rearRightWheelCollider.motorTorque = horsePower * currentAccelerationValue;


                        break;
                    default:
                        break;
                }
                UpdateAccelerationSlider();
            }
        }

        void OnBrake(InputValue brakeValue)
        {
            currentBrakeValue = brakeValue.Get<float>();
            if (currentBrakeValue > 0)
            {
                IsBraking = true;
            }
            else
            {
                IsBraking = false;
            }
        }

        void Braking()
        {
            if (currentBrakeValue > 0f)
            {
                frontLeftWheelCollider.brakeTorque = currentBrakeValue * brakePower;
                frontRightWheelCollider.brakeTorque = currentBrakeValue * brakePower;
                frontLeftWheelCollider.motorTorque = 0;
                frontRightWheelCollider.motorTorque = 0;
                rearLeftWheelCollider.brakeTorque = currentBrakeValue * brakePower;
                rearRightWheelCollider.brakeTorque = currentBrakeValue * brakePower;
                rearLeftWheelCollider.motorTorque = 0;
                rearRightWheelCollider.motorTorque = 0;

            }
            else
            {
                frontLeftWheelCollider.brakeTorque = 0;
                frontRightWheelCollider.brakeTorque = 0;
                rearLeftWheelCollider.brakeTorque = 0;
                rearRightWheelCollider.brakeTorque = 0;
            }
        }

        void OnHandbrake(InputValue handbrakeValue)
        {
            currentHandbrakeValue = handbrakeValue.Get<float>();

            if (isStarted)
            {
                if (currentHandbrakeValue > 0)
                {
                    if (ezerealWheelFrictionController != null)
                    {
                        ezerealWheelFrictionController.StartDrifting(currentHandbrakeValue);
                    }

                    IsHandBraking = true;
                }
                else
                {
                    if (ezerealWheelFrictionController != null)
                    {
                        ezerealWheelFrictionController.StopDrifting();
                    }

                    IsHandBraking = false;
                }
            }
        }

        void Handbraking()
        {
            if (currentHandbrakeValue > 0f)
            {
                rearLeftWheelCollider.motorTorque = 0;
                rearRightWheelCollider.motorTorque = 0;
                rearLeftWheelCollider.brakeTorque = currentHandbrakeValue * handbrakeForce;
                rearRightWheelCollider.brakeTorque = currentHandbrakeValue * handbrakeForce;


            }
            else
            {
                rearLeftWheelCollider.brakeTorque = 0;
                rearRightWheelCollider.brakeTorque = 0;
            }
        }

        void OnSteer(InputValue turnValue)
        {
            targetSteerAngle = turnValue.Get<float>() * maxSteerAngle;
        }

        void Steering()
        {
            //float adjustedspeedFactor = Mathf.InverseLerp(20, maxForwardSpeed, currentSpeed); //minimum speed affecting steerAngle is 20
            //float adjustedTurnAngle = targetSteerAngle * (1 - adjustedspeedFactor); //based on current speed.
            currentSteerAngle = Mathf.Lerp(currentSteerAngle, targetSteerAngle, Time.deltaTime * steeringSpeed);

            frontLeftWheelCollider.steerAngle = currentSteerAngle;
            frontRightWheelCollider.steerAngle = currentSteerAngle;

            UpdateWheel(frontLeftWheelCollider, frontLeftWheelMesh);
            UpdateWheel(frontRightWheelCollider, frontRightWheelMesh);
            UpdateWheel(rearLeftWheelCollider, rearLeftWheelMesh);
            UpdateWheel(rearRightWheelCollider, rearRightWheelMesh);
        }

        void OnDownShift()
        {

            switch (currentGear)
            {
                case AutomaticGears.Reverse:
                    //Debug.Log("Reverse, can't go any lower");
                    break;

                case AutomaticGears.Neutral:
                    currentGear--;
                    UpdateGearText("R");
                    break;
                case AutomaticGears.Drive:
                    currentGear--;
                    UpdateGearText("N");
                    break;
            }
            GearChanged.Invoke();
        }

        void OnUpShift()
        {
            switch (currentGear)
            {
                case AutomaticGears.Reverse:
                    currentGear++;
                    UpdateGearText("N");
                    break;
                case AutomaticGears.Neutral:
                    currentGear++;
                    UpdateGearText("D");
                    break;
                case AutomaticGears.Drive:
                    //Debug.Log("Drive, can't go any higher");
                    break;
            }
            GearChanged.Invoke();
        }


        private void FixedUpdate()
        {
            Acceleration();
            Braking();

            Handbraking();

            Steering();


            RotateSteeringWheel();

            if
                (
                    Mathf.Abs(frontLeftWheelCollider.rpm) < stopThreshold &&
                    Mathf.Abs(frontRightWheelCollider.rpm) < stopThreshold &&
                    Mathf.Abs(rearLeftWheelCollider.rpm) < stopThreshold &&
                    Mathf.Abs(rearRightWheelCollider.rpm) < stopThreshold
                )
            {
                stationary = true;
            }
            else
            {
                stationary = false;
            }

            if (vehicleRB != null) // Unity uses m/s as for default. So I convert from m/s to km/h. For mph use 2.23694f instead of 3.6f.
            {
#if UNITY_6000_0_OR_NEWER
                currentSpeed = Vector3.Dot(vehicleRB.gameObject.transform.forward, vehicleRB.linearVelocity);
                currentSpeed *= 3.6f;
#else
                currentSpeed = Vector3.Dot(vehicleRB.gameObject.transform.forward, vehicleRB.velocity);
                currentSpeed *= 3.6f; 
#endif
                UpdateSpeedText(currentSpeed);

            }


            FrontLeftWheelRPM = frontLeftWheelCollider.rpm;
            FrontRightWheelRPM = frontRightWheelCollider.rpm;
            RearLeftWheelRPM = rearLeftWheelCollider.rpm;
            RearRightWheelRPM = rearRightWheelCollider.rpm;
        }

        private void UpdateWheel(WheelCollider col, Transform mesh)
        {
            col.GetWorldPose(out Vector3 position, out Quaternion rotation);
            mesh.SetPositionAndRotation(position, rotation);
        }


        void RotateSteeringWheel()
        {
            if (steeringWheel == null) return;
            float currentXAngle = steeringWheel.transform.localEulerAngles.x; // Maximum steer angle in degrees

            // Calculate the rotation based on the steer angle
            float normalizedSteerAngle = Mathf.Clamp(frontLeftWheelCollider.steerAngle, -maxSteerAngle, maxSteerAngle);
            float rotation = Mathf.Lerp(maxSteeringWheelRotation, -maxSteeringWheelRotation, (normalizedSteerAngle + maxSteerAngle) / (2 * maxSteerAngle));

            // Set the local rotation of the steering wheel
            steeringWheel.localRotation = Quaternion.Euler(currentXAngle, 0, rotation);
        }

        void UpdateGearText(string gear)
        {
            if (currentGearTMP_Dashboard == null) return;
            currentGearTMP_Dashboard.text = gear;
        }

        void UpdateSpeedText(float speed)
        {
            speed = Mathf.Abs(speed);
            currentSpeedTMP_Dashboard.text = speed.ToString("F0");
        }

        void UpdateAccelerationSlider()
        {
            if (currentGear == AutomaticGears.Drive || currentGear == AutomaticGears.Reverse)
            {
                accelerationSlider.value = Mathf.Lerp(accelerationSlider.value, currentAccelerationValue, Time.deltaTime * 15f);
            }
            else
            {
                accelerationSlider.value = 0;
            }
        }

        public bool InAir()
        {
            foreach (WheelCollider wheel in wheels)
            {
                if (wheel.GetGroundHit(out _))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
