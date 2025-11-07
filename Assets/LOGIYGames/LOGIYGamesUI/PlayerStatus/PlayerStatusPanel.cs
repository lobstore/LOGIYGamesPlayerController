using UnityEngine;

namespace LOGIYGames
{
    public class PlayerStatusPanel : MonoBehaviour
    {
        StatusPresenter statusPresenter;
        [SerializeField] StatusView statusView;
        private void Start()
        {
            statusPresenter = new StatusPresenter(statusView);
        }
    }
}
