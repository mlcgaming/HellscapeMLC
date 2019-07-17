using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Hellscape
{
    public class MoveInputEventArgs : EventArgs
    {
        public float InputValue { get; set; }
    }
    public static class InputManager
    {
        public static event EventHandler<MoveInputEventArgs> LeftPressed;
        public static event EventHandler<MoveInputEventArgs> RightPressed;
        public static event EventHandler<MoveInputEventArgs> UpPressed;
        public static event EventHandler<MoveInputEventArgs> DownPressed;
        public static event EventHandler JumpPressed;
        public static event EventHandler RunPressed;

        public static KeyboardState KeyState;
        public static GamePadState GPState;

        public static void ProcessInputKeyboard()
        {
            KeyState = Keyboard.GetState();

            if(KeyState.IsKeyDown(Keys.W) == true)
            {
                OnMovedUp(1.0f);
            }
            if (KeyState.IsKeyDown(Keys.S) == true)
            {
                OnMovedDown(1.0f);
            }
            if (KeyState.IsKeyDown(Keys.A) == true)
            {
                OnMovedLeft(1.0f);
            }
            if (KeyState.IsKeyDown(Keys.D) == true)
            {
                OnMovedRight(1.0f);
            }
            if (KeyState.IsKeyDown(Keys.Space) == true)
            {
                OnJumpPressed();
            }
            if(KeyState.IsKeyDown(Keys.LeftShift) == true)
            {
                OnRunPressed();
            }
        }
        public static void ProcessInputGamePad(PlayerIndex player)
        {
            GPState = GamePad.GetState(player);
            if(GPState.IsConnected == true)
            {
                float leftXCheck, leftYCheck;
                leftXCheck = Math.Abs(GPState.ThumbSticks.Left.X);
                leftYCheck = Math.Abs(GPState.ThumbSticks.Left.Y);

                if (leftXCheck > 0.1f)
                {
                    if(GPState.ThumbSticks.Left.X > 0)
                    {
                        OnMovedLeft(GPState.ThumbSticks.Left.X);
                    }
                    else
                    {
                        OnMovedRight(GPState.ThumbSticks.Left.X);
                    }
                }

                if(leftYCheck > 0.1f)
                {
                    if (GPState.ThumbSticks.Left.Y > 0)
                    {
                        OnMovedUp(GPState.ThumbSticks.Left.Y);
                    }
                    else
                    {
                        OnMovedDown(GPState.ThumbSticks.Left.Y);
                    }
                }

                if(GPState.IsButtonDown(Buttons.A) == true)
                {
                    OnJumpPressed();
                }
                if(GPState.IsButtonDown(Buttons.X) == true)
                {
                    OnRunPressed();
                }
            }
        }

        public static void OnMovedRight(float thumbstickX)
        {
            RightPressed?.Invoke(null, new MoveInputEventArgs() { InputValue = thumbstickX });
        }
        public static void OnMovedLeft(float thumbstickX)
        {
            LeftPressed?.Invoke(null, new MoveInputEventArgs() { InputValue = thumbstickX });
        }
        public static void OnMovedUp(float thumbstickY)
        {
            UpPressed?.Invoke(null, new MoveInputEventArgs() { InputValue = thumbstickY });
        }
        public static void OnMovedDown(float thumbstickY)
        {
            DownPressed?.Invoke(null, new MoveInputEventArgs() { InputValue = thumbstickY });
        }
        public static void OnJumpPressed()
        {
            JumpPressed?.Invoke(null, EventArgs.Empty);
        }
        public static void OnRunPressed()
        {
            RunPressed?.Invoke(null, EventArgs.Empty);
        }
    }
}
