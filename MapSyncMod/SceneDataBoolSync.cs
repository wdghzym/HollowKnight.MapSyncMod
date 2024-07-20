using Benchwarp;
using ItemChanger;
using ItemChanger.Modules;
using MultiWorldLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MapSyncMod
{
    public class SceneDataBoolSync:BaseSync
    {
        public SceneDataBoolSync():base("MapSyncMod-SceneDataBool") { }
        protected override void OnEnterGame()
        {
            On.SceneData.SaveMyState_PersistentBoolData += SceneData_SaveMyState_PersistentBoolData;
            //On.GameManager.SaveLevelState += SavePersistentBoolItems;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
            /*
            if (Interop.HasRecentItemsDisplay())
                RecentItemsDisplay.ItemDisplayMethods.ShowItemInternal(new ItemChanger.UIDefs.MsgUIDef() { sprite = new ItemChangerSprite("ShopIcons.Marker_B") },
                    $"{"Other Sync".L()} {(MapSyncMod.GS.OtherSync ? "Enabled".L() : "Disabled".L())}");*/
        }

        protected override void OnQuitToMenu()
        {
            On.SceneData.SaveMyState_PersistentBoolData -= SceneData_SaveMyState_PersistentBoolData;
            //On.GameManager.SaveLevelState -= SavePersistentBoolItems;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
            LastPersistentBoolData.Clear();
            QueuedPersistentBoolData.Clear();
        }
        private void SceneData_SaveMyState_PersistentBoolData(On.SceneData.orig_SaveMyState_PersistentBoolData orig, SceneData self, PersistentBoolData persistentBoolData)
        {
            bool? lastActivated = false;
            try
            {
                lastActivated = GameManager.instance.sceneData.FindMyState(persistentBoolData)?.activated;
            }
            catch (Exception e) { MapSyncMod.Instance.LogError($"{e.Message} \n{e.StackTrace}"); }
            if (FindQueued(persistentBoolData) == null)
                orig.Invoke(self, persistentBoolData);
            //else
            //    send = false;
            //if (GameManager.instance.sceneData.FindMyState(persistentBoolData)?.activated != true)
            //    MapSyncMod.LogDebug($"------{Regex.Replace(persistentBoolData.id, @"([\s(\(\)\1-9]+)$", "")}");
            try
            {
                bool send = false;
                //LastPersistentBoolData.TryGetValue((persistentBoolData.sceneName, persistentBoolData.id))

                if (persistentBoolData?.activated == true)
                    if (lastActivated != true
                        || (LastPersistentBoolData.ContainsKey(persistentBoolData) && LastPersistentBoolData[persistentBoolData] == false))
                    {                            
                        string name = Regex.Replace(persistentBoolData.id, @"([\s(\(\)\1-9]+)$", "");

                        send = true;
                        if (BattleDatas.Contains(name) && MapSyncMod.GS.BossSync)
                        {
                            if (bossname.TryGetValue((persistentBoolData.id, persistentBoolData.sceneName), out string showname))
                                ShowItemChangerSprite(showname, null, null, "ShopIcons.Marker_R");
                            else
                                ShowItemChangerSprite(persistentBoolData.id, null, persistentBoolData.sceneName, "ShopIcons.Marker_R");
                        }
                        else if (LeverDatas.Contains(name) && MapSyncMod.GS.LeverSync)
                        {
                            if (MapSyncMod.GS.LeverDisplay)
                                ShowItemChangerSprite(persistentBoolData.id, null, persistentBoolData.sceneName, "ShopIcons.Marker_B");
                        }
                        else if (WallDatas.Contains(name) && MapSyncMod.GS.WallSync)
                        {
                            if (MapSyncMod.GS.WallDisplay)
                                ShowItemChangerSprite(persistentBoolData.id, null, persistentBoolData.sceneName, "ShopIcons.Marker_B");
                        }
                        else if (OtherDatas.Contains(name) && MapSyncMod.GS.OtherSync)
                        {
                            if (MapSyncMod.GS.OtherDisplay)
                                ShowItemChangerSprite(persistentBoolData.id, null, persistentBoolData.sceneName, "ShopIcons.Marker_B");
                        }
                        else if (MDatas.Contains(name))
                        {
                        }
                        else
                            send = false;
                    }

                if (send)
                {
                    foreach (var toPlayerId in SyncPlayers)
                    {
                        ItemSyncMod.ItemSyncMod.Connection.SendData(MESSAGE_LABEL,
                                JsonConvert.SerializeObject(persistentBoolData),
                                toPlayerId);
                        MapSyncMod.LogDebug($"send to id[{toPlayerId}] name[{ItemSyncMod.ItemSyncMod.ISSettings.GetNicknames()[toPlayerId]}]");
                    }
                    MapSyncMod.LogDebug($"sended SceneDataBool {persistentBoolData.activated}-{persistentBoolData.sceneName}-{persistentBoolData.sceneName.L()}--{persistentBoolData.semiPersistent}-{persistentBoolData.id}");
                }
                //else
                //MapSyncMod.LogDebug($"no send BoolData {persistentBoolData.activated}-{persistentBoolData.sceneName}--{persistentBoolData.semiPersistent}-{persistentBoolData.id}");
                MapSyncMod.LogDebug($"AddLastPersistentBoolData {persistentBoolData.activated}-{persistentBoolData.sceneName}-{persistentBoolData.sceneName.L()}--{persistentBoolData.semiPersistent}-{persistentBoolData.id} find {LastPersistentBoolData.ContainsKey(persistentBoolData)}");

                //if (persistentBoolData.activated == false)
                    AddLastPersistentBoolData(persistentBoolData);

            }
            catch (Exception e) { MapSyncMod.Instance.LogError($"{e.Message} \n{e.StackTrace}"); }
        }
        //花园关门战附近 如果掉入荆棘 之后将不会接受到首次的true
        private static Dictionary<PersistentBoolData, bool> LastPersistentBoolData = new();
        
        void AddLastPersistentBoolData(PersistentBoolData pbd)
        {
            if(LastPersistentBoolData.ContainsKey(pbd))
                LastPersistentBoolData[pbd] = pbd.activated;
            else
                LastPersistentBoolData.Add(pbd, pbd.activated);
        }
        private static  List<PersistentBoolData> QueuedPersistentBoolData = new();

        private void SceneManager_activeSceneChanged(Scene from, Scene to)
        {
            try
            {
                LastPersistentBoolData.Clear();
                //QueuedPersistentBoolData.Clear();
                //LastPersistentBoolData = LastPersistentBoolData.Where(x => x.sceneName != to.name).ToList(); 
                QueuedPersistentBoolData = QueuedPersistentBoolData.Where(x => x.sceneName != to.name).ToList();
                MapSyncMod.LogDebug($"BoolData.Clear");
                //SceneData.instance.persistentBoolItems.
            }
            catch (Exception e) { MapSyncMod.Instance.LogError($"{e.Message} \n{e.StackTrace}"); }
        }
        /*
        private PersistentBoolData FindLast(PersistentBoolData persistentBoolData)
        {
            foreach (var item in LastPersistentBoolData)
                if (persistentBoolData.id == item.id && persistentBoolData.sceneName == item.sceneName)
                    return item;
            return null;
        }*/
        private PersistentBoolData FindQueued(PersistentBoolData persistentBoolData)
        {
            try
            {
                foreach (var item in QueuedPersistentBoolData)
                    if (persistentBoolData.id == item.id && persistentBoolData.sceneName == item.sceneName)
                        return item;
                return null;
            }
            catch (Exception e) { MapSyncMod.Instance.LogError($"{e.Message} \n{e.StackTrace}"); }
            return null;
        }
        protected override void OnDataReceived(DataReceivedEvent dataReceivedEvent)
        {
            bool get = false;
            PersistentBoolData persistentBoolData = JsonConvert.DeserializeObject<PersistentBoolData>(dataReceivedEvent.Content);
            if (persistentBoolData != null)
            {
                if (GameManager.instance.sceneData.FindMyState(persistentBoolData)?.activated != true)
                {
                    string name = Regex.Replace(persistentBoolData.id, @"([\s(\(\)\1-9]+)$", "");

                    get = true;
                    if (BattleDatas.Contains(name) && MapSyncMod.GS.BossSync)
                    {
                        if (bossname.TryGetValue((persistentBoolData.id, persistentBoolData.sceneName), out string showname))
                            ShowItemChangerSprite(showname, dataReceivedEvent.From, null, "ShopIcons.Marker_R");
                        else
                            ShowItemChangerSprite(persistentBoolData.id, dataReceivedEvent.From, persistentBoolData.sceneName, "ShopIcons.Marker_R");
                    }
                    else if (LeverDatas.Contains(name) && MapSyncMod.GS.LeverSync)
                    {
                        if (MapSyncMod.GS.LeverDisplay)
                            ShowItemChangerSprite(persistentBoolData.id, dataReceivedEvent.From, persistentBoolData.sceneName, "ShopIcons.Marker_B");
                    }
                    else if (WallDatas.Contains(name) && MapSyncMod.GS.WallSync)
                    {
                        if (MapSyncMod.GS.WallDisplay)
                            ShowItemChangerSprite(persistentBoolData.id, dataReceivedEvent.From, persistentBoolData.sceneName, "ShopIcons.Marker_B");
                    }
                    else if (OtherDatas.Contains(name) && MapSyncMod.GS.OtherSync)
                    {
                        if (MapSyncMod.GS.OtherDisplay)
                            ShowItemChangerSprite(persistentBoolData.id, dataReceivedEvent.From, persistentBoolData.sceneName, "ShopIcons.Marker_B");
                    }
                    else if (MDatas.Contains(name))
                    {
                    }
                    else
                        get = false;

                    if (get)
                    {
                        On.SceneData.SaveMyState_PersistentBoolData -= SceneData_SaveMyState_PersistentBoolData;
                        GameManager.instance.sceneData.SaveMyState(persistentBoolData);
                        QueuedPersistentBoolData.Add(persistentBoolData);
                        On.SceneData.SaveMyState_PersistentBoolData += SceneData_SaveMyState_PersistentBoolData;
                    }
                }

                MapSyncMod.LogDebug($"SceneDataBool get {persistentBoolData.activated}-{persistentBoolData.sceneName}-{persistentBoolData.sceneName.L()}--{persistentBoolData.semiPersistent}-{persistentBoolData.id}\n" +
                    $"     form[{dataReceivedEvent.From}]");
            }
        }
        Dictionary<(string, string), string> bossname = new()
        {
            { ("Battle Scene","Mines_18"),"defeatedCrystalGuardian" },
            { ("Battle Scene","Mines_32"),"defeatedEnragedGuardian" },
            { ("Zombie Beam Miner Rematch","Mines_32"),null },
            { ("Battle Scene","Crossroads_04"),"killedGruzMother" },
            { ("Mawlek Body","Crossroads_09"),"killedMawlek" },
            { ("Battle Scene","Crossroads_09"),null },
            { ("Battle Scene","Crossroads_10"),null },//假骑士
            { ("Battle Scene","Crossroads_11_alt"),null },//巴德尔
            { ("Battle Scene","Crossroads_ShamanTemple"),null },//巴德尔
            { ("Battle Control","Ruins2_03"),"killedWatcherKnight" },
            { ("Battle Scene","Fungus3_23"),"killedTraitorLord" },
            { ("Battle Scene v2","Deepnest_33"),null },//zote2
            { ("Battle Scene","Deepnest_32"),null },//诺斯克
            { ("Battle Scene","Fungus1_21"),null },//grub
            { ("Battle Scene","Ruins_House_01"),null },//grub
            { ("Battle Scene","Room_Fungus_Shaman"),"Squit Battle" },
            { ("Battle Scene","Fungus3_39"),"Love Key Battle" },
            { ("Battle Scene","Fungus3_05"),"Garden Cornifer Battle" },
            { ("Battle Scene","Fungus3_10"),"Garden Stag Battle" },
            { ("Battle Scene","White_Palace_02"),"White Palace Battle" },
            { ("Battle Scene","Waterways_09"),"Waterways Cornifer Battle" },
            { ("Battle Scene","Ruins2_01"),"Watcher's Spire Battle" },
            { ("Battle Scene","Crossroads_22"),"Glowing Womb Battle" },

            { ("Battle Scene","Ruins2_09"), "City Vessel Fragment Battle"},
            { ("Battle Scene Ore","Abyss_17"),"Basin Palc Ore Battle" },
            { ("Battle Scene v2","Ruins1_05"),"City Toll Battle" },
            { ("Battle Scene","Ruins1_09"),"Sanctum Battle 1" },
            { ("Battle Scene v2","Ruins1_23"),"Sanctum Battle 2" },
            { ("Battle Scene v2","Ruins1_31"),"Shade Soul Battle" },
            { ("Battle Scene","Crossroads_08"),"Hot Springs Battle" },
            { ("Battle Scene v2","Fungus1_32"),"Moss Knight Battle" },
            { ("Battle Scene v2","Fungus2_05"),"Shrumal Ogre Battle" },
        };

        List<string> BattleDatas = new List<string>{
            "Battle Scene",
            "Battle Scene v",
            "Battle Scene Ore",//盆地矿石战
            "Battle Control",
            "Mawlek Body",//
            "Blocker",//巴德尔
            "Zombie Beam Miner Rematch",//暴怒守卫
        };
        List<string> LeverDatas = new List<string>
        {
            "Gate Switch",
            "Toll Gate Switch",
            "Toll Gate Machine",
            "Ruins Lever",
            "Mantis Lever",
            "Mines Lever",
            "Mines Lever New",
            "Gate Mantis",
            "Ruins Lever Remade",
            "Waterways_Crank_Lever",
            "White Palace Orb Lever",
            "WP Lever",
            "Bone Gate",
            "infected_door",
        };
        List<string> WallDatas = new List<string>
        {
            "Break Wall",
            "Breakable Wall",
            "Breakable Wall Waterways",
            "Breakable Wall_Silhouette",
            "One Way Wall",
            "Breakable Wall Ruin Lift",
            "Breakable Wall grimm",
            "Breakable Wall top",
            "Hive Break Wall",
            "break_wall_masksa",
            "break_wall_masks",
        };
        List<string> OtherDatas = new List<string>{
            "Break Floor",
            "Quake Floor",
            "Quake Floor Glass",
            "mine_1_quake_floor",
            "Raising Pillar",
            "Vine Platform",
            "Chain Platform",
            "Hive Breakable Pillar",
            "Collapser Small",
            "Garden Slide Floor",
            "Resting Grounds Slide Floor",
            "Fungus Break Floor",
            "Collapser Tute",

            "Door Destroyer",
            "Door",
            "Tute Door",
            "Chest",//巴德尔箱子
            //破墙后的阴影 无法消除
            /*
            [INFO]:[MapSyncMod] - ------Remasker
            [INFO]:[MapSyncMod] - ------Inverse Remasker
            [INFO]:[MapSyncMod] - ------wish inverse remask
            [INFO]:[MapSyncMod] - ------wish_remask
            [INFO]:[MapSyncMod] - ------remask
            [INFO]:[MapSyncMod] - ------boss_floor_remasker
            [INFO]:[MapSyncMod] -   True-Soul Totem-Soul_Totem-Mask_Maker
            [INFO]:[MapSyncMod] - ------hack jump secret remask
            [INFO]:[MapSyncMod] - ------inver remask_below second corridor
            [INFO]:[MapSyncMod] - ------inver remask_above boss encounter
            [INFO]:[MapSyncMod] - ------inver remask_above first corridor
            [INFO]:[MapSyncMod] - ------tram_inverse mask
            [INFO]:[MapSyncMod] - ------mask tram left front
             */
        };

        List<string> MDatas = new List<string>
        {
            "Secret Mask",
            "mask_",//巴德尔阴影
            "Mask Bottom",
            "Inverse Remasker",
            //防止同场景覆盖
            "Toll Machine Bench",
            "Hive Bench"
        };
        }
}
