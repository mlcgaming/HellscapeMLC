using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace Hellscape
{
    public class InventoryItem
    {
        public SceneObject Item { get; protected set; }
        public int Quantity { get; set; }
        public AnimationManager AnimationManager { get; protected set; }
        private Rectangle IconRectangle, LongNameRectangle, QuantityRectangle;
        private Vector2 LongNamePosition, QuantityPosition;

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

        public void Select()
        {
            AnimationManager.Play(Item.Animation);
        }
        public void Deselect()
        {
            AnimationManager.Stop();
        }

        public void SetupInventoryItem(Vector2 position)
        {
            SetRectanglePositions(position);
            SetTextPositions();
        }
        private void SetRectanglePositions(Vector2 position)
        {
            Vector2 iconPos, longPos, qtyPos;

            iconPos = position;
            IconRectangle = new Rectangle((int)iconPos.X, (int)iconPos.Y, 32, 32);

            longPos = new Vector2(IconRectangle.Right, position.Y);
            LongNameRectangle = new Rectangle((int)longPos.X, (int)longPos.Y, 128, 32);

            qtyPos = new Vector2(LongNameRectangle.Right, position.Y);
            QuantityRectangle = new Rectangle((int)qtyPos.X, (int)qtyPos.Y, 32, 32);
        }
        private void SetTextPositions()
        {
            LongNamePosition = Common.AdjustText(new Vector2(LongNameRectangle.Center.X, LongNameRectangle.Center.Y), Global.DebugFont, Item.LongName, Common.TextHalign.Center, Common.TextValign.Middle);
            QuantityPosition = Common.AdjustText(new Vector2(QuantityRectangle.Center.X, QuantityRectangle.Center.Y), Global.DebugFont, "x" + Quantity.ToString(), Common.TextHalign.Center, Common.TextValign.Middle);
        }

        public void Draw()
        {
            Global.SpriteBatch.Draw(Global.DebugTexture, IconRectangle, Color.DarkGray);
            AnimationManager.Draw(IconRectangle, SpriteEffects.None);

            Global.SpriteBatch.DrawString(Global.DebugFont, Item.LongName, LongNamePosition, Color.White);

            Global.SpriteBatch.DrawString(Global.DebugFont, "x" + Quantity.ToString(), QuantityPosition, Color.White);
        }
    }
}
