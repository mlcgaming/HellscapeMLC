using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using Newtonsoft.Json;
using MonoGame.Extended.BitmapFonts;

namespace Hellscape
{
    public static class Global
    {
        public static string DefaultsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/HellscapeDebug/data/default/";

        public static float GLOBAL_OPT_BGM_VOLUME = 1.0f;
        public static float GLOBAL_OPT_SFX_VOLUME = 1.0f;
        public static Vector2 GLOBAL_OPT_WINDOW_SIZE = new Vector2(1024, 576);
        public static bool GLOBAL_OPT_FULLSCREEN_ENABLED = false;
        public static bool GLOBAL_CONTROLLER_IS_CONNECTED = true;
        public const float GRAVITY_RATE = 8f;

        public static ContentManager Content;
        public static SpriteBatch SpriteBatch;
        public static GraphicsDeviceManager Graphics;
        public static GameWindow Window;
        public static GameTime GameTime;
        public static Dictionary<string, bool> ItemTriggers = new Dictionary<string, bool>();
        public static Dictionary<string, bool> DoorTriggers = new Dictionary<string, bool>();
        public static List<SceneObject> SceneObjects = new List<SceneObject>();

        public static Texture2D DebugTexture;
        public static BitmapFont DebugFont;

        public static bool IsNetworkGame;

        public static void InitializeDefaults(ContentManager content, SpriteBatch spriteBatch, GraphicsDeviceManager graphics, GameWindow window)
        {
            GLOBAL_OPT_BGM_VOLUME = 1.0f;
            GLOBAL_OPT_SFX_VOLUME = 1.0f;
            GLOBAL_OPT_FULLSCREEN_ENABLED = false;
            GLOBAL_OPT_WINDOW_SIZE = new Vector2(GLOBAL_OPT_WINDOW_SIZE.X, GLOBAL_OPT_WINDOW_SIZE.Y);
            GLOBAL_CONTROLLER_IS_CONNECTED = false;
            Content = content;
            SpriteBatch = spriteBatch;
            Graphics = graphics;
            Window = window;
            GameTime = null;

            DebugTexture = Content.Load<Texture2D>("GFX/SinglePixel");
            DebugFont = content.Load<BitmapFont>("GFX/Fonts/DebugFont");

            IsNetworkGame = false;

            PopulateDoorTriggers();
            PopulateSceneObjects();

            Directory.CreateDirectory(DefaultsPath);
            PopulateRoomSceneObjectLists();
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

        public static void PopulateItemTriggers()
        {
            ItemTriggers.Add("stoneObjectRoom7", false);
        }
        public static void PopulateDoorTriggers()
        {
            DoorTriggers.Add("DebugRoom1", false);
        }
        public static void PopulateSceneObjects()
        {
            SceneObject nullItem = new SceneObject("nullItem", "DEBUGITEM", 1, new Animation(DebugTexture, 1, 1, 1, 0, new Vector2(0, 0)));
            SceneObjects.Add(nullItem);

            SceneObject goldKey = new SceneObject("soKeyGold", "Gold Key", 1, new Animation(Content.Load<Texture2D>("GFX/Tiles/SceneObjects"), 1, 16, 16, 0f, new Vector2(0, 0)));
            SceneObjects.Add(goldKey);

            SceneObject silverKey = new SceneObject("soKeySilver", "Silver Key", 1, new Animation(Content.Load<Texture2D>("GFX/Tiles/SceneObjects"), 1, 16, 16, 0f, new Vector2(16, 0)));
            SceneObjects.Add(silverKey);
        }
        public static void PopulateRoomSceneObjectLists()
        {
            List<MapLoaderSceneObject> debugRoom3 = new List<MapLoaderSceneObject>();
            List<MapLoaderSceneObject> debugRoom4 = new List<MapLoaderSceneObject>();
            List<MapLoaderSceneObject> debugRoom5 = new List<MapLoaderSceneObject>();
            List<MapLoaderSceneObject> debugRoom6 = new List<MapLoaderSceneObject>();
            List<MapLoaderSceneObject> debugRoom7 = new List<MapLoaderSceneObject>();

            MapLoaderSceneObject soKeyGold = new MapLoaderSceneObject("soKeyGold", 320f, 160f, 1);

            debugRoom5.Add(soKeyGold);

            WriteRoomSceneObjectFile("DebugRoom3", debugRoom3);
            WriteRoomSceneObjectFile("DebugRoom4", debugRoom4);
            WriteRoomSceneObjectFile("DebugRoom5", debugRoom5);
            WriteRoomSceneObjectFile("DebugRoom6", debugRoom6);
            WriteRoomSceneObjectFile("DebugRoom7", debugRoom7);
        }
        public static void WriteRoomSceneObjectFile(string fileName, List<MapLoaderSceneObject> objects)
        {
            string filePath = Path.Combine(DefaultsPath, fileName + ".json");
            File.WriteAllText(filePath, JsonConvert.SerializeObject(objects, Formatting.Indented));
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
