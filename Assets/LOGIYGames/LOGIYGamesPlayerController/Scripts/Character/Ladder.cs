using UnityEngine;
using UnityEngine.Splines;



namespace LOGIYGames.CharacterCore
{
    public class Ladder : MonoBehaviour
    {
        public SplineContainer spline;
        private void Awake()
        {
            Lenght = spline.CalculateLength();
        }
        public float Lenght { get; private set; }
        public Vector3 GetPosition(float t)
        {
            return spline.EvaluatePosition(t);
        }

        public Vector3 GetDirection(float t)
        {
            return ((Vector3)spline.EvaluateTangent(t)).normalized;
        }
    }
}
