using UnityEngine;



namespace LOGIYGames.CharacterCore
{
    [System.Serializable]
    public class CharacterVelocityData
    {
        /// <summary>
        /// Movement velocity (walk/run/air control)
        /// </summary>
        public Vector3 Locomotion;

        /// <summary>
        /// Gravity + jump
        /// </summary>
        public Vector3 Gravity;

        /// <summary>
        /// Dash / knockback / external forces
        /// </summary>
        public Vector3 External;

        public Vector3 Total =>
            Locomotion + Gravity + External;

        public void Reset()
        {
            Locomotion = Vector3.zero;
            Gravity = Vector3.zero;
            External = Vector3.zero;
        }

        public void ResetHorizontal()
        {
            Locomotion = Vector3.zero;
            External = Vector3.zero;
        }

        public void ResetVertical()
        {
            Gravity = Vector3.zero;
        }
    } 

}
