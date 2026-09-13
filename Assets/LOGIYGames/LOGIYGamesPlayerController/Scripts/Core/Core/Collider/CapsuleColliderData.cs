using UnityEngine;

namespace GenshinImpactMovementSystem
{
    public class CapsuleColliderData
    {
        public CapsuleCollider Collider { get; private set; }
        public Vector3 ColliderCenterInLocalSpace { get; private set; }
        public Vector3 ColliderVerticalExtents { get; private set; }

        public void Initialize(CapsuleCollider coll)
        {
            if (Collider != null)
            {
                return;
            }

            Collider = coll;

            UpdateColliderData();
        }

        public void UpdateColliderData()
        {
            ColliderCenterInLocalSpace = Collider.center;

            ColliderVerticalExtents = new Vector3(0f, Collider.bounds.extents.y, 0f);
        }
    }
}