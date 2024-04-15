using Benchwarp;
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
    public class BenchSync
    {
        public static readonly string BENCH_UNLOCK_MESSAGE_LABEL = "ItemSync-BenchUnlock";
        public List<int> BenchSyncPlayers = new List<int>();
        public BenchSync()
        {
            if (Interop.HasBenchwarp())
                Init();
        }
        public void Init()
        {
            MapChanger.Events.OnEnterGame += OnEnterGame;
            MapChanger.Events.OnQuitToMenu += OnQuitToMenu;
        }
        public void UnInit()
        {
            MapChanger.Events.OnEnterGame -= OnEnterGame;
            MapChanger.Events.OnQuitToMenu -= OnQuitToMenu;
            OnQuitToMenu();
        }
        private void OnEnterGame()
        {
            if (!ItemSyncMod.ItemSyncMod.ISSettings.IsItemSync) return;
            MapSyncMod.LogDebug($"BenchSync OnEnterGame");
            Benchwarp.Events.OnBenchUnlock += Events_OnBenchUnlock;
            ItemSyncMod.ItemSyncMod.Connection.OnDataReceived += OnDataReceived;
        }

        private void OnQuitToMenu()
        {
            MapSyncMod.LogDebug($"OnQuitToMenu");
            Benchwarp.Events.OnBenchUnlock -= Events_OnBenchUnlock;
            ItemSyncMod.ItemSyncMod.Connection.OnDataReceived -= OnDataReceived;
        }
        private void Events_OnBenchUnlock(Benchwarp.BenchKey benchKey)
        {
            MapSyncMod.LogDebug($"Events_OnBenchUnlock[{benchKey.SceneName}][{benchKey.RespawnMarkerName}]");
            if (ItemSyncMod.ItemSyncMod.Connection == null) return;

            MapSyncMod.LogDebug($"ItemSyncMod.Connection nonull[]");

            if (!ItemSyncMod.ItemSyncMod.Connection.IsConnected()) return;

            MapSyncMod.LogDebug($"ItemSyncMod.Connection.IsConnected[]");

            foreach (var toPlayerId in BenchSyncPlayers)
            {
                ItemSyncMod.ItemSyncMod.Connection.SendData(BENCH_UNLOCK_MESSAGE_LABEL,
                        JsonConvert.SerializeObject(benchKey),
                        toPlayerId);
                MapSyncMod.LogDebug($"send to id[{toPlayerId}] name[{ItemSyncMod.ItemSyncMod.ISSettings.GetNicknames()[toPlayerId]}]");
            }
            if (Interop.HasRecentItemsDisplay())
                RecentItemsDisplay.Export.ShowItemChangerSprite($"{getBenchNmae(benchKey)}", "ShopIcons.BenchPin");

            MapSyncMod.LogDebug($"send[{benchKey.SceneName}][{benchKey.RespawnMarkerName}]");
        }

        private void OnDataReceived(DataReceivedEvent dataReceivedEvent)
        {
            if (dataReceivedEvent.Label != BENCH_UNLOCK_MESSAGE_LABEL) return;

            Benchwarp.BenchKey benchKey = JsonConvert.DeserializeObject<Benchwarp.BenchKey>(dataReceivedEvent.Content);

            MapSyncMod.LogDebug($"BenchSync get Bench[{benchKey.SceneName}][{benchKey.RespawnMarkerName}]");

            if (!Benchwarp.Benchwarp.LS.visitedBenchScenes.Contains(benchKey))
            {
                MapSyncMod.LogDebug($"BenchSync Bench Is New");
                if (Interop.HasRecentItemsDisplay())
                    RecentItemsDisplay.Export.ShowItemChangerSprite($"{getBenchNmae(benchKey)}\n from {dataReceivedEvent.From}", "ShopIcons.BenchPin");

                Benchwarp.Benchwarp.LS.visitedBenchScenes.Add(benchKey);
                UnlockBench(benchKey.SceneName);
            }
            GameManager._instance.UpdateGameMap();
            MapChanger.UI.MapUILayerUpdater.Update();
            foreach (var mapObject in MapChanger.MapObjectUpdater.MapObjects)
            {
                mapObject.MainUpdate();
            }
            dataReceivedEvent.Handled = true;
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
                            return $"{Benchwarp.Localization.Localize(item.areaName)}-{Benchwarp.Localization.Localize(item.name)}";
                        default:
                            return $"{Benchwarp.Localization.Localize(item.name)}";
                    }
                }
            }
            return $"{Benchwarp.Localization.Localize(benchKey.SceneName)}-{Benchwarp.Localization.Localize(benchKey.RespawnMarkerName)}";
        }
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
