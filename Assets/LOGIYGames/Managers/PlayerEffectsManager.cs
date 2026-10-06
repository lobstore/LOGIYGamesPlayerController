using R3;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace LOGIYGames
{
    public class PlayerEffectsManager : PersistentSingleton<PlayerEffectsManager>
    {
        private List<PlayerEffectPresenter> effectsPresenters = new();
        private List<PlayerEffectView> effectsViews = new();

        [SerializeField] private GameObject effectIconPrefab;
        [SerializeField] private RectTransform effectsContainer;

        IDisposable subscription;
        override protected void Awake()
        {
            base.Awake();
            PlayerManager.Instance.OnCharacterChanged.AddListener((newChar) =>
             {
                 subscription?.Dispose();
                 subscription = newChar.Effects.OnContinuousEffectsChanged.Subscribe((effects) =>
                 {
                     UpdateEffectsViews(effects);
                 });
                 UpdateEffectsViews(newChar.Effects.Effects);

             });
        }
        private void UpdateEffectsViews(IReadOnlyList<RuntimeEffect> effects)
        {

            for (int i = 0; i < effectsContainer.childCount; i++)
            {
                Destroy(effectsContainer.GetChild(i).gameObject);
            }
            if (effectsPresenters.Count > 0)
            {
                effectsPresenters.Clear();
            }
            if (effectsViews.Count > 0)
            {
                foreach (var item in effectsViews)
                {
                    Destroy(item);
                }
                effectsViews.Clear();
            }

            if (effects.Count > 0)
            {
                foreach (var item in effects)
                {
                    if (item.Data.Icon == null) return;
                    var obj = Instantiate(effectIconPrefab);
                    obj.transform.SetParent(effectsContainer, true);
                    obj.transform.localScale = Vector3.one;
                    var view = obj.GetComponent<PlayerEffectView>();
                    effectsViews.Add(view);
                    effectsPresenters.Add(new PlayerEffectPresenter(item, view));
                }
            }
        }
    }
}
