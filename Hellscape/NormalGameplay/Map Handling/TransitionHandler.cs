using Microsoft.Xna.Framework;

namespace Hellscape
{
    public class TransitionHandler
    {
        public string TransitionMapID { get; protected set; }
        public Vector2 Position { get; protected set; }
        public Vector2 TransitionPosition { get; protected set; }
        public Rectangle CollisionMask { get; protected set; }
        public bool IsKeyLocked { get; protected set; }
        public SceneObject Key { get; protected set; }
        public int KeyQuantity { get; protected set; }
        public bool IsTriggerLocked { get; protected set; }
        public string TriggerKey { get; private set; }

        public TransitionHandler(string mapID, float posX, float posY, float transitionX, float transitionY, float width, float height)
        {
            TransitionMapID = mapID;
            Position = new Vector2(posX, posY);
            TransitionPosition = new Vector2(transitionX, transitionY);
            CollisionMask = new Rectangle((int)Position.X, (int)Position.Y, (int)width, (int)height);
        }

        public void LockWithKey(SceneObject key, int keyQty)
        {
            IsKeyLocked = true;
            Key = key;
            KeyQuantity = keyQty;
        }

        public void LockWithTrigger(string triggerKey)
        {
            IsTriggerLocked = true;
            TriggerKey = triggerKey;
        }
    }
}
