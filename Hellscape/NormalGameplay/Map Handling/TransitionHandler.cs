using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Hellscape
{
    public class TransitionHandler
    {
        public string TransitionMapID { get; protected set; }
        public Vector2 Position { get; protected set; }
        public Vector2 TransitionPosition { get; protected set; }
        public Rectangle CollisionMask { get; protected set; }

        public TransitionHandler(string mapID, float posX, float posY, float transitionX, float transitionY, float width, float height)
        {
            TransitionMapID = mapID;
            Position = new Vector2(posX, posY);
            TransitionPosition = new Vector2(transitionX, transitionY);
            CollisionMask = new Rectangle((int)Position.X, (int)Position.Y, (int)width, (int)height);
        }
    }
}
