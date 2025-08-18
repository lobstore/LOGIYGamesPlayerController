using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace LOGIYGames
{
    public interface IControllable
    {
        Transform CinemachineCameraLookAtTransform { get; set; }
        Transform CinemachineCameraFollowTransform { get; set; }
        void EnableControl();
        void DisableControl();
        void OnControlGained();
        void OnControlLost();
    }

    public class CharacterManager : Singleton<CharacterManager>
    {
        [SerializeField] List<CharacterModule> characters;
        public IControllable CurrentControllable { get; private set; }
        protected override void Awake()
        {
            base.Awake();
        }
        private IEnumerator Start()
        {
            yield return null;
            SetCharacterControl(characters[0]);
        }
        public void SetCharacterControl(IControllable ccontrollable)
        {
            if (CurrentControllable == ccontrollable)
            {
                return;
            }

            CurrentControllable?.DisableControl();
            CurrentControllable?.OnControlLost();
            CurrentControllable = ccontrollable;

            CurrentControllable.EnableControl();
            CurrentControllable.OnControlGained();
        }
    }
}