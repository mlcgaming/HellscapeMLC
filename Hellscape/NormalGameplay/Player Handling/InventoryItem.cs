namespace Hellscape
{
    public class InventoryItem
    {
        public SceneObject Item { get; protected set; }
        public int Quantity { get; set; }
        private AnimationManager AnimationManager { get; set; }

        public InventoryItem(SceneObject item, int qty)
        {
            Item = item;
            Quantity = qty;

            AnimationManager = new AnimationManager();
        }

        public void SetQuantity(int newQty)
        {
            Quantity = newQty;
        }
    }
}
