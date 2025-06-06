using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
namespace LOGIYGames
{
    public class VirtualCamProvider : MonoBehaviour
    {
        public static VirtualCamProvider Instance { get; private set; }
        [field: SerializeField] public GameObject FirstPlayerVirtualCameraPrefab { get; private set; }
        [field: SerializeField] public GameObject ThirdPlayerVirtualCameraPrefab { get; private set; }
        [field: SerializeField] public GameObject TopDownPlayerVirtualCameraPrefab { get; private set; }
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

    }
}