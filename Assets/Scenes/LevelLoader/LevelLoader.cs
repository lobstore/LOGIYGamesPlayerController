using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace LOGIYGames
{
    public class LevelLoader : PersistentSingleton<LevelLoader>
    {
        public TextMeshProUGUI loadingPercentrage;
        public Image loadingProgressBar;
        private Animator m_Animator;

        AsyncOperation newSceneLoadingOperation = null;

        private void Start()
        {
            m_Animator = GetComponent<Animator>();
        }
        private void Update()
        {
            if (newSceneLoadingOperation == null) return;
            loadingPercentrage.text = (newSceneLoadingOperation.progress * 100f) + "%";
            loadingProgressBar.fillAmount = newSceneLoadingOperation.progress;
            if (newSceneLoadingOperation.isDone)
            {
                m_Animator.CrossFade("Crossfade_Opening", 0.1f);
                newSceneLoadingOperation = null;
            }
        }
        public void SwitchToScene(int index)
        {
            m_Animator.CrossFade("Crossfade_Closing", 0.1f);
            newSceneLoadingOperation = SceneManager.LoadSceneAsync(index);
            newSceneLoadingOperation.allowSceneActivation = false;
        }
        public void OnCloseSceneAnimationEnd()
        {
            if (newSceneLoadingOperation == null) return;
            newSceneLoadingOperation.allowSceneActivation = true;
        }
    }
}