using UnityEditor;
using UnityEngine;

namespace LOGIYGames
{
    public class ControllerBuilderWindow : EditorWindow
    {
        private GameObject targetGameObject;
        private ControllerComponentsSO componentsSO;

        [MenuItem("Tools/Controller Builder")]
        public static void ShowWindow()
        {
            GetWindow<ControllerBuilderWindow>("Controller Builder");
        }

        private void OnGUI()
        {
            GUILayout.Label("Добавить компоненты на GameObject", EditorStyles.boldLabel);

            targetGameObject = (GameObject)EditorGUILayout.ObjectField("Целевой GameObject", targetGameObject, typeof(GameObject), true);
            componentsSO = (ControllerComponentsSO)EditorGUILayout.ObjectField("ScriptableObject с компонентами", componentsSO, typeof(ControllerComponentsSO), false);

            if (GUILayout.Button("Добавить компоненты"))
            {
                AddComponentsToGameObject();
            }
        }

        private void AddComponentsToGameObject()
        {
            if (targetGameObject == null)
            {
                Debug.LogWarning("Не выбран целевой GameObject!");
                return;
            }

            if (componentsSO == null)
            {
                Debug.LogWarning("Не выбран ScriptableObject с компонентами!");
                return;
            }

            Undo.RecordObject(targetGameObject, "Добавление компонентов");

            foreach (var compWrapper in componentsSO.componentsToAdd)
            {
                var type = compWrapper.GetComponentType();
                if (type == null)
                {
                    Debug.LogWarning($"Не найден тип компонента: {compWrapper.componentTypeName}");
                    continue;
                }

                if (!typeof(Component).IsAssignableFrom(type))
                {
                    Debug.LogWarning($"{compWrapper.componentTypeName} не является компонентом Unity");
                    continue;
                }

                if (targetGameObject.GetComponent(type) == null)
                {
                    targetGameObject.AddComponent(type);
                    Debug.Log($"Добавлен компонент {type.Name} на {targetGameObject.name}");
                }
                else
                {
                    Debug.Log($"{targetGameObject.name} уже содержит компонент {type.Name}");
                }
            }
        }
    }
}
