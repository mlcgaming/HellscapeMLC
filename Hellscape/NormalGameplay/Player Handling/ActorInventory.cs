using System;
using System.Collections.Generic;
using System.Linq;

namespace Hellscape
{
    public class ItemUsedEventArgs : EventArgs
    {
        public SceneObject Item { get; set; }
        public int Quantity { get; set; }
    }
    public class ActorInventory
    {
        public event EventHandler<ItemUsedEventArgs> ItemUsed;

        public List<InventoryItem> Items { get; protected set; }
        public int MaxSlots { get; protected set; }
        public int CurrentSlot { get; protected set; }

        public bool Active { get; protected set; }
        public float SlotMoveTimer { get; protected set; }

        public ActorInventory()
        {
            Items = new List<InventoryItem>();
            MaxSlots = 6;
            CurrentSlot = 0;

            Active = false;

            InputManager.InteractPressed += OnInteractPressed;
            InputManager.JumpPressed += OnInteractPressed;
        }

        public void SetMaxSlots(int qty)
        {
            MaxSlots = qty;
        }
        public bool AddItem(TileEntitySceneObject item)
        {
            // There's still room in the Inventory if the Max Slot is not filled
            foreach(InventoryItem ii in Items)
            {
                if(ii.Item == item.Object)
                {
                    if(ii.Quantity + item.Quantity <= item.Object.MaxQuantity)
                    {
                        ii.SetQuantity(ii.Quantity + item.Quantity);
                        return true;
                    }
                    else
                    {
                        if(Items.Count < MaxSlots)
                        {
                            int toFillQuantity = item.Object.MaxQuantity - ii.Quantity;
                            int remainder = item.Quantity - toFillQuantity;

                            ii.SetQuantity(item.Object.MaxQuantity);
                            Items.Add(new InventoryItem(item.Object, remainder));
                            return true;
                        }
                    }
                }
            }

            if (Items.Count < MaxSlots)
            {
                Items.Add(new InventoryItem(item.Object, item.Quantity));
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Activate()
        {
            Active = true;
        }
        public void Deactivate()
        {
            Active = false;
        }

        private void OnDownPressed(object obj, EventArgs args)
        {
            if(SlotMoveTimer <= 0)
            {
                if(CurrentSlot == MaxSlots)
                {
                    CurrentSlot = 0;
                }
                else
                {
                    CurrentSlot += 1;
                }

                SlotMoveTimer = 0.35f;
            }
        }
        private void OnUpPressed(object obj, EventArgs args)
        {
            if (SlotMoveTimer <= 0)
            {
                if (CurrentSlot == 0)
                {
                    CurrentSlot = MaxSlots;
                }
                else
                {
                    CurrentSlot -= 1;
                }

                SlotMoveTimer = 0.35f;
            }
        }
        private void OnInteractPressed(object obj, EventArgs args)
        {
            if(Items.Count != 0 && Active == true)
            {
                OnItemUsed(this, new ItemUsedEventArgs() { Item = Items.ElementAt(CurrentSlot).Item, Quantity = Items.ElementAt(CurrentSlot).Quantity });
            }
        }

        protected virtual void OnItemUsed(object obj, ItemUsedEventArgs args)
        {
            ItemUsed?.Invoke(this, args);
        }
    }
}
