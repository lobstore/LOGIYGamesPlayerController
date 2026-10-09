namespace LOGIYGames
{
    public class ItemSlotPresenter
    {
        public ItemSlotView View { get; private set; }
        public ItemSlot Slot { get; private set; }
        public Inventory InventoryPresenter { get; private set; }
        public ItemSlotPresenter(ItemSlotView view, ItemSlot model)
        {
            Slot = model;
            View = view;
            View.Bind(this);
        }
        public void Bind(Inventory inventory)
        {
            InventoryPresenter = inventory;
        }
    }
}
