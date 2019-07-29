using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace Hellscape
{
    public static class Common
    {
        public enum TextHalign
        {
            Left,
            Center,
            Right
        }
        public enum TextValign
        {
            Top,
            Middle,
            Bottom
        }

        public static Rectangle CreateCollisionMask(Vector2 position, int width, int height)
        {
            return new Rectangle((int)position.X, (int)position.Y, width, height);
        }
        public static int GetCollisionAngleIntersect(Point xZeroYOnMask, float rate = 1f)
        {
            return (int)(xZeroYOnMask.Y - (rate * xZeroYOnMask.X));
        }
        public static int GetCollisionAngleY(EntityCollisionSolid collisionObject, int playerCollisionMaskSide)
        {
            return (int)((playerCollisionMaskSide * collisionObject.Tilt) - collisionObject.YIntercept);
        }
        public static int GetYFromSlope(int x, float slope, int intercept)
        {
            return (int)(x * slope) + intercept;
        }
        public static Vector2 AdjustText(Vector2 position, BitmapFont font, string text, TextHalign halign, TextValign valign)
        {
            Vector2 stringMeasure = font.MeasureString(text);
            float hAdjust, vAdjust;
            hAdjust = 0f;
            vAdjust = 0f;

            switch (halign)
            {
                case TextHalign.Center:
                    {
                        hAdjust = -(stringMeasure.X / 2);
                        break;
                    }
                case TextHalign.Left:
                    {
                        hAdjust = 0f;
                        break;
                    }
                case TextHalign.Right:
                    {
                        hAdjust = -(stringMeasure.X);
                        break;
                    }
            }

            switch (valign)
            {
                case TextValign.Top:
                    {
                        vAdjust = 0f;
                        break;
                    }
                case TextValign.Middle:
                    {
                        vAdjust = -(stringMeasure.Y / 2);
                        break;
                    }
                case TextValign.Bottom:
                    {
                        vAdjust = -(stringMeasure.Y);
                        break;
                    }
            }

            return (position + new Vector2(hAdjust, vAdjust));
        }
    }
}
