using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

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
        public event EventHandler CancelPressed;

        public List<InventoryItem> Items { get; protected set; }
        public int MaxSlots { get; protected set; }
        public int CurrentSlot { get; protected set; }

        public bool Active { get; protected set; }
        public float SlotMoveTimer { get; protected set; }

        public ActorInventory()
        {
            MaxSlots = 6;
            CurrentSlot = 0;

            Active = false;

            Initialize();

            InputManager.UpPressed += OnUpPressed;
            InputManager.DownPressed += OnDownPressed;
            InputManager.VerticalReleased += OnVerticalReleased;
            InputManager.InventoryPressed += OnCancelPressed;
            InputManager.InteractPressed += OnInteractPressed;
            InputManager.JumpPressed += OnInteractPressed;
        }

        private void Initialize()
        {
            Items = new List<InventoryItem>();

            for(int i = 0; i < MaxSlots; i++)
            {
                InventoryItem newItem = new InventoryItem(Global.GetSceneObjectBYID("nullItem"), 1);
                Items.Add(newItem);
            }
        }

        public void Update()
        {
            InputManager.ProcessInputGamePad(PlayerIndex.One);


        }
        public void Draw()
        {


            foreach(InventoryItem ii in Items)
            {
                ii.Draw();
            }
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

        public void Activate(Camera2D camera)
        {
            Vector2 itemPosition = new Vector2(camera.BoundingRectangle.Center.X - 160, camera.BoundingRectangle.Center.Y - 108);

            for(int i = 0; i < MaxSlots; i++)
            {
                Items.ElementAt(i).SetupInventoryItem(new Vector2((int)itemPosition.X, itemPosition.Y + (36 * i)));
            }

            Active = true;
        }
        public void Deactivate()
        {
            Active = false;
        }

        protected virtual void OnDownPressed(object source, EventArgs args)
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
        protected virtual void OnUpPressed(object source, EventArgs args)
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
        protected virtual void OnVerticalReleased(object source, EventArgs args)
        {
            SlotMoveTimer = 0f;
        }
        protected virtual void OnCancelPressed(object source, EventArgs args)
        {
            CancelPressed?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void OnInteractPressed(object source, EventArgs args)
        {
            if(Items.ElementAt(CurrentSlot).Item.ShortName != "nullItem" && Active == true)
            {
                OnItemUsed(this, new ItemUsedEventArgs() { Item = Items.ElementAt(CurrentSlot).Item, Quantity = Items.ElementAt(CurrentSlot).Quantity });
            }
        }

        protected virtual void OnItemUsed(object source, ItemUsedEventArgs args)
        {
            ItemUsed?.Invoke(this, args);
        }
    }
}
