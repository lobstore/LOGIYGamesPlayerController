using UnityEngine;

public class ExampleComponent : MonoBehaviour
{
    private Vector3 rotationAxis;
    private float rotationSpeed;

    void Start()
    {
        rotationAxis = Random.insideUnitSphere.normalized; // Случайная ось вращения
        rotationSpeed = Random.Range(30f, 180f); // Скорость вращения
    }

    void Update()
    {
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime); // Вращаем объект
    }
}