using UnityEngine;

namespace LOGIYGames
{
    public abstract class ControllerWrapperBase : MonoBehaviour
    {
        public virtual GroundedReport LastGroundedReport { get; }
        public abstract bool UseGravity { get; set; }
        public abstract float MaxStepHeight { get; set; }
        public abstract float Height { get; set; }
        public abstract float SlopeLimit { get; set; }
        public bool UseProjectionOnPlane {  get; set; }
        public virtual bool IsNoClip { private get; set; }
        public virtual Collider Collider => null;
        public abstract Vector3 Center { get; set; }
        public virtual Vector3 Position => gameObject.transform.position;
        public virtual Quaternion Rotation => gameObject.transform.rotation;
        public virtual Transform Transform => gameObject.transform;
        public abstract Vector3 Velocity { get; }
        public abstract float Radius { get; set; }
        public abstract void Move(Vector3 a_move);
        public virtual void ForceMove(Vector3 a_move) { }
        public abstract void SetRotation(Quaternion a_targetRotation);
        public abstract void SetPosition(Vector3 a_position);
        public abstract void AddForce(Vector3 force);
        public abstract void ResetVelocity();
    }
}
