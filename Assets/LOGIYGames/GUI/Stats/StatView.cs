using R3;
using System;
using TMPro;
using UnityEngine;

namespace LOGIYGames
{
    public sealed class StatView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI valueText;
        Stat stat;
        private IDisposable subscription;
        private void LateUpdate()
        {
            valueText.text = stat.Value.ToString("0");
        }
        public void Bind(StatPresenter presenter)
        {
            Unbind();
            stat = presenter.stat;
            nameText.text = presenter.DisplayName;

            subscription = presenter.DisplayValue.Subscribe(
                value => valueText.text = value);
        }

        public void Unbind()
        {
            subscription?.Dispose();
            subscription = null;
        }

        private void OnDestroy()
        {
            Unbind();
        }
    }
}