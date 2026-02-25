using LOGIYGames.Timers;
using Unity.Netcode;
using UnityEngine;
using UniVRM10;
namespace LOGIYGames
{
    public class EyeBlinkerModule : MonoModuleBase
    {
        [Header("Required References")]
        [SerializeField] private Vrm10Instance _vrm10;
        public SkinnedMeshRenderer skinnedMeshRenderer;
        public string blinkBlendshapeName = "Fcl_EYE_Close";

        private Vrm10RuntimeExpression _expressions;
        public ExpressionKey blinkExpression = ExpressionKey.Blink;

        private float BlinkValue { get => _expressions.GetWeight(blinkExpression); set => _expressions.SetWeight(blinkExpression, value); }

        [Header("Blink Settings")]
        [Range(2f, 10f)] public float minBlinkInterval = 3f;
        [Range(2f, 10f)] public float maxBlinkInterval = 6f;
        [Range(0.1f, 1f)] public float blinkDuration = 0.4f;



        [Header("Animation Curve")]
        public AnimationCurve blinkCurve = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.3f, 1f),
            new Keyframe(0.7f, 1f),
            new Keyframe(1f, 0f)
        );

        private int _blendshapeIndex = -1;
        private CountdownTimer _cooldonwTimer;
        private CountdownTimer _blinkTimer;

        private float _nextBlinkTime;

        private void Awake()
        {
            _blinkTimer = new(blinkDuration);
            _cooldonwTimer = new(_nextBlinkTime = Random.Range(minBlinkInterval, maxBlinkInterval));
            _expressions = _vrm10.Runtime.Expression;
            if (skinnedMeshRenderer == null)
            {
                Debug.LogError("SkinnedMeshRenderer is not assigned!", this);
                enabled = false;
                return;
            }

            _blendshapeIndex = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(blinkBlendshapeName);

            if (_blendshapeIndex == -1)
            {
                Debug.LogError($"Blendshape '{blinkBlendshapeName}' not found on mesh!", this);
                enabled = false;
                return;
            }
        }
        
        private void Start()
        {


            _cooldonwTimer.OnTimerStop = () => { _blinkTimer.Reset(); _blinkTimer.Start(); };
            _blinkTimer.OnTimerStop = () => {_nextBlinkTime = Random.Range(minBlinkInterval, maxBlinkInterval); _cooldonwTimer.Reset(_nextBlinkTime); _cooldonwTimer.Start(); };
            _blinkTimer.Start();

        }

        private void OnEnable()
        {
            BlinkValue = 0;
            _blinkTimer.Start();
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            PerformBlink();
        }
        private void PerformBlink()
        {
            BlinkValue = blinkCurve.Evaluate(_blinkTimer.Progress);
        }




        private void OnValidate()
        {
            if (minBlinkInterval > maxBlinkInterval)
            {
                maxBlinkInterval = minBlinkInterval + 1f;
            }
        }
    }
}
