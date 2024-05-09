using Benchwarp;
using ItemChanger;
using ItemChanger.Modules;
using MultiWorldLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using static tk2dSpriteCollectionDefinition;

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

            if (Interop.HasRecentItemsDisplay())
                RecentItemsDisplay.ItemDisplayMethods.ShowItemInternal(new ItemChanger.UIDefs.MsgUIDef() { sprite = new ItemChangerSprite("ShopIcons.Marker_B") },
                    $"{"Other Sync".L()} {(MapSyncMod.GS.OtherSync ? "Enabled".L() : "Disabled".L())}");
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
            bool send = false;
            //LastPersistentBoolData.TryGetValue((persistentBoolData.sceneName, persistentBoolData.id))

            if (persistentBoolData?.activated == true)
                if (GameManager.instance.sceneData.FindMyState(persistentBoolData)?.activated != true
                    || (LastPersistentBoolData.ContainsKey(persistentBoolData) && LastPersistentBoolData[persistentBoolData] == false))
                        send = true;
            if (FindQueued(persistentBoolData) == null)
                orig.Invoke(self, persistentBoolData);
            //else
            //    send = false;
            //if (GameManager.instance.sceneData.FindMyState(persistentBoolData)?.activated != true)
            //    MapSyncMod.LogDebug($"------{Regex.Replace(persistentBoolData.id, @"([\s(\(\)\1-9]+)$", "")}");

            if (send && MapSyncMod.GS.OtherSync)
            {
                if (!SceneDatas.Contains(Regex.Replace(persistentBoolData.id, @"([\s(\(\)\1-9]+)$", ""))) return;
                if (ItemSyncMod.ItemSyncMod.Connection?.IsConnected() != true) return;
                foreach (var toPlayerId in SyncPlayers)
                {
                    ItemSyncMod.ItemSyncMod.Connection.SendData(MESSAGE_LABEL,
                            JsonConvert.SerializeObject(persistentBoolData),
                            toPlayerId);
                    MapSyncMod.LogDebug($"send to id[{toPlayerId}] name[{ItemSyncMod.ItemSyncMod.ISSettings.GetNicknames()[toPlayerId]}]");
                }
                if (Interop.HasRecentItemsDisplay() && MapSyncMod.GS.OtherDisplay)
                    RecentItemsDisplay.Export.ShowItemChangerSprite($"{persistentBoolData.id}\n from {persistentBoolData.sceneName.BL()}", "ShopIcons.Marker_B");

                MapSyncMod.LogDebug($"sended SceneDataBool {persistentBoolData.activated}-{persistentBoolData.sceneName.BL()}--{persistentBoolData.semiPersistent}-{persistentBoolData.id}");
            }
            //else
            //MapSyncMod.LogDebug($"no send BoolData {persistentBoolData.activated}-{persistentBoolData.sceneName}--{persistentBoolData.semiPersistent}-{persistentBoolData.id}");
            MapSyncMod.LogDebug($"AddLastPersistentBoolData {persistentBoolData.activated}-{persistentBoolData.sceneName.BL()}--{persistentBoolData.semiPersistent}-{persistentBoolData.id} find {LastPersistentBoolData.ContainsKey(persistentBoolData)}");
            //AddLastPersistentBoolData(persistentBoolData);
            if (persistentBoolData.activated == false)
                AddLastPersistentBoolData(persistentBoolData);

        }
        //花园关门战附近 如果掉入荆棘 之后将不会接受到首次的true
        private static Dictionary<PersistentBoolData, bool> LastPersistentBoolData = new();
        
        void AddLastPersistentBoolData(PersistentBoolData pbd)
        {
            if(LastPersistentBoolData.ContainsKey(pbd))
                LastPersistentBoolData[pbd] = false;
            else
                LastPersistentBoolData.Add(pbd, false);
        }
        private static  List<PersistentBoolData> QueuedPersistentBoolData = new();

        private void SceneManager_activeSceneChanged(Scene from, Scene to)
        {
            LastPersistentBoolData.Clear();
            //QueuedPersistentBoolData.Clear();
            //LastPersistentBoolData = LastPersistentBoolData.Where(x => x.sceneName != to.name).ToList(); 
            QueuedPersistentBoolData = QueuedPersistentBoolData.Where(x=>x.sceneName!= to.name).ToList();
            MapSyncMod.LogDebug($"BoolData.Clear");
            //SceneData.instance.persistentBoolItems.
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
            foreach (var item in QueuedPersistentBoolData)
                if (persistentBoolData.id == item.id && persistentBoolData.sceneName == item.sceneName)
                    return item;
            return null;
        }
        protected override void OnDataReceived(DataReceivedEvent dataReceivedEvent)
        {
            if (!MapSyncMod.GS.OtherSync) return;
            PersistentBoolData persistentBoolData = JsonConvert.DeserializeObject<PersistentBoolData>(dataReceivedEvent.Content);
            if (persistentBoolData != null)
                if (GameManager.instance.sceneData.FindMyState(persistentBoolData)?.activated != true)
                    if (SceneDatas.Contains(Regex.Replace(persistentBoolData.id, @"([\s(\(\)\1-9]+)$", "")))
                    {
                        On.SceneData.SaveMyState_PersistentBoolData -= SceneData_SaveMyState_PersistentBoolData;
                        //Modules/ TransitionFixes
                        //ItemChanger.Util.SceneDataUtil.SavePersistentBoolItemState(persistentBoolData);
                        GameManager.instance.sceneData.SaveMyState(persistentBoolData);
                        QueuedPersistentBoolData.Add(persistentBoolData);
                        On.SceneData.SaveMyState_PersistentBoolData += SceneData_SaveMyState_PersistentBoolData;
                    }
            MapSyncMod.LogDebug($"SceneDataBool get {persistentBoolData.activated}-{persistentBoolData.sceneName.BL()}--{persistentBoolData.semiPersistent}-{persistentBoolData.id}\n" +
                $"     form[{dataReceivedEvent.From}]");
            if (Interop.HasRecentItemsDisplay() && MapSyncMod.GS.OtherDisplay)
                RecentItemsDisplay.Export.ShowItemChangerSprite($"{persistentBoolData.id}\n from {dataReceivedEvent.From} in {persistentBoolData.sceneName.BL()}", "ShopIcons.Marker_B");

        }
        List<string> SceneDatas = new List<string>{
            "Battle Scene",
            "Battle Scene v",
            "Battle Scene Ore",//盆地矿石战
            "Battle Control",
            "Break Floor",
            "Quake Floor",
            "Quake Floor Glass",
            "mine_1_quake_floor",
            "Break Wall",
            "Breakable Wall",
            "Breakable Wall Waterways",
            "Breakable Wall_Silhouette",
            "One Way Wall",
            "Breakable Wall Ruin Lift",
            "Gate Switch",
            "Toll Gate Switch",
            "Toll Gate Machine",
            "Raising Pillar",
            "Vine Platform",
            "Chain Platform",
            "Hive Breakable Pillar",
            "Ruins Lever",
            "Mantis Lever",
            "Mines Lever",
            "Mines Lever New",
            "Gate Mantis",
            "Tute Door",
            "Collapser Small",
            "Ruins Lever Remade",
            "Waterways_Crank_Lever",
            "White Palace Orb Lever",
            "WP Lever",
            //"break_wall_masks",
            "Breakable Wall grimm",
            "Breakable Wall top",
            "Hive Break Wall",
            "Garden Slide Floor",
            "Resting Grounds Slide Floor",
            "Fungus Break Floor",
            //"boss_floor_remasker",
            "Bone Gate",
            //"Gate Mantis",
            "infected_door",
            "Collapser Tute",
            "Door Destroyer",
            "Door",

            "Mawlek Body",//
            "Blocker",//巴德尔?
            "Chest",//巴德尔箱子
            "mask_",//巴德尔阴影
            "Zombie Beam Miner Rematch",//暴怒守卫
            //破墙后的阴影 无法消除
            "break_wall_masksa",
            "Secret Mask",
            "break_wall_masks",
            "Inverse Remasker",
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
            //防止同场景覆盖
            "Toll Machine Bench",
            "Hive Bench"
        };
        /*
         * 
				"id": "boss_floor_remasker",
				"sceneName": "Ruins2_03",

				"id": "Bone Gate",
				"sceneName": "Crossroads_ShamanTemple",

        PersistentBoolData False-Crossroads_08
[INFO]:[MapSyncMod] -   False-break_wall_masksa



        失败
        电饭煲不关门 但人在
[INFO]:[MapSyncMod] - PersistentBoolData True-Crossroads_09
[INFO]:[MapSyncMod] -   False-Mawlek Body


        
        水晶守卫
            defeatedMegaBeamMiner
        梦钉场景?
            enteredDreamWorld

        
        playdata int ghost
        nameof(PlayerData.xeroDefeated),
        nameof(PlayerData.noEyesDefeated),
        nameof(PlayerData.elderHuDefeated),
        nameof(PlayerData.markothDefeated),
        nameof(PlayerData.galienDefeated),
        nameof(PlayerData.mumCaterpillarDefeated),
        nameof(PlayerData.aladarSlugDefeated),

        openedRestingGrounds02?
        竞技场

        //未发现参数
        斯莱
        表哥
        蜂巢骑士
        竞技场右 潜伏者 
[INFO]:[MapSyncMod] - send to id[1] name[2]
[INFO]:[MapSyncMod] - send[GG_Lurker]
[INFO]:[MapSyncMod] - disablePause-False
[INFO]:[MapSyncMod] - disablePause-False
[INFO]:[MapSyncMod] - hazardRespawnFacingRight-True
[INFO]:[MapSyncMod] - isInvincible-False
[INFO]:[MapSyncMod] - hazardRespawnFacingRight-True
[INFO]:[MapSyncMod] - hazardRespawnFacingRight-True
[INFO]:[MapSyncMod] - hazardRespawnFacingRight-True
[INFO]:[MapSyncMod] - hazardRespawnFacingRight-True
[INFO]:[MapSyncMod] - hazardRespawnFacingRight-True
[INFO]:[MapSyncMod] - killedPaleLurker-True
[INFO]:[MapSyncMod] - newDataPaleLurker-True
[INFO]:[MapSyncMod] - disablePause-True
[INFO]:[MapSyncMod] - disablePause-False
[INFO]:[MapSyncMod] - hazardRespawnFacingRight-False
[INFO]:[MapSyncMod] - hazardRespawnFacingRight-True
[INFO]:[MapSyncMod] - PersistentBoolData False-GG_Lurker
[INFO]:[MapSyncMod] -   False-Secret Mask (1)
[INFO]:[MapSyncMod] - ------Secret Mask
[INFO]:[MapSyncMod] - PersistentBoolData False-GG_Lurker
[INFO]:[MapSyncMod] -   False-Secret Mask
[INFO]:[MapSyncMod] - ------Secret Mask
[INFO]:[MapSyncMod] - PersistentBoolData False-GG_Lurker
[INFO]:[MapSyncMod] -   False-Shiny Item
[INFO]:[MapSyncMod] - ------Shiny Item
[INFO]:[MapSyncMod] - PersistentIntData 0-GG_Lurker
[INFO]:[MapSyncMod] -   True-Soul Totem-Soul_Totem-Pale_Lurker
[INFO]:[MapSyncMod] - PersistentBoolData True-GG_Lurker
[INFO]:[MapSyncMod] -   False-Shiny Item-Simple_Key-Lurker
[INFO]:[MapSyncMod] - ------Shiny Item-Simple_Key-Lurker
[INFO]:[MapSyncMod] - isInvincible-True
        */
    }
}
