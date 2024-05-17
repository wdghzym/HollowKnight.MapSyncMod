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
    public class BossDoorSync :BaseSync
    {
        public BossDoorSync() : base("MapSyncMod-BossDoor")        { }
        protected override void OnEnterGame()
        {
            lastBossDoorStateTier1 = PlayerData.instance.bossDoorStateTier1;
            lastBossDoorStateTier2 = PlayerData.instance.bossDoorStateTier2;
            lastBossDoorStateTier3 = PlayerData.instance.bossDoorStateTier3;
            lastBossDoorStateTier4 = PlayerData.instance.bossDoorStateTier4;
            lastBossDoorStateTier5 = PlayerData.instance.bossDoorStateTier5;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        }

        private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
        {
            update();
        }

        protected override void OnQuitToMenu()
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        }

        BossSequenceDoor.Completion lastBossDoorStateTier1;
        BossSequenceDoor.Completion lastBossDoorStateTier2;
        BossSequenceDoor.Completion lastBossDoorStateTier3;
        BossSequenceDoor.Completion lastBossDoorStateTier4;
        BossSequenceDoor.Completion lastBossDoorStateTier5;

        private void update()
        {

            if (lastBossDoorStateTier1.completed == true && PlayerData.instance.bossDoorStateTier1.completed == false)
                PlayerData.instance.bossDoorStateTier1 = lastBossDoorStateTier1;
            if (lastBossDoorStateTier2.completed == true && PlayerData.instance.bossDoorStateTier1.completed == false)
                PlayerData.instance.bossDoorStateTier2 = lastBossDoorStateTier2;
            if (lastBossDoorStateTier3.completed == true && PlayerData.instance.bossDoorStateTier1.completed == false)
                PlayerData.instance.bossDoorStateTier3 = lastBossDoorStateTier3;
            if (lastBossDoorStateTier4.completed == true && PlayerData.instance.bossDoorStateTier1.completed == false)
                PlayerData.instance.bossDoorStateTier4 = lastBossDoorStateTier4;
            if (lastBossDoorStateTier5.completed == true && PlayerData.instance.bossDoorStateTier1.completed == false)
                PlayerData.instance.bossDoorStateTier5 = lastBossDoorStateTier5;

            if (lastBossDoorStateTier1.completed == false && PlayerData.instance.bossDoorStateTier1.completed == true)
                send(1, PlayerData.instance.bossDoorStateTier1);
            if (lastBossDoorStateTier2.completed == false && PlayerData.instance.bossDoorStateTier2.completed == true)
                send(2, PlayerData.instance.bossDoorStateTier2);
            if (lastBossDoorStateTier3.completed == false && PlayerData.instance.bossDoorStateTier3.completed == true)
                send(3, PlayerData.instance.bossDoorStateTier3);
            if (lastBossDoorStateTier4.completed == false && PlayerData.instance.bossDoorStateTier4.completed == true)
                send(4, PlayerData.instance.bossDoorStateTier4);
            if (lastBossDoorStateTier5.completed == false && PlayerData.instance.bossDoorStateTier5.completed == true)
                send(5, PlayerData.instance.bossDoorStateTier5);

            if (lastBossDoorStateTier1.completed == false)
                lastBossDoorStateTier1 = PlayerData.instance.bossDoorStateTier1;
            if (lastBossDoorStateTier2.completed == false)
                lastBossDoorStateTier2 = PlayerData.instance.bossDoorStateTier2;
            if (lastBossDoorStateTier3.completed == false)
                lastBossDoorStateTier3 = PlayerData.instance.bossDoorStateTier3;
            if (lastBossDoorStateTier4.completed == false)
                lastBossDoorStateTier4 = PlayerData.instance.bossDoorStateTier4;
            if (lastBossDoorStateTier5.completed == false)
                lastBossDoorStateTier5 = PlayerData.instance.bossDoorStateTier5;
        }
        private void send(int id, BossSequenceDoor.Completion completion)
        {
            if (!MapSyncMod.GS.BossDoorSync) return;
            foreach (var toPlayerId in SyncPlayers)
            {
                ItemSyncMod.ItemSyncMod.Connection.SendData(MESSAGE_LABEL,
                    JsonConvert.SerializeObject(new BossDoor { Id = id, Completion = completion }),
                    toPlayerId);
                MapSyncMod.LogDebug($"send to id[{toPlayerId}] name[{ItemSyncMod.ItemSyncMod.ISSettings.GetNicknames()[toPlayerId]}]");
            }
            ShowItemChangerSprite($"Pantheons {id} Completed", null, null, "ShopIcons.Marker_Y");
            MapSyncMod.LogDebug($"BossDoorSync send[{id}] {completion}");
        }

        protected override void OnDataReceived(DataReceivedEvent dataReceivedEvent)
        {
            if (!MapSyncMod.GS.BossDoorSync) return;
            BossDoor bossDoor = JsonConvert.DeserializeObject<BossDoor>(dataReceivedEvent.Content);
            if (bossDoor.Completion.completed)
            {

                switch (bossDoor.Id)
                {
                    case 1:
                        if (!PlayerData.instance.bossDoorStateTier1.completed)
                        {
                            lastBossDoorStateTier1 = PlayerData.instance.bossDoorStateTier1 = bossDoor.Completion;
                            ShowItemChangerSprite($"Pantheons {bossDoor.Id} Completed", dataReceivedEvent.From, null, "ShopIcons.Marker_Y");
                            MapSyncMod.LogDebug($"BossDoor Get {bossDoor.Id}");
                        }
                        break;
                    case 2:
                        if (!PlayerData.instance.bossDoorStateTier2.completed)
                        {
                            lastBossDoorStateTier2 = PlayerData.instance.bossDoorStateTier2 = bossDoor.Completion;
                            ShowItemChangerSprite($"Pantheons {bossDoor.Id} Completed", dataReceivedEvent.From, null, "ShopIcons.Marker_Y");
                            MapSyncMod.LogDebug($"BossDoor Get {bossDoor.Id}");
                        }
                        break;
                    case 3:
                        if (!PlayerData.instance.bossDoorStateTier3.completed)
                        {
                            lastBossDoorStateTier3 = PlayerData.instance.bossDoorStateTier3 = bossDoor.Completion;
                            ShowItemChangerSprite($"Pantheons {bossDoor.Id} Completed", dataReceivedEvent.From, null, "ShopIcons.Marker_Y");
                            MapSyncMod.LogDebug($"BossDoor Get {bossDoor.Id}");
                        }
                        break;
                    case 4:
                        if (!PlayerData.instance.bossDoorStateTier4.completed)
                        {
                            lastBossDoorStateTier4 = PlayerData.instance.bossDoorStateTier4 = bossDoor.Completion;
                            ShowItemChangerSprite($"Pantheons {bossDoor.Id} Completed", dataReceivedEvent.From, null, "ShopIcons.Marker_Y");
                            MapSyncMod.LogDebug($"BossDoor Get {bossDoor.Id}");
                        }
                        break;
                    case 5:
                        if (!PlayerData.instance.bossDoorStateTier5.completed)
                        {
                            lastBossDoorStateTier5 = PlayerData.instance.bossDoorStateTier5 = bossDoor.Completion;
                            ShowItemChangerSprite($"Pantheons {bossDoor.Id} Completed", dataReceivedEvent.From, null, "ShopIcons.Marker_Y");
                            MapSyncMod.LogDebug($"BossDoor Get {bossDoor.Id}");
                        }
                        break;
                }
            }
        }

        struct BossDoor
        {
            public int Id;
            public BossSequenceDoor.Completion Completion;
        }


    }
}
