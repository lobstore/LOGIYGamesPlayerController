using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    /// <summary>
    /// Validates MovementStateDriver setup on start
    /// </summary>
    public class MovementStateDriverValidator : MonoBehaviour
    {
        [SerializeField] private MovementStateDriver stateDriver;
        
        private void Start()
        {
            if (stateDriver == null)
            {
                stateDriver = GetComponent<MovementStateDriver>();
            }
            
            Validate();
        }
        
        private void Validate()
        {
            bool isValid = true;
            
            if (stateDriver.Character == null)
            {
                Debug.LogError($"[MovementStateDriverValidator] Character reference is NULL! Please assign it in the Inspector.", stateDriver);
                isValid = false;
            }
            
            if (stateDriver.Sensors == null)
            {
                Debug.LogError($"[MovementStateDriverValidator] Sensors reference is NULL! Please assign it in the Inspector.", stateDriver);
                isValid = false;
            }
            
            if (stateDriver.InputReader == null)
            {
                Debug.LogError($"[MovementStateDriverValidator] InputReader reference is NULL! Please assign it in the Inspector.", stateDriver);
                isValid = false;
            }
            
            if (isValid)
            {
                Debug.Log("[MovementStateDriverValidator] All references are valid!", stateDriver);
            }
            else
            {
                Debug.LogWarning("[MovementStateDriverValidator] MovementStateDriver is NOT properly configured. Character rotation will NOT work!", stateDriver);
            }
        }
    }
}
