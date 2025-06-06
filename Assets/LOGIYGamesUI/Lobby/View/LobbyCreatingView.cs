using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace LOGIYGames
{
    public class LobbyCreatingView : MonoBehaviour
    {
        [SerializeField] public TMP_InputField LobbyNameInputField;
        [SerializeField] public Slider LobbyMembersMaxCount;
        [SerializeField] public Toggle PrivacyToggle;
        [SerializeField] public TMP_InputField LobbyCodeInputField;
        [SerializeField] public Button ApplyingButton;
    }
}