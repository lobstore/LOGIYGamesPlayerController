using System.Threading.Tasks;
using UnityEngine;
// Этот класс висит на каждом объекте и вызывает свой Update()
public class MonoBehaviourMover : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private Vector3 rotationAxis;
    private float rotationSpeed;

    void Start()
    {
        direction = Random.insideUnitSphere.normalized; // Случайное направление
        speed = Random.Range(1f, 5f); // Случайная скорость движения

        rotationAxis = Random.insideUnitSphere.normalized; // Ось вращения
        rotationSpeed = Random.Range(30f, 180f); // Скорость вращения
    }

    void Update()
    {
        float dummy = 0f;
        Task.Run(() =>
        {
            for (int j = 0; j < 100; j++)
            {
                dummy += Mathf.Sin(j) * Mathf.Cos(j * 0.5f);
            }
        });
    }
}
