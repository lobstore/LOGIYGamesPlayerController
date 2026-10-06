using LOGIYGames.CharacterCore;
using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames
{
    public class PlayerAbilitiesManager : PersistentSingleton<PlayerAbilitiesManager>
    {
        [SerializeField] private GameObject abilityIconPrefab;
        [SerializeField] private RectTransform abilitiesContainer;

        private List<PlayerAbilityPresenter> abilityPresenters = new();
        private List<PlayerAbilityView> abilitiesViews = new();
        override protected void Awake()
        {
            base.Awake();
            PlayerManager.Instance.OnCharacterChanged.AddListener(UpdateAbilitiesViews);
        }
        public void UpdateAbilitiesViews(Actor newChar)
        {
            for (int i = 0; i < abilitiesContainer.childCount; i++)
            {
                Destroy(abilitiesContainer.GetChild(i).gameObject);
            }
            if (abilityPresenters.Count > 0)
            {
                abilityPresenters.Clear();
            }
            if (abilitiesViews.Count > 0)
            {
                foreach (var item in abilitiesViews)
                {
                    Destroy(item);
                }
                abilitiesViews.Clear();
            }

            if (newChar.GetComponent<AbilitiesController>().Abilities.Count > 0)
            {
                foreach (var item in newChar.GetComponent<AbilitiesController>().Abilities)
                {
                    var obj = Instantiate(abilityIconPrefab);
                    obj.transform.SetParent(abilitiesContainer, true);
                    obj.transform.localScale = Vector3.one;
                    var view = obj.GetComponent<PlayerAbilityView>();
                    abilitiesViews.Add(view);
                    abilityPresenters.Add(new PlayerAbilityPresenter(item, view));
                }
            }
        }
    }
}
