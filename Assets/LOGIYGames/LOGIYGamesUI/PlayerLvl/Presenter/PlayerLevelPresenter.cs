namespace LOGIYGames
{
    public class PlayerLevelPresenter
    {
        PlayerLevelView _playerLevelView;
        PlayerLevelModel _playerLevelModel;
        public PlayerLevelPresenter(PlayerLevelModel model, PlayerLevelView view)
        {
            _playerLevelModel = model;
            _playerLevelView = view;
            _playerLevelModel?.OnXPChanged.AddListener(UpdateView);
            view.Initialize(_playerLevelModel);
        }

        private void UpdateView()
        {
            _playerLevelView.UpdateView(_playerLevelModel);
        }
    }
}