using R3;
using System;
using UnityEngine;

namespace LOGIYGames
{
    public class PlayerSkillPresenter : IDisposable
    {
        public ReactiveProperty<float> Cooldown = new ReactiveProperty<float>();
        public ReactiveProperty<Sprite> Icon = new ReactiveProperty<Sprite>();
        DisposableBag DisposableBag;
        private Ability Ability;
        private PlayerAbilityView view;
        public PlayerSkillPresenter(Ability ability, PlayerAbilityView view)
        {
            Ability = ability;
            this.view = view;
            DisposableBag.Add(ability.CooldownTimer.CurrentTime.Subscribe((_) =>
            {
                Cooldown.Value = ability.CooldownTimer.Progress;

            }));
            Icon.Value = ability.Data.icon;
            view.Bind(this);
        }

        public void Dispose()
        {
            DisposableBag.Dispose();
            view.Unbind();
        }
    }
}
