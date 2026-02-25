using LOGIYGames.Timers;
using UnityEngine;
using UniVRM10;

public class EyesNystagmusModule : MonoModuleBase
{
    [Header("LookAt Settings")]
    [SerializeField] private Vrm10Instance _vrm10;
    private Vrm10Runtime Vrm10Runtime;
    Transform TargetTransform;

    [Header("Random Offset Settings")]
    [SerializeField] private float maxOffsetRadius = 0.1f; // Максимальный радиус случайного смещения
    [SerializeField] private Vector2 offsetIntervalRange = new Vector2(2f, 5f); // Интервал между смещениями (мин, макс)
    [SerializeField] private AnimationCurve distanceToOffsetCurve; // Кривая для зависимости радиуса от расстояния

    [SerializeField] private Transform SightTransform;
    private Vector3? LookTargetPosition { get => Vrm10Runtime.LookAt.LookAtInput.WorldPosition; set => Vrm10Runtime.LookAt.LookAtInput = new LookAtInput { WorldPosition = value }; }
    private Vector3 currentOffset;
    private Vector3 targetOffset;
    private float threshold = 0.01f;
    private float offsetSmoothTime = 20f;


    [Header("Distance Settings")]
    [SerializeField] private float minDistance = 0.1f; // Минимальное расстояние для расчета
    [SerializeField] private float maxDistance = 5f; // Максимальное расстояние для расчета
    private CountdownTimer offsetTimer;
    private void Awake()
    {
        Vrm10Runtime = _vrm10.Runtime;
    }
    private void Start()
    {
        offsetTimer = new CountdownTimer(GetRandomOffsetInterval());
        offsetTimer.Start();

    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
        ProcessNystagmus();
        LookTargetPosition = SightTransform.position + transform.forward+ currentOffset;
    }
    private void ProcessNystagmus()
    {
        if (offsetTimer.IsFinished)
        {
            // Получаем текущий радиус смещения на основе расстояния
            float currentRadius = GetCurrentOffsetRadius();

            // Генерируем новое случайное смещение
            targetOffset = Random.insideUnitSphere * currentRadius;
            offsetTimer.Reset(GetRandomOffsetInterval());
            offsetTimer.Start();
        }

        if (currentOffset != targetOffset)
        {
            // Плавное изменение смещения
            if (AbsDifference() >= threshold)
            {
                currentOffset = Vector3.Lerp(currentOffset, targetOffset, Time.deltaTime * offsetSmoothTime);
            }
            else
            {
                currentOffset = targetOffset;
            }
        }
    }
    private float AbsDifference()
    {
        return Mathf.Abs(currentOffset.magnitude - targetOffset.magnitude);
    }
    private float GetCurrentOffsetRadius()
    {
        if (TargetTransform == null) return maxOffsetRadius;

        // Рассчитываем расстояние до цели
        float distance = Vector3.Distance(transform.position, TargetTransform.position);

        // Нормализуем расстояние в диапазон [0,1]
        float normalizedDistance = Mathf.Clamp01((distance - minDistance) / (maxDistance - minDistance));

        // Получаем значение из кривой (кривая должна быть убывающей)
        float curveValue = distanceToOffsetCurve.Evaluate(normalizedDistance);

        // Применяем к максимальному радиусу
        return maxOffsetRadius * curveValue;
    }
    private float GetRandomOffsetInterval()
    {
        return Random.Range(offsetIntervalRange.x, offsetIntervalRange.y);
    }

}