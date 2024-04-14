using MapChanger;
using MultiWorldLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace MapSyncMod
{
    public class MapSync
    {
        public static readonly string SCENE_VISITED_MESSAGE_LABEL = "ItemSync-SceneVisited";
        public List<int> MapSyncPlayers = new List<int>();
        public MapSync()
        {
            Init();
        }
        public void Init()
        {
            Events.OnEnterGame += OnEnterGame;
            Events.OnQuitToMenu += OnQuitToMenu;
        }
        public void UnInit()
        {
            Events.OnEnterGame -= OnEnterGame;
            Events.OnQuitToMenu -= OnQuitToMenu;
            OnQuitToMenu();
        }
        private void OnEnterGame()
        {
            if (!ItemSyncMod.ItemSyncMod.ISSettings.IsItemSync) return;
            MapSyncMod.LogDebug($"OnEnterGame");
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
            ItemSyncMod.ItemSyncMod.Connection.OnDataReceived += OnDataReceived;
        }
        private void OnQuitToMenu()
        {
            MapSyncMod.LogDebug($"OnQuitToMenu");

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
            ItemSyncMod.ItemSyncMod.Connection.OnDataReceived -= OnDataReceived;

        }

        private void SceneManager_activeSceneChanged(Scene from, Scene to)
        {
            if (to.name == "Quit_To_Menu") return;
            if (!PlayerData.instance.scenesVisited.Contains(to.name))
            {
                MapSyncMod.LogDebug($"scenesVisited.!Contains[{to.name}]");

                if (ItemSyncMod.ItemSyncMod.Connection == null) return;

                MapSyncMod.LogDebug($"ItemSyncMod.Connection nonull[{to.name}]");

                if (!ItemSyncMod.ItemSyncMod.Connection.IsConnected()) return;

                MapSyncMod.LogDebug($"ItemSyncMod.Connection.IsConnected[{to.name}]");

                foreach (var toPlayerId in MapSyncPlayers)
                {
                    ItemSyncMod.ItemSyncMod.Connection.SendData(SCENE_VISITED_MESSAGE_LABEL,
                        JsonConvert.SerializeObject(to.name),
                        toPlayerId);
                    MapSyncMod.LogDebug($"send to id[{toPlayerId}] name[{ItemSyncMod.ItemSyncMod.ISSettings.GetNicknames()[toPlayerId]}]");
                }
                MapSyncMod.LogDebug($"send[{to.name}]");
            }
        }

        private void OnDataReceived(DataReceivedEvent dataReceivedEvent)
        {
            MapSyncMod.LogDebug($"OnDataReceived[{dataReceivedEvent.Label}]");
            if (dataReceivedEvent.Label != SCENE_VISITED_MESSAGE_LABEL) return;
            string scenes = dataReceivedEvent.Content.Replace("\"", "");

            MapSyncMod.LogDebug($"get[{scenes}]");

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
