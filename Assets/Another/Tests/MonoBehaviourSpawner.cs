using UnityEngine;

public class MonoBehaviourSpawner : MonoBehaviour
{
    public int objectCount = 10000; // Количество объектов

    void Start()
    {
        for (int i = 0; i < objectCount; i++)
        {
            GameObject obj = new GameObject("Mono_" + i);
            obj.AddComponent<MonoBehaviourMover>(); // Каждый объект сам обновляется
        }
    }
}
