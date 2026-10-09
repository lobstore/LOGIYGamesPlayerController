using UnityEngine;

namespace LOGIYGames
{
    public class ItemSlotView : MonoBehaviour
    {
        [SerializeField] ItemView ItemView;
        ItemSlotPresenter presenter;
        ItemSlot ItemSlot;
        public void Bind(ItemSlotPresenter presenter)
        {
            this.presenter = presenter;
            ItemSlot = presenter.Slot;
            UpdateView();
            presenter.Slot.OnItemChanged.AddListener(UpdateView);
        }
        private void OnDestroy()
        {
            Unbind();
        }
        public void Unbind()
        {
            presenter.Slot.OnItemChanged.RemoveListener(UpdateView);
        }
        private void UpdateView()
        {
            ItemView.SetItem(ItemSlot.Item);
        }
    }
}
