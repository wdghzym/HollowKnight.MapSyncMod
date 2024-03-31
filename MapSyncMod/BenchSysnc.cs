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
    public class BenchSysnc
    {
        public static readonly string BENCH_UNLOCK_MESSAGE_LABEL = "ItemSync-BenchUnlock";
        public BenchSysnc()
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
            Benchwarp.Events.OnBenchUnlock += Events_OnBenchUnlock;

            ItemSyncMod.ItemSyncMod.Connection.OnDataReceived += OnDataReceived;
        }


        private void OnQuitToMenu()
        {
#if DEBUG
            MapSyncMod.Instance.Log($"OnQuitToMenu");
#endif
            Benchwarp.Events.OnBenchUnlock -= Events_OnBenchUnlock;
            ItemSyncMod.ItemSyncMod.Connection.OnDataReceived -= OnDataReceived;
        }
        private void Events_OnBenchUnlock(Benchwarp.BenchKey obj)
        {/*
            var temp = new Benchwarp.BenchKey("Crossroads_30", "RestBench");
            if (!Benchwarp.Benchwarp.LS.visitedBenchScenes.Contains(temp))
            {
                Benchwarp.Benchwarp.LS.visitedBenchScenes.Add(temp);
            }*/
#if DEBUG
            MapSyncMod.Instance.Log($"Events_OnBenchUnlock[{obj.SceneName}][{obj.RespawnMarkerName}]");
#endif

                if (ItemSyncMod.ItemSyncMod.Connection == null) return;

#if DEBUG
                MapSyncMod.Instance.Log($"ItemSyncMod.Connection nonull[]");
#endif
                if (!ItemSyncMod.ItemSyncMod.Connection.IsConnected()) return;

#if DEBUG
                MapSyncMod.Instance.Log($"ItemSyncMod.Connection.IsConnected[]");
#endif
                ItemSyncMod.ItemSyncMod.Connection.SendDataToAll(BENCH_UNLOCK_MESSAGE_LABEL,
                    JsonConvert.SerializeObject(obj));

#if DEBUG
                MapSyncMod.Instance.Log($"send[{obj.SceneName}][{obj.RespawnMarkerName}]");
#endif

        }
        private void OnDataReceived(DataReceivedEvent dataReceivedEvent)
        {
#if DEBUG
            MapSyncMod.Instance.Log($"OnDataReceived[{dataReceivedEvent.Label}]");
#endif

            if (dataReceivedEvent.Label != BENCH_UNLOCK_MESSAGE_LABEL) return;

            Benchwarp.BenchKey benchKey = JsonConvert.DeserializeObject<Benchwarp.BenchKey>(dataReceivedEvent.Content);
#if DEBUG
            MapSyncMod.Instance.Log($"BenchSysnc get Bench[{benchKey.SceneName}][{benchKey.RespawnMarkerName}]");
#endif
            if (!Benchwarp.Benchwarp.LS.visitedBenchScenes.Contains(benchKey))
            {
#if DEBUG
                MapSyncMod.Instance.Log($"BenchSysnc Bench Is New");
#endif
                Benchwarp.Benchwarp.LS.visitedBenchScenes.Add(benchKey);
            }
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
