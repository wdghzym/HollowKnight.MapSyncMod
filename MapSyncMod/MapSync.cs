using ItemChanger;
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
    public class MapSync:BaseSync
    {
        internal MapSync1000 mapSync1000;
        public MapSync() : base("MapSyncMod-SceneVisited") { mapSync1000 = new(); }
        protected override void OnEnterGame()
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
            /*if (Interop.HasRecentItemsDisplay())
                RecentItemsDisplay.ItemDisplayMethods.ShowItemInternal(new ItemChanger.UIDefs.MsgUIDef() { sprite = new ItemChangerSprite("ShopIcons.Marker_B") },
                    $"{"Map Sync".L()} {(MapSyncMod.GS.MapSync ? "Enabled".L() : "Disabled".L())}");*/
        }
        protected override void OnQuitToMenu()
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        }

        private void SceneManager_activeSceneChanged(Scene from, Scene to)
        {
            try
            {
                if (!MapSyncMod.GS.MapSync) return;
                if (to.name == "Quit_To_Menu") return;
                /*
                 Cinematic_Stag_travel
                 Fungus1_04_boss
                Fungus2_15_boss
                Fungus2_15_boss_defeated
                Ruins1_24_boss
                Ruins1_24_boss_defeated
                Mines_18_boss
                Dream_Nailcollection //梦钉
                Dream_Guardian_Lurien //守梦者守望者
                Dream_Guardian_Monomon
                Dream_Guardian_Hegemol
                 */
                if (!PlayerData.instance.scenesVisited.Contains(to.name))
                {
                    MapSyncMod.LogDebug($"scenesVisited.!Contains[{to.name.L()}]");
                    foreach (var toPlayerId in SyncPlayers)
                    {
                        ItemSyncMod.ItemSyncMod.Connection.SendData(MESSAGE_LABEL,
                            JsonConvert.SerializeObject(to.name),
                            toPlayerId);
                        MapSyncMod.LogDebug($"send to id[{toPlayerId}] name[{ItemSyncMod.ItemSyncMod.ISSettings.GetNicknames()[toPlayerId]}]");
                    }
                    mapSync1000.SceneManager_activeSceneChanged(from, to);
                    MapSyncMod.LogDebug($"send[{to.name.L()}]");
                }
            }
            catch (Exception e) { MapSyncMod.Instance.LogError($"{e.Message} \n{e.StackTrace}"); }
        }

        internal void OnDataReceived1000(DataReceivedEvent dataReceivedEvent) => this.OnDataReceived(dataReceivedEvent);
        protected override void OnDataReceived(DataReceivedEvent dataReceivedEvent)
        {
            if (!MapSyncMod.GS.MapSync) return;
            string scenes = JsonConvert.DeserializeObject<string>(dataReceivedEvent.Content);

            if (!MapChanger.Tracker.ScenesVisited.Contains(scenes))
                MapSyncMod.LogDebug($"mapSync get[{scenes.L()}]     form[{dataReceivedEvent.From}]");

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
        }
    }
}
