﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Hellscape
{
    public class AnimationManager
    {
        public Animation Animation { get; private set; }
        private float FrameTimer;

        public AnimationManager()
        {
            Animation = null;
        }

        public void Play(Animation animation)
        {
            if (animation == null)
            {
                return;
            }
            if (Animation == animation)
            {
                return;
            }

            Animation = animation;
            Animation.CurrentFrame = 0;
            FrameTimer = 0;
        }
        public void Stop()
        {
            FrameTimer = 0;
            Animation.CurrentFrame = 0;
        }

        public void Update()
        {
            FrameTimer += (float)Global.GameTime.ElapsedGameTime.TotalSeconds;

            if (FrameTimer >= Animation.FrameSpeed)
            {
                FrameTimer = 0f;
                Animation.GotoNextFrame();
            }
        }

        public void Draw(Vector2 position)
        {
            Global.SpriteBatch.Draw(Animation.Texture, position,
                new Rectangle((int)Animation.FrameStartPosition.X + (Animation.CurrentFrame * Animation.FrameWidth),
                (int)Animation.FrameStartPosition.Y, Animation.FrameWidth, Animation.FrameHeight),
                Color.White);
        }
        public void Draw(Rectangle rectangle, SpriteEffects effects, float alpha = 1f)
        {
            Global.SpriteBatch.Draw(Animation.Texture, rectangle,
                new Rectangle((int)Animation.FrameStartPosition.X + (Animation.CurrentFrame * Animation.FrameWidth),
                (int)Animation.FrameStartPosition.Y, Animation.FrameWidth, Animation.FrameHeight),
                Color.White * alpha, 0f, new Vector2(0), effects, 0f);
        }
    }
}
