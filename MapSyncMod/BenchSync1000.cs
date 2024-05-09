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
    public class BenchSync1000:BaseSync
    {
        public BenchSync1000() : base("ItemSync-BenchUnlock") { }
        internal void Events_OnBenchUnlock(Benchwarp.BenchKey benchKey)
        {
            if (ItemSyncMod.ItemSyncMod.Connection?.IsConnected() != true) return;
            foreach (var toPlayerId in SyncPlayers)
            {
                ItemSyncMod.ItemSyncMod.Connection.SendData(MESSAGE_LABEL,
                        JsonConvert.SerializeObject(benchKey),
                        toPlayerId);
                MapSyncMod.LogDebug($"1000send to id[{toPlayerId}] name[{ItemSyncMod.ItemSyncMod.ISSettings.GetNicknames()[toPlayerId]}]");
            }
        }

        protected override void OnDataReceived(DataReceivedEvent dataReceivedEvent)
        {
            MapSyncMod.Instance.BenchSync.OnDataReceived1000(dataReceivedEvent);
        }
    }
}
