using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseWindowPresenter : MonoBehaviour
{
    [Header("Views")]

    [SerializeField] private TextMeshProUGUI closeText;
    [SerializeField] private Button closeButton;
    

    private void OnCloseButtonClicked() {
        SetActive(false);
    }

    public void SetActive(bool isActive)
    {
        GetComponent<Canvas>().enabled=isActive;
    }

    public void Initialize()
    {
        SetActive(false);
        closeButton.onClick.AddListener(OnCloseButtonClicked);
    }
}
