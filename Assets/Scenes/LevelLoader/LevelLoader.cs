using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace LOGIYGames
{
    public class LevelLoader : MonoBehaviour
    {

        public static LevelLoader Instance;
        public TextMeshProUGUI loadingPercentrage;
        public Image loadingProgressBar;
        private Animator m_Animator;

        AsyncOperation newSceneLoadingOperation;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                m_Animator = GetComponent<Animator>();

            }
            else
            {
                Destroy(gameObject);
            }

        }

        private void Update()
        {
            if (newSceneLoadingOperation == null) return;
            loadingPercentrage.text = (newSceneLoadingOperation.progress * 100f) + "%";
            loadingProgressBar.fillAmount = newSceneLoadingOperation.progress;
        }
        public void SwitchToScene(int index)
        {
            print("LoadingPre");

            m_Animator.SetTrigger("CloseScene");
            print("LoadingA");

            newSceneLoadingOperation = SceneManager.LoadSceneAsync(index);
            newSceneLoadingOperation.allowSceneActivation = false;
            print("LoadingB");
        }
        public void OnCloseSceneAnimationEnd()
        {
            if (newSceneLoadingOperation == null) return;
            newSceneLoadingOperation.allowSceneActivation = true;
        }
    }
}