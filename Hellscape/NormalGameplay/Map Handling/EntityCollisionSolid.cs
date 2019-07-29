using Microsoft.Xna.Framework;

namespace Hellscape
{
    public class EntityCollisionSolid
    {
        public Vector2 Position { get; protected set; }
        public Rectangle CollisionMask { get; protected set; }
        public float Tilt { get; private set; }
        public int YIntercept { get; private set; }
        public bool IsAngled { get; private set; }

        public EntityCollisionSolid(Vector2 position, int width, int height)
        {
            Position = position;
            CollisionMask = new Rectangle((int)Position.X, (int)Position.Y, width, height);
            Tilt = 0f;
            YIntercept = 0;
            IsAngled = false;
        }
        public EntityCollisionSolid(Vector2 position, int width, int height, float tilt)
        {
            Position = position;
            CollisionMask = new Rectangle((int)Position.X, (int)Position.Y, width, height);
            Tilt = tilt;

            if(Tilt < 0)
            {
                Point yInterceptCalcPoint = new Point((int)Position.X, (int)(Position.Y + CollisionMask.Height));
                YIntercept = Common.GetCollisionAngleIntersect(yInterceptCalcPoint, Tilt);
            }
            else
            {
                Point yInterceptCalcPoint = new Point((int)Position.X, (int)Position.Y);
                YIntercept = Common.GetCollisionAngleIntersect(yInterceptCalcPoint, Tilt);
            }

            IsAngled = true;
        }

        public EntityCollisionSolid Clone()
        {
            return new EntityCollisionSolid(Position, CollisionMask.Width, CollisionMask.Height);
        }
    }
}
