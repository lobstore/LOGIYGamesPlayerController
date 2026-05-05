using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;



namespace LOGIYGames.CharacterCore
{
    public class Ladder : MonoBehaviour
    {
        public SplineContainer spline;

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
