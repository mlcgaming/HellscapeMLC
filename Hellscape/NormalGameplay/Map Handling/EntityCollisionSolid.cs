using Microsoft.Xna.Framework;

namespace Hellscape
{
    public class EntityCollisionSolid
    {
        public Vector2 Position { get; protected set; }
        public Rectangle CollisionMask { get; protected set; }

        public EntityCollisionSolid(Vector2 position, int width, int height)
        {
            Position = position;
            CollisionMask = new Rectangle((int)Position.X, (int)Position.Y, width, height);
        }

        public EntityCollisionSolid Clone()
        {
            return new EntityCollisionSolid(Position, CollisionMask.Width, CollisionMask.Height);
        }
    }
}
