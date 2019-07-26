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
        public bool IsInteractive { get; private set; }
        private float InteractionTimer;
        public bool IsSilent { get; private set; }

        public TransitionHandler(string mapID, float posX, float posY, float transitionX, float transitionY, float width, float height)
        {
            TransitionMapID = mapID;
            Position = new Vector2(posX, posY);
            TransitionPosition = new Vector2(transitionX, transitionY);
            CollisionMask = new Rectangle((int)Position.X, (int)Position.Y, (int)width, (int)height);
            IsInteractive = true;
            InteractionTimer = 0f;
            IsSilent = false;
        }

        public void Update()
        {
            float deltaTime = (float)Global.GameTime.ElapsedGameTime.TotalSeconds;
            if(InteractionTimer > 0)
            {
                InteractionTimer -= deltaTime;
            }
            else
            {
                IsInteractive = true;
            }
        }

        public void StartInteraction()
        {
            IsInteractive = false;
            InteractionTimer = 0.35f;
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

        public void SetAsSilent()
        {
            IsSilent = true;
        }
    }
}
