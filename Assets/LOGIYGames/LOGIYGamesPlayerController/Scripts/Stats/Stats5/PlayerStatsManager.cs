using System;
using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames
{

namespace LOGIYGames
    {
        public class PlayerStatsManager : PersistentSingleton<PlayerStatsManager>
        {
            [SerializeField] private GameObject statPrefab;
            [SerializeField] private RectTransform statsContainer;

            private readonly List<StatPresenter> statsPresenters = new();
            private readonly List<StatView> statsViews = new();
            override protected void Awake()
            {
                base.Awake();
                PlayerManager.Instance.OnCharacterChanged.AddListener((newChar) =>
                {

                    UpdateStatsViews(newChar.Stats);

                });
            }
            public void UpdateStatsViews(StatsController controller)
            {
                ClearStatsViews();

                if (controller == null)
                    return;

                foreach (var entry in controller.Stats)
                {
                    var obj = Instantiate(
                        statPrefab,
                        statsContainer,
                        false);

                    var view = obj.GetComponent<StatView>();

                    if (view == null)
                    {
                        Debug.LogError(
                            "На prefab характеристики отсутствует PlayerStatView.",
                            obj);

                        Destroy(obj);
                        continue;
                    }

                    statsViews.Add(view);

                    statsPresenters.Add(new StatPresenter(
                        entry.Key,
                        entry.Value,
                        view));
                }
            }

            private void ClearStatsViews()
            {
                foreach (var presenter in statsPresenters)
                    presenter.Dispose();

                statsPresenters.Clear();

                foreach (var view in statsViews)
                {
                    if (view == null)
                        continue;

                    // Destroy отложен до конца кадра:
                    // скрываем старые строки сразу.
                    view.gameObject.SetActive(false);
                    Destroy(view.gameObject);
                }

                statsViews.Clear();
            }

            private void OnDestroy()
            {
                ClearStatsViews();
            }
        }
    }
}