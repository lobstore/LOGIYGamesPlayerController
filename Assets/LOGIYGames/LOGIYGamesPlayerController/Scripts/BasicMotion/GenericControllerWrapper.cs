using UnityEngine;

namespace LOGIYGames
{
    /// <summary>
    /// Abstract base class for character controller wrappers.
    /// Provides a unified interface for both Unity CharacterController and KinematicCharacterController.
    /// </summary>
    public abstract class GenericControllerWrapper : MonoBehaviour
    {
        // Ground detection
        public abstract bool IsGrounded { get; }
        
        // Velocity
        public abstract Vector3 Velocity { get; }
        
        // Collision toggle
        public abstract bool CollisionEnabled { get; set; }
        
        // Movement methods
        public abstract void Move(Vector3 a_move);
        public abstract void MoveAndRotate(Vector3 a_move, Quaternion a_rotDelta);
        public abstract void Rotate(Quaternion a_rotDelta);
        
        // Capsule collider properties (both controllers use capsule collision)
        public abstract float MaxStepHeight { get; set; }
        public abstract float Height { get; set; }
        public abstract float SlopeLimit { get; set; }
        public abstract Vector3 Center { get; set; }
        public abstract float Radius { get; set; }
        
        // Gravity settings
        public abstract bool ApplyGravityWhenGrounded { get; }
        
        // Lifecycle
        public abstract void Initialize();
        
        // Transform manipulation
        public abstract void SetPosition(Vector3 a_position);
        public abstract void SetRotation(Quaternion a_rotation);
        public abstract void SetPositionAndRotation(Vector3 a_position, Quaternion a_rotation);
        
        // Cached movement data (for rollback/netcode support)
        public abstract Vector3 GetCachedMoveDelta();
        public abstract Quaternion GetCachedRotDelta();
        
        // Jumping
        public abstract void Jump(float force);
        
        /// <summary>
        /// Returns the underlying collider component (CharacterController or CapsuleCollider)
        /// </summary>
        public abstract Collider GetCollider();
        
        /// <summary>
        /// Returns the transform of the character
        /// </summary>
        public new Transform transform { get { return base.transform; } }
        
    }
}
