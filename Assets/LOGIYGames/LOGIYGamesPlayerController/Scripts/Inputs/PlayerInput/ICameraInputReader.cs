using UnityEngine;

namespace LOGIYGames
{
    public interface ICameraInputReader
    {
        public float ZoomDelta { get; }
        public Vector2 LookInput {  get; }
    }
}
