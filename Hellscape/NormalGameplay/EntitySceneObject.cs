using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hellscape.NormalGameplay
{
    public class EntitySceneObject
    {
        public string ShortName { get; protected set; }
        public string LongName { get; protected set; }
        public Texture2D Sprite { get; protected set; }
        public Vector2 Position { get; protected set; }
        public Rectangle CollisionMask { get; protected set; }

        public EntitySceneObject(string shortName, string longName, Texture2D sprite, Vector2 position)
        {
            ShortName = shortName;
            LongName = longName;
            Sprite = sprite;
            Position = position;
            CreateCollisionMask();
        }

        private void CreateCollisionMask()
        {
            CollisionMask = new Rectangle((int)Position.X, (int)Position.Y, Sprite.Width, Sprite.Height);
        }
    }
}
