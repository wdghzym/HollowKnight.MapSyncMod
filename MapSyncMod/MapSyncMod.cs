using ItemSyncMod.SyncFeatures.TransitionsFoundSync;
using MapChanger;
using MapChanger.UI;
using Modding;
using MultiWorldLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.SceneManagement;
using static tk2dSpriteCollectionDefinition;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.SaveSlotButton;

namespace MapSyncMod
{
    public class MapSyncMod:Mod
    {
        public static readonly string SCENE_VISITED_MESSAGE_LABEL = "ItemSync-SceneVisited";
        public MapSyncMod() { }
        public override string GetVersion()
        {
            return GetType().Assembly.GetName().Version.ToString();
        }
        public override int LoadPriority() => 5;
        public override void Initialize()
        {
            base.Initialize();
            //On.GameManager.LoadGame += GameManager_LoadGame;
            //On.GameManager.ReturnToMainMenu += GameManager_ReturnToMainMenu;
            Events.OnEnterGame += OnEnterGame;
            Events.OnQuitToMenu += OnQuitToMenu;

        }

        private void OnEnterGame()
        {
#if DEBUG
            Log($"OnEnterGame");
#endif
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneManager_activeSceneChanged; ;
            ItemSyncMod.ItemSyncMod.Connection.OnDataReceived += OnDataReceived;
            //ItemSyncMod.ItemSyncMod.Connection.OnConnectedPlayersChanged
              //ItemSyncMod.ItemSyncMod.Connection.  
            //ItemSyncMod.ItemSyncMod.ISSettings.AddSentData

        }
        private void OnQuitToMenu()
        {
#if DEBUG
            Log($"OnQuitToMenu");
#endif
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged; ;
            ItemSyncMod.ItemSyncMod.Connection.OnDataReceived -= OnDataReceived;

        }


        private void SceneManager_activeSceneChanged(Scene from, Scene to)
        {
            if (to.name == "Quit_To_Menu") return;
            if (!PlayerData.instance.scenesVisited.Contains(to.name))
            {
#if DEBUG
                Log($"scenesVisited.!Contains[{to.name}]");
#endif
                if (ItemSyncMod.ItemSyncMod.Connection == null) return;

#if DEBUG
                Log($"ItemSyncMod.Connection nonull[{to.name}]");
#endif
                if (!ItemSyncMod.ItemSyncMod.Connection.IsConnected()) return;

#if DEBUG
                Log($"ItemSyncMod.Connection.IsConnected[{to.name}]");
#endif
                ItemSyncMod.ItemSyncMod.Connection.SendDataToAll(SCENE_VISITED_MESSAGE_LABEL,
                    JsonConvert.SerializeObject(to.name));

#if DEBUG
                Log($"send[{to.name}]");
#endif

            }
        }


        private void OnDataReceived(DataReceivedEvent dataReceivedEvent)
        {
#if DEBUG
            Log($"OnDataReceived[{dataReceivedEvent.Label}]");
#endif

            if (dataReceivedEvent.Label != SCENE_VISITED_MESSAGE_LABEL) return;
            string scenes = dataReceivedEvent.Content.Replace("\"", "");
#if DEBUG
            Log($"get[{scenes}]");
#endif
            if (!MapChanger.Tracker.ScenesVisited.Contains(scenes))
                MapChanger.Tracker.ScenesVisited.Add(scenes);

            

            //GameManager._instance.AddToScenesVisited(dataReceivedEvent.Content);
            if (!PlayerData.instance.scenesVisited.Contains(scenes))
                PlayerData.instance.scenesVisited.Add(scenes);
            
            GameManager._instance.UpdateGameMap();
            MapChanger.UI.MapUILayerUpdater.Update();
            foreach (var mapObject in MapChanger.MapObjectUpdater.MapObjects)
            {
                mapObject.MainUpdate();
            }
            dataReceivedEvent.Handled = true;
        }


    }
}
