using R3;
using System;

namespace LOGIYGames
{
    public sealed class StatPresenter : IDisposable
    {
        public string DisplayName { get; }

        public ReactiveProperty<string> DisplayValue { get; } = new("");

        public Stat stat { get; private set; }
        private readonly StatView view;
        private readonly IDisposable subscription;

        private bool disposed;

        public StatPresenter(StatType type, Stat stat, StatView view)
        {
            this.stat = stat;
            this.view = view;

            // Позже можно заменить на локализованное название.
            DisplayName = type.ToString();

            Refresh();

            subscription = stat.OnModifiersChanged.Subscribe(
                _ => Refresh());

            view.Bind(this);
        }

        private void Refresh()
        {
            DisplayValue.Value = stat.Value.ToString("0.##");
        }

        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;

            subscription.Dispose();

            if (view != null)
                view.Unbind();

            DisplayValue.Dispose();
        }
    }
}