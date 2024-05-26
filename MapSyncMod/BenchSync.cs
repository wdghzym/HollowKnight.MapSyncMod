using Benchwarp;
using ItemChanger;
using ItemSyncMod;
using ItemSyncMod.SyncFeatures.TransitionsFoundSync;
using MapChanger;
using MultiWorldLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapSyncMod
{
    public class BenchSync:BaseSync
    {
        internal BenchSync1000 benchSync1000;
        public BenchSync() : base("MapSyncMod-BenchUnlock") { benchSync1000 = new(); }
        protected override void OnEnterGame()
        {
            Benchwarp.Events.OnBenchUnlock += Events_OnBenchUnlock;
            /*if (Interop.HasRecentItemsDisplay())
                RecentItemsDisplay.ItemDisplayMethods.ShowItemInternal(new ItemChanger.UIDefs.MsgUIDef() { sprite = new ItemChangerSprite("ShopIcons.Marker_B") },
                    $"{"Bench Sync".L()} {(MapSyncMod.GS.BenchSync ? "Enabled".L() : "Disabled".L())}");*/
        }
        protected override void OnQuitToMenu()
        {
            Benchwarp.Events.OnBenchUnlock -= Events_OnBenchUnlock;
        }
        private void Events_OnBenchUnlock(Benchwarp.BenchKey benchKey)
        {
            try
            {
                if (!MapSyncMod.GS.BenchSync) return;
                MapSyncMod.LogDebug($"Events_OnBenchUnlock[{benchKey.SceneName}][{benchKey.RespawnMarkerName}]");
                foreach (var toPlayerId in SyncPlayers)
                {
                    ItemSyncMod.ItemSyncMod.Connection.SendData(MESSAGE_LABEL,
                            JsonConvert.SerializeObject(benchKey),
                            toPlayerId);
                    MapSyncMod.LogDebug($"send to id[{toPlayerId}] name[{ItemSyncMod.ItemSyncMod.ISSettings.GetNicknames()[toPlayerId]}]");
                }
                benchSync1000.Events_OnBenchUnlock(benchKey);
                ShowItemChangerSprite(getBenchNmae(benchKey), null, null, "ShopIcons.BenchPin");

                MapSyncMod.LogDebug($"send[{benchKey.SceneName}][{benchKey.RespawnMarkerName}]");
            }
            catch (Exception e) { MapSyncMod.Instance.LogError($"{e.Message} \n{e.StackTrace}"); }
        }

        internal void OnDataReceived1000(DataReceivedEvent dataReceivedEvent) => this.OnDataReceived(dataReceivedEvent);
        protected override void OnDataReceived(DataReceivedEvent dataReceivedEvent)
        {
            if (!MapSyncMod.GS.BenchSync) return;
            Benchwarp.BenchKey benchKey = JsonConvert.DeserializeObject<Benchwarp.BenchKey>(dataReceivedEvent.Content);

            MapSyncMod.LogDebug($"BenchSync get Bench[{benchKey.SceneName}][{benchKey.RespawnMarkerName}]\n     form[{dataReceivedEvent.From}]");

            if (!Benchwarp.Benchwarp.LS.visitedBenchScenes.Contains(benchKey))
            {
                MapSyncMod.LogDebug($"BenchSync Bench Is New");
                ShowItemChangerSprite(getBenchNmae(benchKey), dataReceivedEvent.From, null, "ShopIcons.BenchPin");
                Benchwarp.Benchwarp.LS.visitedBenchScenes.Add(benchKey);
                UnlockBench(benchKey.SceneName);
            }
            GameManager._instance.UpdateGameMap();
            MapChanger.UI.MapUILayerUpdater.Update();
            foreach (var mapObject in MapChanger.MapObjectUpdater.MapObjects)
            {
                mapObject.MainUpdate();
            }
        }

        private string getBenchNmae(BenchKey benchKey)
        {
            foreach (var item in Benchwarp.Bench.Benches)
            {
                if (benchKey.SceneName == item.sceneName && benchKey.RespawnMarkerName == item.respawnMarker)
                {
                    switch (item.name)
                    {
                        case "Toll":
                        case "Stag":
                        case "Hot Springs":
                            return $"{item.areaName.BL()}-{item.name.BL()}";
                        default:
                            return $"{item.name.BL()}";
                    }
                }
            }
            return $"{benchKey.SceneName.L()}-{benchKey.RespawnMarkerName.L()}";
        }
        //from Benchwarp
        static readonly (string, string)[] _lockedBenches = new (string, string)[]
        {
            ("Hive_01", "Hive Bench"),
            ("Ruins1_31", "Toll Machine Bench"),
            ("Abyss_18", "Toll Machine Bench"),
            ("Fungus3_50", "Toll Machine Bench")
        };
        private void UnlockBench(string sceneName)
        {
            if (_lockedBenches.FirstOrDefault(p => p.Item1 == sceneName).Item2 is string id)
            {
                GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
                {
                    activated = true,
                    sceneName = sceneName,
                    semiPersistent = false,
                    id = id
                });
            }
            switch (sceneName)
            {
                /*
                case "Town":
                    PlayerData.instance.SetBool(nameof(PlayerData.visitedDirtmouth), true);
                    break;
                case "Crossroads_04":
                    PlayerData.instance.SetBool(nameof(PlayerData.visitedCrossroads), true);
                    break;
                case "White_Palace_01":
                    PlayerData.instance.SetBool(nameof(PlayerData.visitedWhitePalace), true);
                    break;
                */
                case "Room_Tram":
                    PlayerData.instance.SetBool(nameof(PlayerData.openedTramLower), true);
                    PlayerData.instance.SetBool(nameof(PlayerData.tramOpenedDeepnest), true);
                    break;
                case "Room_Tram_RG":
                    PlayerData.instance.SetBool(nameof(PlayerData.openedTramRestingGrounds), true);
                    PlayerData.instance.SetBool(nameof(PlayerData.tramOpenedCrossroads), true);
                    break;
            }
        }
    }
}
