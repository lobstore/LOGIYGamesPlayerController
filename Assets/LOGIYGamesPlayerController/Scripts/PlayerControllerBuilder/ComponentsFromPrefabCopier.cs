using System;
using UnityEditor;
using UnityEngine;

namespace LOGIYGames
{
    public class ComponentsFromPrefabCopier : EditorWindow
    {
        private GameObject targetGameObject;
        private GameObject prefabTemplate;

        [MenuItem("Tools/Copy Components From Prefab")]
        public static void ShowWindow()
        {
            GetWindow<ComponentsFromPrefabCopier>("Copy Components From Prefab");
        }

        private void OnGUI()
        {
            GUILayout.Label("Копирование компонентов с префаба на GameObject", EditorStyles.boldLabel);

            targetGameObject = (GameObject)EditorGUILayout.ObjectField("Целевой GameObject", targetGameObject, typeof(GameObject), true);
            prefabTemplate = (GameObject)EditorGUILayout.ObjectField("Префаб-шаблон", prefabTemplate, typeof(GameObject), false);

            if (GUILayout.Button("Скопировать компоненты"))
            {
                if (targetGameObject == null || prefabTemplate == null)
                {
                    Debug.LogWarning("Выберите и целевой GameObject, и префаб-шаблон!");
                    return;
                }

                CopyComponentsFromPrefabToGameObject(prefabTemplate, targetGameObject);
            }
        }

        private void CopyComponentsFromPrefabToGameObject(GameObject prefab, GameObject target)
        {
            Undo.RegisterCompleteObjectUndo(target, "Копирование компонентов с префаба");

            // Получаем все компоненты префаба (кроме Transform)
            Component[] prefabComponents = prefab.GetComponents<Component>();
            foreach (var comp in prefabComponents)
            {
                if (comp is Transform)
                    continue; // Transform не копируем

                Type type = comp.GetType();

                // Если компонент уже есть — пропускаем или можно обновить значения
                Component existingComp = target.GetComponent(type);
                if (existingComp == null)
                {
                    existingComp = target.AddComponent(type);
                }

                // Копируем значения полей и свойств из префаба в компонент на целевом объекте
                CopyComponentValues(comp, existingComp);
            }

            Debug.Log($"Компоненты скопированы с префаба '{prefab.name}' на '{target.name}'");
        }

        private void CopyComponentValues(Component source, Component destination)
        {
            if (source == null || destination == null)
                return;

            SerializedObject sourceSO = new SerializedObject(source);
            SerializedObject destSO = new SerializedObject(destination);

            SerializedProperty prop = sourceSO.GetIterator();

            while (prop.NextVisible(true))
            {
                if (prop.name == "m_Script") // Не копируем ссылку на скрипт
                    continue;

                destSO.CopyFromSerializedProperty(prop);
            }

            destSO.ApplyModifiedProperties();
        }
    }
}