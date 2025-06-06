using System.Collections.Generic;
using UnityEngine;
namespace LOGIYGames
{
    public class ResultPopup : PopupBaseMono
    {
        [SerializeField] private Transform viewTransform;
        [SerializeField] private GameObject vewPrefab;

        List<IResultModel> resultsModels;

        protected override void Start()
        {
            //TODO Implement Results Repository
            // resultModels = ResultsRepository.Instance.results;
            CreateResults();
            base.Start();
        }

        private void CreateResults()
        {
            foreach (var item in resultsModels)
            {
                // TODO Implement Presenter
                // var go = Instantiate(vewPrefab, viewTransform);
                // IResultPresenter resultPresenter = new (go.GetComponent<ResultView>(), item);
            }
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}