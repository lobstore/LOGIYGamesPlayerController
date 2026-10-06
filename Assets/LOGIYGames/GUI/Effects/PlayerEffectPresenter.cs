using R3;
using System;
using UnityEngine;

namespace LOGIYGames
{
    [Serializable]
    public class PlayerEffectPresenter : IDisposable
    {
        public SerializableReactiveProperty<string> DisplayValue = new ();
        public SerializableReactiveProperty<Sprite> Icon = new ();
        DisposableBag DisposableBag;
        [SerializeField] PlayerEffectView view;
        public PlayerEffectPresenter(RuntimeEffect effect, PlayerEffectView view)
        {
            this.view = view;
            DisposableBag.Add(effect.DisplayValue.Subscribe((value) =>
            {
                DisplayValue.Value = value;

            }));
            Icon.Value = effect.Data.Icon;
            view.Bind(this);
        }

        public void Dispose()
        {
            DisposableBag.Dispose();
            view.Unbind();
        }
    }
}
