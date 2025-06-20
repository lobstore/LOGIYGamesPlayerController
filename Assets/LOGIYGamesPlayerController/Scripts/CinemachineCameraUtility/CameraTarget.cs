using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
namespace LOGIYGames
{
    public class CameraTarget : MonoBehaviour
    {
        [field: SerializeField] public Transform Follow { get; private set; }
        [field: SerializeField] public Transform LookAt { get; private set; }

    }
}