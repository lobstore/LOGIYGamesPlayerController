using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentSpawner : MonoBehaviour
{
    public int componentCount = 1000; // Количество компонентов

    void Start()
    {
        for (int i = 0; i < componentCount; i++)
        {
            gameObject.AddComponent<ExampleComponent>(); // Вешаем 1000 компонентов
        }
    }
}
