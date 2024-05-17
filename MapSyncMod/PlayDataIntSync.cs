using MultiWorldLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapSyncMod
{
    public class PlayDataIntSync:BaseSync
    {
        public PlayDataIntSync() : base("MapSyncMod-PlayDataInt") { 

        }
        protected override void OnEnterGame()
        {
            On.PlayerData.SetInt += PlayerData_SetInt;
        }
        protected override void OnQuitToMenu()
        {
            On.PlayerData.SetInt -= PlayerData_SetInt;
        }

        private void PlayerData_SetInt(On.PlayerData.orig_SetInt orig, PlayerData self, string intName, int value)
        {
            bool send = false;
            if (value == 2 && PlayerData.instance?.GetInt(intName) != 2)
                if (BossDatas.Contains(intName) && MapSyncMod.GS.BossSync)
                    send = true;
            orig.Invoke(self, intName, value);
            if (send)
            {
                foreach (var toPlayerId in SyncPlayers)
                {
                    ItemSyncMod.ItemSyncMod.Connection.SendData(MESSAGE_LABEL,
                            JsonConvert.SerializeObject(intName),
                            toPlayerId);
                    MapSyncMod.LogDebug($"send to id[{toPlayerId}] name[{ItemSyncMod.ItemSyncMod.ISSettings.GetNicknames()[toPlayerId]}]");
                }
                ShowItemChangerSprite(intName, null, null, "ShopIcons.Marker_R");
                MapSyncMod.LogDebug($"sended setBool {intName}-{value}");
            }
        }

        protected override void OnDataReceived(DataReceivedEvent dataReceivedEvent)
        {
            string intName = JsonConvert.DeserializeObject<string>(dataReceivedEvent.Content);
            if (intName != null)
            {
                if (PlayerData.instance?.GetInt(intName) != 2)
                {
                    if (BossDatas.Contains(intName) && MapSyncMod.GS.BossSync)
                    {
                        PlayerData.instance.SetIntInternal(intName, 2);
                        ShowItemChangerSprite(intName, dataReceivedEvent.From, null, "ShopIcons.Marker_R");
                    }
                }
                MapSyncMod.LogDebug($"PlayDataBool get [{intName}]     form[{dataReceivedEvent.From}]");
            }

        }
        public static List<string> BossDatas = new List<string>{
                nameof(PlayerData.xeroDefeated),//泽若?
                nameof(PlayerData.noEyesDefeated),//无眼?
                nameof(PlayerData.elderHuDefeated),//胡?
                nameof(PlayerData.markothDefeated),//马尔斯?
                nameof(PlayerData.galienDefeated),//加利安
                nameof(PlayerData.mumCaterpillarDefeated),//马尔穆?
                nameof(PlayerData.aladarSlugDefeated),//戈布

                //nameof(PlayerData.),
        };
    }
}
