using UnityEngine;
// Этот класс не наследуется от MonoBehaviour и вызывается вручную
public class ManualMover
{
    private Transform transform;
    private Vector3 direction;
    private float speed;
    private Vector3 rotationAxis;
    private float rotationSpeed;

    public ManualMover(Transform objTransform)
    {
        transform = objTransform;
        direction = Random.insideUnitSphere.normalized; // Случайное направление
        speed = Random.Range(1f, 5f); // Случайная скорость

        rotationAxis = Random.insideUnitSphere.normalized; // Ось вращения
        rotationSpeed = Random.Range(30f, 180f); // Скорость вращения
    }

    public void ManualUpdateLogic(float time)
    {
        float dummy = 0f;
        for (int j = 0; j < 100; j++)
        {
            dummy += Mathf.Sin(j) * Mathf.Cos(j * 0.5f);
        }
    }
    public void ManualUpdatePhisics(float time)
    {
        var newPosition = direction * speed * time;
        // Двигаем объект
        if (transform.position != newPosition)
        {
            transform.position = newPosition;

        }
    }
}
