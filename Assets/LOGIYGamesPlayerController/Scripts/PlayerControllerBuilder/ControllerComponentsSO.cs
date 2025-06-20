using System;
using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "ControllerComponents", menuName = "Scriptable Objects/ControllerComponents")]
    public class ControllerComponentsSO
        : ScriptableObject
    {
        public List<ComponentTypeWrapper> componentsToAdd = new List<ComponentTypeWrapper>();
    }
    [Serializable]
    public class ComponentTypeWrapper
    {
        public string componentTypeName;

        public Type GetComponentType()
        {
            return Type.GetType(componentTypeName);
        }
    }
}
