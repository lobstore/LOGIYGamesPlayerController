using KinematicCharacterController;
using UnityEngine;

namespace LOGIYGames
{
    public class KinematicControllerWrapper : GenericControllerWrapper, ICharacterController
    {

        KinematicCharacterMotor KinematicCharacterMotor;

        public override bool IsGrounded => throw new System.NotImplementedException();

        public override Vector3 Velocity => throw new System.NotImplementedException();

        public override bool CollisionEnabled { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public override float MaxStepHeight { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public override float Height { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public override Vector3 Center { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public override float Radius { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public override bool ApplyGravityWhenGrounded => throw new System.NotImplementedException();

        public override float SlopeLimit { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public void AfterCharacterUpdate(float deltaTime)
        {
            
        }

        public void BeforeCharacterUpdate(float deltaTime)
        {
            
        }

        public override Vector3 GetCachedMoveDelta()
        {
            return Vector3.zero;
        }

        public override Quaternion GetCachedRotDelta()
        {
            return Quaternion.identity;
        }

        public override void Initialize()
        {
            
        }

        public bool IsColliderValidForCollisions(Collider coll)
        {
            return true;
        }

        public override void Jump(float force)
        {
            throw new System.NotImplementedException();
        }

        public override void Move(Vector3 a_move)
        {

        }

        public override void MoveAndRotate(Vector3 a_move, Quaternion a_rotDelta)
        {
            
        }

        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
           
        }

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
           
        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
          
        }

        public void PostGroundingUpdate(float deltaTime)
        {
           
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
           
        }

        public override void Rotate(Quaternion a_rotDelta)
        {
         
        }

        public override void SetPosition(Vector3 a_position)
        {
         
        }

        public override void SetPositionAndRotation(Vector3 a_position, Quaternion a_rotation)
        {
           
        }

        public override void SetRotation(Quaternion a_rotation)
        {
        
        }

        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            float currentVelocityMagnitude = currentVelocity.magnitude;

            Vector3 effectiveGroundNormal = KinematicCharacterMotor.GroundingStatus.GroundNormal;

            // Reorient velocity on slope
            currentVelocity = KinematicCharacterMotor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocityMagnitude;
            Vector3 targetMovementVelocity = currentVelocity;

            // Smooth movement Velocity
            currentVelocity = targetMovementVelocity;
        }
    }
}
