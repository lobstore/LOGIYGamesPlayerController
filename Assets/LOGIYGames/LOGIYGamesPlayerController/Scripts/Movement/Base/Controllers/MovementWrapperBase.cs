using UnityEngine;

namespace LOGIYGames
{
    public abstract class MovementWrapperBase : MonoBehaviour
    {
        public virtual GroundedReport LastGroundedReport { get; }
        public abstract bool UseGravity { get; set; }
        public abstract float MaxStepHeight { get; }
        public abstract float Height { get; set; }
        public abstract float SlopeLimit { get; set; }
        public bool UseProjectionOnPlane { get; set; }
        public virtual bool IsNoClip { get; set; }
        public abstract Collider Collider { get; }
        public abstract Vector3 Center { get; set; }
        public virtual Vector3 Position => gameObject.transform.position;
        public virtual Quaternion Rotation => gameObject.transform.rotation;
        public virtual Transform Transform => gameObject.transform;
        public abstract Vector3 Velocity { get; }
        public abstract float Radius { get; set; }

        public virtual void AddAcceleration(Vector3 accelerationForce) { }
        public abstract void ChangeVelocity(Vector3 velocity);
        public abstract void AddImpulse(Vector3 impulseForce);
        public virtual void ForceMove(Vector3 velocity) { }
        public abstract void SetRotation(Quaternion rotation);
        public abstract void SetPosition(Vector3 position);
        public abstract void ResetGravity();
        public abstract void ResetVelocity();
        public abstract void DisableMovement();
        public abstract void EnableMovement();
    }
}