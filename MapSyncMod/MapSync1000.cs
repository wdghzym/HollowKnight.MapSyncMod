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
    public class MapSync1000:BaseSync
    {
        public MapSync1000():base("ItemSync-SceneVisited") { }

        internal void SceneManager_activeSceneChanged(Scene from, Scene to)
        {
            foreach (var toPlayerId in SyncPlayers)
            {
                ItemSyncMod.ItemSyncMod.Connection.SendData(MESSAGE_LABEL,
                    JsonConvert.SerializeObject(to.name),
                    toPlayerId);
                MapSyncMod.LogDebug($"1000send to id[{toPlayerId}] name[{ItemSyncMod.ItemSyncMod.ISSettings.GetNicknames()[toPlayerId]}]");
            }
        }

        protected override void OnDataReceived(DataReceivedEvent dataReceivedEvent)
        {
            MapSyncMod.Instance.MapSync.OnDataReceived1000(dataReceivedEvent);
        }
    }
}
