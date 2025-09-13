using UnityEngine;
using UniVRM10;
namespace LOGIYGames
{
    public class Vrm10LookAtModule : MonoModuleBase
    {
        [Header("LookAt Settings")]
        [SerializeField] private Vrm10Instance _vrm10;
        private Vrm10Runtime Vrm10Runtime;

        [HideInInspector] public Transform TargetTransform; // Объект, за которым следим (если null - смотрит вперёд)

        [Header("Random Offset Settings")]
        [SerializeField] private float maxOffsetRadius = 0.1f; // Максимальный радиус случайного смещения
        [SerializeField] private Vector2 offsetIntervalRange = new Vector2(2f, 5f); // Интервал между смещениями (мин, макс)
        [SerializeField] private AnimationCurve distanceToOffsetCurve; // Кривая для зависимости радиуса от расстояния

        [Header("Distance Settings")]
        [SerializeField] private float minDistance = 0.1f; // Минимальное расстояние для расчета
        [SerializeField] private float maxDistance = 5f; // Максимальное расстояние для расчета

        [SerializeField] private Transform EyeSight;
        [SerializeField] private Transform HeadTarget;
        [SerializeField] private Transform DefaultPosition;
        [SerializeField] private AnimationCurve sightAnimationCurve;
        [SerializeField] private AnimationCurve headAnimationCurve;
        [SerializeField] private float sightTurningDuration;

        private StopwatchTimer turningDurationTimer;
        private CountdownTimer offsetTimer;
        private Vector3? LookTargetPosition { get => Vrm10Runtime.LookAt.LookAtInput.WorldPosition; set => Vrm10Runtime.LookAt.LookAtInput = new LookAtInput { WorldPosition = value }; }
        private Vector3 currentOffset;
        private Vector3 targetOffset;
        private Transform prevTransform = null;
        private float threshold = 0.01f;
        private float offsetSmoothTime = 20f;

        private void Awake()
        {
            turningDurationTimer = new StopwatchTimer();
            offsetTimer = new CountdownTimer(GetRandomOffsetInterval());
            Vrm10Runtime = _vrm10.Runtime;
        }

        private void Start()
        {
            EyeSight.position = DefaultPosition.position;
            HeadTarget.position = DefaultPosition.position;
            LookTargetPosition = EyeSight.position;
            prevTransform = EyeSight;
            turningDurationTimer.Start();
            offsetTimer.Start();
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

        private void TimerUpdate()
        {
            turningDurationTimer.Tick(Time.deltaTime);
            offsetTimer.Tick(Time.deltaTime);
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

        private float GetRandomOffsetInterval()
        {
            return Random.Range(offsetIntervalRange.x, offsetIntervalRange.y);
        }


        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            TimerUpdate();
        }

        public override void OnLateUpdate(float deltaTime)
        {
            base.OnLateUpdate(deltaTime);
            if (prevTransform != TargetTransform)
            {
                ResetTimer();
                prevTransform = TargetTransform;
            }

            ProcessNystagmus();
            Tracking();
        }

        private void Tracking()
        {
            // Применяем текущее смещение к позиции взгляда
            Vector3 finalLookPosition = EyeSight.position + currentOffset;
            LookTargetPosition = finalLookPosition;

            if (TargetTransform != null)
            {
                if (EyeSight.position != TargetTransform.position || HeadTarget.position != TargetTransform.position)
                {
                    PerformTurningSight(TargetTransform.position);
                }
                else
                {
                    ResetTimer();
                }
            }
            else
            {
                if (EyeSight.position != DefaultPosition.position || HeadTarget.position != DefaultPosition.position)
                {
                    PerformTurningSight(DefaultPosition.position);
                }
                else
                {
                    ResetTimer();
                }
            }
        }

        private void ResetTimer()
        {
            turningDurationTimer.Reset();
        }

        void PerformTurningSight(Vector3 target)
        {
            var sightProgress = Mathf.Clamp01(turningDurationTimer.GetTime() / sightTurningDuration);
            float curveValue = sightAnimationCurve.Evaluate(sightProgress);
            EyeSight.position = Vector3.Lerp(EyeSight.position, target, curveValue);

            curveValue = headAnimationCurve.Evaluate(sightProgress);
            HeadTarget.position = Vector3.Lerp(HeadTarget.position, target, curveValue);
        }
    }
}