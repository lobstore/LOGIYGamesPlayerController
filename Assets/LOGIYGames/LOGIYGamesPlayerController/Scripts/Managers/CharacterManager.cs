using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames.CharacterCore
{
    public interface IControllable
    {
        Transform CinemachineCameraLookAtTransform { get; set; }
        Transform CinemachineCameraFollowTransform { get; set; }
        void EnableControl();
        void DisableControl();
        void OnControlGained();
        void OnControlLost();
        void HandleInputs();
    }

    public class CharacterManager : Singleton<CharacterManager>
    {
        [SerializeField] List<Character> characters;
        public IControllable CurrentControllable { get; private set; }
        int index = 0;
        private IEnumerator Start()
        {
            yield return null;
            SetCharacterControl(characters[index]);
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetCharacterControl(characters[index++ %characters.Count]);
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetCharacterControl(characters[index-- % characters.Count]);
            }
        }
        private void FixedUpdate()
        {
            CurrentControllable?.HandleInputs();
        }
        public void SetCharacterControl(IControllable ccontrollable)
        {
            if (CurrentControllable == ccontrollable)
            {
                return;
            }
            print("SetCharacterControl");
            CurrentControllable?.DisableControl();
            CurrentControllable?.OnControlLost();
            CurrentControllable = ccontrollable;

            CameraManager.Instance.SetTargetTo(
            CurrentControllable.CinemachineCameraFollowTransform,
            CurrentControllable.CinemachineCameraLookAtTransform
            );

            CurrentControllable.EnableControl();
            CurrentControllable.OnControlGained();
        }

    }
}