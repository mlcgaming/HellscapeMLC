﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Hellscape
{
    public static class Global
    {
        public static float GLOBAL_OPT_BGM_VOLUME = 1.0f;
        public static float GLOBAL_OPT_SFX_VOLUME = 1.0f;
        public static Vector2 GLOBAL_OPT_WINDOW_SIZE = new Vector2(1280, 720);
        public static bool GLOBAL_OPT_FULLSCREEN_ENABLED = false;
        public static bool GLOBAL_CONTROLLER_IS_CONNECTED = true;
        public const float GRAVITY_RATE = 8f;
        

        public static ContentManager Content;
        public static SpriteBatch SpriteBatch;
        public static GraphicsDeviceManager Graphics;
        public static GameWindow Window;
        public static GameTime GameTime;
        public static Dictionary<string, string> MapAssets = new Dictionary<string, string>();

        public static void InitializeDefaults(ContentManager content, SpriteBatch spriteBatch, GraphicsDeviceManager graphics, GameWindow window)
        {
            GLOBAL_OPT_BGM_VOLUME = 1.0f;
            GLOBAL_OPT_SFX_VOLUME = 1.0f;
            GLOBAL_OPT_FULLSCREEN_ENABLED = false;
            GLOBAL_OPT_WINDOW_SIZE = new Vector2(1280, 720);
            GLOBAL_CONTROLLER_IS_CONNECTED = false;
            Content = content;
            SpriteBatch = spriteBatch;
            Graphics = graphics;
            Window = window;
            GameTime = null;

            PopulateMapAssets();
        }
        public static void SetAudioLevels(float bgmVolume, float sfxVolume)
        {
            GLOBAL_OPT_BGM_VOLUME = bgmVolume;
            GLOBAL_OPT_SFX_VOLUME = sfxVolume;
        }
        public static void SetControllerEnabled(bool enabled)
        {
            GLOBAL_CONTROLLER_IS_CONNECTED = enabled;
        }
        public static void SetGameWindowSize(int width, int height)
        {
            GLOBAL_OPT_WINDOW_SIZE = new Vector2(width, height);
        }
        public static void SetFullscreen(bool enabled)
        {
            GLOBAL_OPT_FULLSCREEN_ENABLED = enabled;
        }
        public static void SetGameTime(GameTime gameTime)
        {
            GameTime = gameTime;
        }
        
        public static void PopulateMapAssets()
        {
            MapAssets.Add("testStageRoom1", "Data/maps/TestMap");
        }

        public static string GetMapAssetPathByID(string mapID)
        {
            return MapAssets[mapID];
        }
    }
}
