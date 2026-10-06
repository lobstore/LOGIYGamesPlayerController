using System;

namespace LOGIYGames
{
    public class StatPresenter : IDisposable
    {
        StatView view;
        public StatPresenter(StatView view,Stat stat)
        {
            this.view = view;
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
