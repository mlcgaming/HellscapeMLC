using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Hellscape
{
    public class ItemSelectedEventArgs
    {
        public string ItemName;
    }
    public class MainMenuItem
    {
        public EventHandler<ItemSelectedEventArgs> ItemSelected;

        private string Text;
        public bool IsSelected { get;  protected set; }
        private Vector2 Position;
        private SpriteFont Font;

        public MainMenuItem(string text, Vector2 textCenterPosition)
        {
            Text = text;
            IsSelected = false;
            Position = textCenterPosition;
            Font = Global.Content.Load<SpriteFont>("GFX/Fonts/MainMenuItemFont");

            Common.AdjustText(Position, Font, Text, Common.TextHalign.Center, Common.TextValign.Middle);

            InputManager.JumpPressed += OnConfirmPressed;
            InputManager.StartPressed += OnConfirmPressed;
        }

        public void Draw()
        {
            if(IsSelected == true)
            {
                Global.SpriteBatch.DrawString(Font, Text, Position, Color.Yellow);
            }
            else
            {
                Global.SpriteBatch.DrawString(Font, Text, Position, Color.White);
            }
        }

        public void Select()
        {
            IsSelected = true;
        }
        public void Deselect()
        {
            IsSelected = false;
        }

        private void OnConfirmPressed(object source, EventArgs args)
        {
            if(IsSelected == true)
            {
                ItemSelected?.Invoke(this, new ItemSelectedEventArgs() { ItemName = Text });
            }
        }
    }
}
