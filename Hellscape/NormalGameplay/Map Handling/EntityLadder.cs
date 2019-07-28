using Microsoft.Xna.Framework;

namespace Hellscape
{
    public class EntityLadder
    {
        public Vector2 Position { get; }
        public Rectangle CollisionMask { get; private set; }

        public EntityLadder(Vector2 position, int width, int height)
        {
            Position = position;
            CollisionMask = Common.CreateCollisionMask(Position, width, height);
        }
    }
}
