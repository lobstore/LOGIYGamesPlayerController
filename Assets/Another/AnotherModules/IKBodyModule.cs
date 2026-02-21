using UnityEngine;

namespace LOGIYGames
{
    public class IKBodyModule : MonoModuleBase
    {
        [SerializeField]
        private Vector3 m_LookOffset = new Vector3(0f, 1.5f, 3f);
        [SerializeField]
        private float m_BodyWeight = 0.6f;
        [SerializeField]
        private float m_HeadWeight = 0.2f;
        [SerializeField]
        private float m_EyesWeight = 0.2f;
        [SerializeField]
        private float m_ClampWeight = 0.35f;        
        [SerializeField]
        private float maxLookAngle = 90;

        private float m_Weight = 0f;
        private Transform m_CameraTransform;
        private Transform m_Transform;
        private Animator m_Animator;
        private Vector3 m_AimPosition;
        private bool m_ControllerActive = true;

        private bool ik = true;

        private void Start()
        {
            this.m_CameraTransform = Camera.main.transform;
            this.m_Transform = transform;
            this.m_Animator = GetComponent<Animator>();
        }

        override public void OnUpdate(float deltaTime)
        {
            float relativeX = this.m_CameraTransform.InverseTransformPoint(this.m_Transform.position).x;
            this.m_AimPosition = this.m_Transform.position + this.m_CameraTransform.forward * this.m_LookOffset.z + Vector3.up * this.m_LookOffset.y + this.m_CameraTransform.right * (this.m_LookOffset.x - relativeX * 2f);
            Vector3 directionToTarget = this.m_Transform.position - this.m_CameraTransform.position;
            float angle = Vector3.Angle(this.m_Transform.forward, directionToTarget);
            if (Mathf.Abs(angle) < maxLookAngle && this.m_ControllerActive && ik)
            {
                this.m_Weight = Mathf.Lerp(this.m_Weight, 1f, deltaTime);
            }
            else
            {
                this.m_Weight = Mathf.Lerp(this.m_Weight, 0f, deltaTime * 2f);
            }
        }

        public void SetIK(bool state)
        {
            ik = state;
        }

        private void OnAnimatorIK(int layer)
        {

            if (layer == 0)
            {
                this.m_Animator.SetLookAtPosition(this.m_AimPosition);
                this.m_Animator.SetLookAtWeight(this.m_Weight, this.m_BodyWeight, this.m_HeadWeight, this.m_EyesWeight, this.m_ClampWeight);
            }
        }
    }
}
