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
        }
        private void OnEnterGame()
        {
#if DEBUG
            MapSyncMod.Instance.Log($"OnEnterGame");
#endif
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneManager_activeSceneChanged; ;
            ItemSyncMod.ItemSyncMod.Connection.OnDataReceived += OnDataReceived;
        }
        private void OnQuitToMenu()
        {
#if DEBUG
            MapSyncMod.Instance.Log($"OnQuitToMenu");
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
                MapSyncMod.Instance.Log($"scenesVisited.!Contains[{to.name}]");
#endif
                if (ItemSyncMod.ItemSyncMod.Connection == null) return;

#if DEBUG
                MapSyncMod.Instance.Log($"ItemSyncMod.Connection nonull[{to.name}]");
#endif
                if (!ItemSyncMod.ItemSyncMod.Connection.IsConnected()) return;

#if DEBUG
                MapSyncMod.Instance.Log($"ItemSyncMod.Connection.IsConnected[{to.name}]");
#endif
                ItemSyncMod.ItemSyncMod.Connection.SendDataToAll(SCENE_VISITED_MESSAGE_LABEL,
                    JsonConvert.SerializeObject(to.name));

#if DEBUG
                MapSyncMod.Instance.Log($"send[{to.name}]");
#endif

            }
        }


        private void OnDataReceived(DataReceivedEvent dataReceivedEvent)
        {
#if DEBUG
            MapSyncMod.Instance.Log($"OnDataReceived[{dataReceivedEvent.Label}]");
#endif

            if (dataReceivedEvent.Label != SCENE_VISITED_MESSAGE_LABEL) return;
            string scenes = dataReceivedEvent.Content.Replace("\"", "");
#if DEBUG
            MapSyncMod.Instance.Log($"get[{scenes}]");
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
