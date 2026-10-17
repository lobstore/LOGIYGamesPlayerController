using UnityEngine;
using UnityEngine.Splines;
namespace LOGIYGames.CharacterCore
{
    public class Zipline : MonoBehaviour
    {
        public SplineAnimate Animate;
        public bool Finished { get; private set; }
        private void Awake()
        {
            Animate.Completed += Complete;
        }

        private void Complete()
        {
            Finished = true;
        }
        public void Run()
        {
            Animate.Play();
        }
    }
}
