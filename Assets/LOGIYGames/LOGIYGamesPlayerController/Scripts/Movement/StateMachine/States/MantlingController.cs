using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;
using RealStep;
using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames
{
    public class MantlingController : MonoBehaviour
    {
        #region Hand IK Cache

        private Vector3 _leftHandLocalPoint;
        private Vector3 _rightHandLocalPoint;

        private Vector3 _leftHandNormal;
        private Vector3 _rightHandNormal;

        private Vector3 _ledgePoint;
        private Vector3 _ledgeNormal;

        #endregion

        private HandsIK mantleIKController;

        private List<MantlingStrategy> Mantlings = new();
        public List<MantlingFactory> MantlingFactories = new();
        private MantlingStrategy CurrentMantling;
        Character _character;
        private void Awake()
        {
            mantleIKController = GetComponent<HandsIK>();
            _character = GetComponent<Character>();
            foreach (var item in MantlingFactories)
            {
                Mantlings.Add(item.Create(_character));
            }
        }
        public void BeginMantling()
        {
            CurrentMantling.Enter();
            MantlingType mantlingType = CurrentMantling.MantlingType;

            EnableIK();
            DisableFootIK();

            _character.EventBus.Publish(new MantlingEvent
            {
                Type = mantlingType
            });

            _ledgePoint = CurrentMantling.TargetTopPoint.point;
            _ledgeNormal = CurrentMantling.TargetTopPoint.normal;

            CalculateHandTargets(_ledgePoint, _ledgeNormal);
            UpdateHandTargets();
        }
        public void Tick()
        {
            if (CurrentMantling == null) return;
            UpdateDynamicHandTargets();
            CurrentMantling.Tick();
        }
        private void UpdateHandTargets()
        {
            if (mantleIKController == null)
                return;

            mantleIKController.LeftHandPoint = _leftHandLocalPoint;
            mantleIKController.RightHandPoint = _rightHandLocalPoint;

            mantleIKController.LeftHandNormal = _leftHandNormal;
            mantleIKController.RightHandNormal = _rightHandNormal;
        }
        private void CalculateHandTargets(Vector3 ledgePoint, Vector3 normal)
        {
            if (mantleIKController == null)
                return;

            Vector3 rightOffset = _character.transform.right * 0.25f;
            Vector3 handOffset = Vector3.up * 0.05f;

            _leftHandLocalPoint = (ledgePoint - rightOffset + handOffset);
            _rightHandLocalPoint = (ledgePoint + rightOffset + handOffset);

            _leftHandNormal = normal;
            _rightHandNormal = normal;
        }
        private void UpdateDynamicHandTargets()
        {
            if (mantleIKController == null)
                return;

            _ledgePoint = CurrentMantling.MantleTargetPosition;

            Vector3 rightOffset = _character.transform.right * 0.25f;
            Vector3 handOffset = Vector3.up * 0.05f;

            Vector3 leftWorld =
                _ledgePoint - rightOffset + handOffset;

            Vector3 rightWorld =
                _ledgePoint + rightOffset + handOffset;

            mantleIKController.LeftHandPoint = leftWorld;
            mantleIKController.RightHandPoint = rightWorld;

            mantleIKController.LeftHandNormal = _ledgeNormal;
            mantleIKController.RightHandNormal = _ledgeNormal;
        }
        private void DisableHandIK()
        {
            var ik = _character.GetComponent<HandsIK>();
            if (ik != null) ik.DisableIK();
        }
        private void EnableIK()
        {
            if (mantleIKController != null &&
                (CurrentMantling.MantlingType == MantlingType.BracedLow ||
                 CurrentMantling.MantlingType == MantlingType.BracedHigh))
            {
                mantleIKController.EnableIK();
            }
        }
        private void DisableFootIK()
        {
            var footIK = _character.GetComponent<FootIK>();
            if (footIK != null) footIK.enabled = false;
        }

        private void EnableFootIK()
        {
            var footIK = _character.GetComponent<FootIK>();
            if (footIK != null) footIK.enabled = true;
        }
        public bool CanEnter()
        {
            foreach (var item in Mantlings)
            {
                if (item.CanEnter())
                {
                    CurrentMantling = item;
                    return true;
                }
            }

            return false;
        }
        public bool CanExit()
        {
            return CurrentMantling == null ? true : CurrentMantling.CanExit();
        }
        public void Cancel()
        {
            CurrentMantling.Exit();
            CurrentMantling = null;
            DisableHandIK();
            EnableFootIK();
        }
    }
}