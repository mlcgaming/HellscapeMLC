﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
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
        public static Dictionary<string, bool> DoorTriggers = new Dictionary<string, bool>();
        public static List<SceneObject> SceneObjects = new List<SceneObject>();

        public static bool IsNetworkGame;

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

            IsNetworkGame = false;

            PopulateDoorTriggers();
            PopulateSceneObjects();
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

        public static void PopulateDoorTriggers()
        {
            DoorTriggers.Add("DebugRoom1", false);
        }
        public static void PopulateSceneObjects()
        {
            SceneObject goldKey = new SceneObject("soKeyGold", "Gold Key", 1, new Animation(Content.Load<Texture2D>("GFX/Tiles/SceneObjects"), 1, 16, 16, 0f, new Vector2(0, 0)));
            SceneObjects.Add(goldKey);

            SceneObject silverKey = new SceneObject("soKeySilver", "Silver Key", 1, new Animation(Content.Load<Texture2D>("GFX/Tiles/SceneObjects"), 1, 16, 16, 0f, new Vector2(16, 0)));
            SceneObjects.Add(silverKey);
        }

        public static string GetMapAssetPathByID(string mapID)
        {
            return "Data/maps/" + mapID;
        }
        public static SceneObject GetSceneObjectBYID(string shortName)
        {
            foreach(SceneObject so in SceneObjects)
            {
                if(so.ShortName == shortName)
                {
                    return so;
                }
            }

            return null;
        }
    }
}
