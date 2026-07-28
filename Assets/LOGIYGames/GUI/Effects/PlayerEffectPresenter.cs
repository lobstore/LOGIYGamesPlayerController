using R3;
using System;
using UnityEngine;

namespace LOGIYGames
{
    public class PlayerEffectPresenter : IDisposable
    {
        public ReactiveProperty<string> DisplayValue = new ReactiveProperty<string>();
        public ReactiveProperty<Sprite> Icon = new ReactiveProperty<Sprite>();
        DisposableBag DisposableBag;
        PlayerEffectView view;
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
