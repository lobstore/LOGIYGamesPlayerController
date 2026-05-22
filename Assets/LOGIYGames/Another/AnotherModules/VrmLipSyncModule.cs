using System.Collections.Generic;
using uLipSync;
using UnityEngine;
using UniVRM10;
namespace LOGIYGames
{
    public class VrmLipSyncModule : MonoModuleBase
    {

        [System.Serializable]
        public class BlendShapeInfo
        {
            public string phoneme;
            public ExpressionKey key;
            public float maxWeight = 1f;

            public float weight { get; set; } = 0f;

            public float weightVelocity { get; set; } = 0f;
        }
        public virtual float maxWeight => 1f;
        public virtual float minWeight => 0f;
        public UpdateMethod updateMethod = UpdateMethod.LateUpdate;
        public List<BlendShapeInfo> blendShapes = new List<BlendShapeInfo>() {
        new BlendShapeInfo{phoneme = "A" , key = ExpressionKey.Aa},
        new BlendShapeInfo{phoneme = "I" , key = ExpressionKey.Ih},
        new BlendShapeInfo{phoneme = "U" , key = ExpressionKey.Ou},
        new BlendShapeInfo{phoneme = "E" , key = ExpressionKey.Ee},
        new BlendShapeInfo{phoneme = "O" , key = ExpressionKey.Oh},
        new BlendShapeInfo{phoneme = "-" , key = ExpressionKey.Neutral},

    };
        public float maxBlendShapeValue = 1f;
        public float minVolume = -2.5f;
        public float maxVolume = -1.5f;
        [Range(0f, 0.3f)] public float smoothness = 0.05f;
        public bool usePhonemeBlend = false;

        LipSyncInfo _info = new LipSyncInfo();
        bool _lipSyncUpdated = false;
        float _volume = 0f;
        float _openCloseVelocity = 0f;
   
        protected float volume => _volume;
        [SerializeField] Vrm10Instance Vrm10Instance;
        private Vrm10RuntimeExpression _expressions;

        private void Awake()
        {
            _expressions = Vrm10Instance.Runtime.Expression;
        }

        void UpdateLipSync()
        {
            UpdateVolume();
            UpdateVowels();
            _lipSyncUpdated = false;
        }
        public void OnLipSyncUpdate(LipSyncInfo info)
        {
            _info = info;
            _lipSyncUpdated = true;
            if (updateMethod == UpdateMethod.LipSyncUpdateEvent)
            {
                UpdateLipSync();
                OnApplyBlendShapes();
            }
        }

        public override void OnUpdate(float deltaTime)
        {
            if (updateMethod != UpdateMethod.LipSyncUpdateEvent)
            {
                UpdateLipSync();
            }

            if (updateMethod == UpdateMethod.Update)
            {
                OnApplyBlendShapes();
            }
        }
        public override void OnLateUpdate(float deltaTime)
        {
            if (updateMethod == UpdateMethod.LateUpdate)
            {
                OnApplyBlendShapes();
            }
        }

        public override void OnFixedUpdate(float fixedDeltaTime)
        {
            if (updateMethod == UpdateMethod.FixedUpdate)
            {
                OnApplyBlendShapes();
            }
        }
        float SmoothDamp(float value, float target, ref float velocity)
        {
            return Mathf.SmoothDamp(value, target, ref velocity, smoothness);
        }

        void UpdateVolume()
        {
            float normVol = 0f;
            if (_lipSyncUpdated && _info.rawVolume > 0f)
            {
                normVol = Mathf.Log10(_info.rawVolume);
                normVol = (normVol - minVolume) / Mathf.Max(maxVolume - minVolume, 1e-4f);
                normVol = Mathf.Clamp(normVol, 0f, 1f);
            }
            _volume = SmoothDamp(_volume, normVol, ref _openCloseVelocity);
        }

        void UpdateVowels()
        {
            float sum = 0f;
            var ratios = _info.phonemeRatios;

            foreach (var bs in blendShapes)
            {
                float targetWeight = 0f;
                if (usePhonemeBlend)
                {
                    if (ratios != null && !string.IsNullOrEmpty(bs.phoneme))
                    {
                        ratios.TryGetValue(bs.phoneme, out targetWeight);
                    }
                }
                else
                {
                    targetWeight = (bs.phoneme == _info.phoneme) ? 1f : 0f;
                }
                float weightVel = bs.weightVelocity;
                bs.weight = SmoothDamp(bs.weight, targetWeight, ref weightVel);
                bs.weightVelocity = weightVel;
                sum += bs.weight;
            }

            foreach (var bs in blendShapes)
            {
                bs.weight = sum > 0f ? bs.weight / sum : 0f;
            }
        }

        public void ApplyBlendShapes()
        {
            if (updateMethod == UpdateMethod.External)
            {
                OnApplyBlendShapes();
            }
        }

        protected virtual void OnApplyBlendShapes()
        {
            if (_expressions == null) return;

            foreach (var bs in blendShapes)
            {
                if (bs.key.Equals(ExpressionKey.Neutral)) continue;
                _expressions.SetWeight(bs.key, 0f);
                ;
            }

            foreach (var bs in blendShapes)
            {
                if (bs.key.Equals(ExpressionKey.Neutral)) continue;
                float weight = _expressions.GetWeight(bs.key);
                weight += bs.weight * bs.maxWeight * volume * maxBlendShapeValue;
                _expressions.SetWeight(bs.key, weight);
            }
        }
        public BlendShapeInfo GetBlendShapeInfo(string phoneme)
        {
            foreach (var info in blendShapes)
            {
                if (info.phoneme == phoneme) return info;
            }
            return null;
        }
    }
}