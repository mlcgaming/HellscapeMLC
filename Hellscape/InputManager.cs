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
        public static event EventHandler InteractPressed;
        public static event EventHandler JumpPressed;
        public static event EventHandler RunPressed;
        public static event EventHandler RunReleased;
        public static event EventHandler StartPressed;

        public static Dictionary<Keys, bool> KeysPressed = new Dictionary<Keys, bool>();
        public static Dictionary<Buttons, bool> ButtonsPressed = new Dictionary<Buttons, bool>();

        public static KeyboardState KeyState;
        public static GamePadState GPState;

        public static void Initialize()
        {
            KeysPressed.Add(Keys.E, false);
            KeysPressed.Add(Keys.Space, false);
            KeysPressed.Add(Keys.LeftShift, false);
            KeysPressed.Add(Keys.Escape, false);

            ButtonsPressed.Add(Buttons.A, false);
            ButtonsPressed.Add(Buttons.B, false);
            ButtonsPressed.Add(Buttons.X, false);
            ButtonsPressed.Add(Buttons.Start, false);
        }
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
            if (KeyState.IsKeyDown(Keys.E) == true)
            {
                OnInteractPressed();
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
                if(GPState.IsButtonDown(Buttons.B) == true)
                {
                    if (ButtonsPressed[Buttons.B] == false)
                    {
                        ButtonsPressed[Buttons.B] = true;
                        OnInteractPressed();
                    }
                }
                if (GPState.IsButtonUp(Buttons.B) == true)
                {
                    if (ButtonsPressed[Buttons.B] == true)
                    {
                        ButtonsPressed[Buttons.B] = false;
                    }
                }
                if (GPState.IsButtonDown(Buttons.A) == true)
                {
                    if (ButtonsPressed[Buttons.A] == false)
                    {
                        ButtonsPressed[Buttons.A] = true;
                        OnJumpPressed();
                    }
                }
                if (GPState.IsButtonUp(Buttons.A) == true)
                {
                    if (ButtonsPressed[Buttons.A] == true)
                    {
                        ButtonsPressed[Buttons.A] = false;
                    }
                }
                if (GPState.IsButtonDown(Buttons.X) == true)
                {
                    if(ButtonsPressed[Buttons.X] == false)
                    {
                        ButtonsPressed[Buttons.X] = true;
                        OnRunPressed();
                    }
                }
                if (GPState.IsButtonUp(Buttons.X) == true)
                {
                    if (ButtonsPressed[Buttons.X] == true)
                    {
                        ButtonsPressed[Buttons.X] = false;
                        OnRunReleased();
                    }
                }
                if (GPState.IsButtonDown(Buttons.Start) == true)
                {
                    if (ButtonsPressed[Buttons.Start] == false)
                    {
                        ButtonsPressed[Buttons.Start] = true;
                        OnStartPressed();
                    }
                }
                if (GPState.IsButtonUp(Buttons.Start) == true)
                {
                    if (ButtonsPressed[Buttons.Start] == true)
                    {
                        ButtonsPressed[Buttons.Start] = false;
                    }
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
        public static void OnInteractPressed()
        {
            InteractPressed?.Invoke(null, EventArgs.Empty);
        }
        public static void OnJumpPressed()
        {
            JumpPressed?.Invoke(null, EventArgs.Empty);
        }
        public static void OnRunPressed()
        {
            RunPressed?.Invoke(null, EventArgs.Empty);
        }
        public static void OnRunReleased()
        {
            RunReleased?.Invoke(null, EventArgs.Empty);
        }
        public static void OnStartPressed()
        {
            StartPressed?.Invoke(null, EventArgs.Empty);
        }
    }
}
