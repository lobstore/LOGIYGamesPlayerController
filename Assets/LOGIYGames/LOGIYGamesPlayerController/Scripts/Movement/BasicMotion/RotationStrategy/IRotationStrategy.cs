using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace LOGIYGames
{
    public interface IRotationStrategy
    {

        public Quaternion GetRotation();
    }

}

