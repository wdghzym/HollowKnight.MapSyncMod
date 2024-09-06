using MultiWorldLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapSyncMod
{
    public class BenchDeploySync : BaseSync
    {
        private Bench lastBench,lastDreamGate;
        public BenchDeploySync() : base("MapSyncMod-BenchDeploy")
        {
        }
        protected override void OnEnterGame()
        {
            base.OnEnterGame();
            On.HeroController.Update += HeroController_Update;
            lastBench = GetCurrentBench();
            lastDreamGate = GetCurrentDreamGate();
            HCUTimer.Restart();
        }
        public bool SkipBench;
        private readonly Stopwatch HCUTimer = new();
        //private readonly Stopwatch SBTimer = new();
        internal void SendBenchToPlayers(string benchScene, float benchX, float benchY)
        {
            if (!MapSyncMod.GS.BenchDeploySync) return;
            Bench bench = new Bench("Bench", benchScene, benchX, benchY);
            foreach (var toPlayerId in SyncPlayers)
            {
                ItemSyncMod.ItemSyncMod.Connection.SendData(MESSAGE_LABEL,
                        JsonConvert.SerializeObject(bench),
                        toPlayerId);
                MapSyncMod.LogDebug($"send to id[{toPlayerId}] name[{ItemSyncMod.ItemSyncMod.ISSettings.GetNicknames()[toPlayerId]}]");
            }
            MapSyncMod.LogDebug($"sended BenchDeploySync SendBenchToPlayers {bench.BenchScene} {bench.BenchX} {bench.BenchY}");
        }
        private void HeroController_Update(On.HeroController.orig_Update orig, HeroController self)
        {
            orig(self);
            try
            {
                if (!MapSyncMod.GS.BenchDeploySync) return;
                if (HCUTimer.ElapsedMilliseconds < 1000) return;
                HCUTimer.Restart();
                //MapSyncMod.LogDebug($"");
                if (lastBench.BenchScene != Benchwarp.Benchwarp.LS.benchScene || lastBench.BenchX != Benchwarp.Benchwarp.LS.benchX || lastBench.BenchY != Benchwarp.Benchwarp.LS.benchY)
                {
                    Bench currentBench = GetCurrentBench();
                    if (!SkipBench)
                    {
                        foreach (var toPlayerId in SyncPlayers)
                        {
                            ItemSyncMod.ItemSyncMod.Connection.SendData(MESSAGE_LABEL,
                                    JsonConvert.SerializeObject(currentBench),
                                    toPlayerId);
                            MapSyncMod.LogDebug($"send to id[{toPlayerId}] name[{ItemSyncMod.ItemSyncMod.ISSettings.GetNicknames()[toPlayerId]}]");
                        }
                        MapSyncMod.LogDebug($"sended BenchDeploySync {currentBench.BenchScene} {currentBench.BenchX} {currentBench.BenchY}");
                    }
                    SkipBench = false;
                    /*
                    //Export.BenchDeploy("name", "bench", currentBench.BenchScene, currentBench.BenchX, currentBench.BenchY);
                    //playersBench.Add(("name", currentBench.BenchScene, currentBench.BenchX, currentBench.BenchY, currentBench.BenchScene, currentBench.BenchX, currentBench.BenchY));
                    Event.BenchDeployingInternal("name", lastBench.BenchScene, lastBench.BenchX, lastBench.BenchY);
                    Event.BenchDeployingInternal("DDDAATOU", currentBench.BenchScene, currentBench.BenchX, currentBench.BenchY);
                    MapSyncMod.LogDebug($"lastbench {lastBench}");
                    MapSyncMod.LogDebug($"currentBench {currentBench}");
                    */
                    lastBench = currentBench;
                }
                if (lastDreamGate.BenchScene != PlayerData.instance.dreamGateScene || lastDreamGate.BenchX != PlayerData.instance.dreamGateX || lastDreamGate.BenchY != PlayerData.instance.dreamGateY)
                {
                    Bench dreamGate = GetCurrentDreamGate();
                    foreach (var toPlayerId in SyncPlayers)
                    {
                        ItemSyncMod.ItemSyncMod.Connection.SendData(MESSAGE_LABEL,
                                JsonConvert.SerializeObject(dreamGate),
                                toPlayerId);
                        MapSyncMod.LogDebug($"send to id[{toPlayerId}] name[{ItemSyncMod.ItemSyncMod.ISSettings.GetNicknames()[toPlayerId]}]");
                    }
                    MapSyncMod.LogDebug($"sended BenchDeploySync dreamGate {dreamGate.BenchScene} {dreamGate.BenchX} {dreamGate.BenchY}");
                    lastDreamGate = dreamGate;
                }
            }
            catch (Exception e) { MapSyncMod.Instance.LogError($"{e.Message} \n{e.StackTrace}"); }
        }

        protected override void OnQuitToMenu()
        {
            On.HeroController.Update -= HeroController_Update;
        }


        protected override void OnDataReceived(DataReceivedEvent dataReceivedEvent)
        {
            if (!MapSyncMod.GS.BenchDeploySync) return;
            Bench? bench = JsonConvert.DeserializeObject<Bench?>(dataReceivedEvent.Content);
            if (bench != null)
            {
                switch (bench.Value.Type)
                {
                    case "Bench":
                        Events.BenchDeployingInternal(dataReceivedEvent.From, bench.Value.BenchScene, bench.Value.BenchX, bench.Value.BenchY);
                        break;
                    case "DreamGate":
                        Events.DreamGateingInternal(dataReceivedEvent.From, bench.Value.BenchScene, bench.Value.BenchX, bench.Value.BenchY);
                        break;
                    default:
                        break;
                }
                MapSyncMod.LogDebug($"get BenchDeploySync {dataReceivedEvent.From} {bench}");
            }
        }

        private Bench GetCurrentBench()
        {
            return new Bench("Bench", Benchwarp.Benchwarp.LS.benchScene, Benchwarp.Benchwarp.LS.benchX, Benchwarp.Benchwarp.LS.benchY);
        }
        private Bench GetCurrentDreamGate()
        {
            return new Bench("DreamGate", PlayerData.instance.dreamGateScene, PlayerData.instance.dreamGateX, PlayerData.instance.dreamGateY);
        }
        public struct Bench
        {
            public string Type;
            public float BenchX;
            public float BenchY;
            public string BenchScene;

            public Bench(string type ,string scene, float x, float y)
            {
                Type = type;
                BenchScene = scene;
                BenchX = x;
                BenchY = y;
            }
            public override string ToString()
            {
                return $"{BenchScene}:{BenchX}_{BenchY}";
            }
        }
    }
}
