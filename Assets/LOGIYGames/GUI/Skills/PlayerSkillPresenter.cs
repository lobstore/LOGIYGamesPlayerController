using R3;
using System;
using UnityEngine;

namespace LOGIYGames
{
    public class PlayerSkillPresenter : IDisposable
    {
        public ReactiveProperty<float> CooldownProgress = new ReactiveProperty<float>();
        public ReactiveProperty<float> CooldownTime = new ReactiveProperty<float>();
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
                CooldownProgress.Value = 1- ability.CooldownTimer.Progress;

            }));
            DisposableBag.Add(ability.CooldownTimer.CurrentTime.Subscribe((time) =>
            {
                CooldownTime.Value = time;

            }));
            Icon.Value = ability.Data.Icon;
            view.Bind(this);
        }

        public void Dispose()
        {
            DisposableBag.Dispose();
            view.Unbind();
        }
    }
}
