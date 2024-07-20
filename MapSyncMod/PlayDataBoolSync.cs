using MapChanger;
using Modding;
using MultiWorldLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MapSyncMod
{
    public class PlayDataBoolSync:BaseSync
    {
        public PlayDataBoolSync() : base("MapSyncMod-PlayDataBool") { }
        protected override void OnEnterGame()
        {
            On.PlayerData.SetBool += PlayerData_SetBool;
        }
        protected override void OnQuitToMenu()
        {
            On.PlayerData.SetBool -= PlayerData_SetBool;
        }
        private void PlayerData_SetBool(On.PlayerData.orig_SetBool orig, PlayerData self, string boolName, bool value)
        {
            bool send = false;
            try
            {
                if (value == true && PlayerData.instance?.GetBool(boolName) == false)
                {
                    send = true;

                    if (BossDatas.Contains(boolName) && MapSyncMod.GS.BossSync)
                    {
                        if (ShowDatas.Contains(boolName))
                            ShowItemChangerSprite(boolName, null, null, "ShopIcons.Marker_R");
                    }
                    else if (LeverDatas.Contains(boolName) && MapSyncMod.GS.LeverSync)
                    {
                        if (MapSyncMod.GS.LeverDisplay)
                            ShowItemChangerSprite(boolName, null, null, "ShopIcons.Marker_W");
                    }
                    else if (WallDatas.Contains(boolName) && MapSyncMod.GS.WallSync)
                    {
                        if (MapSyncMod.GS.WallDisplay)
                            ShowItemChangerSprite(boolName, null, null, "ShopIcons.Marker_W");
                    }
                    else if (OtherDatas.Contains(boolName) && MapSyncMod.GS.OtherSync)
                    {
                        if (MapSyncMod.GS.OtherDisplay)
                            ShowItemChangerSprite(boolName, null, null, "ShopIcons.Marker_W");
                    }
                    else
                        send = false;
                }
            }
            catch (Exception e) { MapSyncMod.Instance.LogError($"{e.Message} \n{e.StackTrace}"); }
            orig.Invoke(self, boolName, value);
            try
            {
                if (send)
                {
                    foreach (var toPlayerId in SyncPlayers)
                    {
                        ItemSyncMod.ItemSyncMod.Connection.SendData(MESSAGE_LABEL,
                                JsonConvert.SerializeObject(boolName),
                                toPlayerId);
                        MapSyncMod.LogDebug($"send to id[{toPlayerId}] name[{ItemSyncMod.ItemSyncMod.ISSettings.GetNicknames()[toPlayerId]}]");
                    }

                    MapSyncMod.LogDebug($"sended setBool {boolName}-{value}");
                }
            }
            catch (Exception e) { MapSyncMod.Instance.LogError($"{e.Message} \n{e.StackTrace}"); }
}
        protected override void OnDataReceived(DataReceivedEvent dataReceivedEvent)
        {
            string boolName = JsonConvert.DeserializeObject<string>(dataReceivedEvent.Content);
            if (boolName != null)
            {
                if (PlayerData.instance?.GetBool(boolName) == false)
                {
                    if (BossDatas.Contains(boolName) && MapSyncMod.GS.BossSync)
                    {
                        PlayerData.instance.SetBoolInternal(boolName, true);
                        if (ShowDatas.Contains(boolName))
                            ShowItemChangerSprite(boolName, dataReceivedEvent.From, null, "ShopIcons.Marker_R");
                    }
                    else if (LeverDatas.Contains(boolName) && MapSyncMod.GS.LeverSync)
                    {
                        PlayerData.instance.SetBoolInternal(boolName, true);
                        if (MapSyncMod.GS.LeverDisplay)
                            ShowItemChangerSprite(boolName, dataReceivedEvent.From, null, "ShopIcons.Marker_W");
                    }
                    else if (WallDatas.Contains(boolName) && MapSyncMod.GS.WallSync)
                    {
                        PlayerData.instance.SetBoolInternal(boolName, true);
                        if (MapSyncMod.GS.WallDisplay)
                            ShowItemChangerSprite(boolName, dataReceivedEvent.From, null, "ShopIcons.Marker_W");
                    }
                    else if (OtherDatas.Contains(boolName) && MapSyncMod.GS.OtherSync)
                    {
                        PlayerData.instance.SetBoolInternal(boolName, true);
                        if (MapSyncMod.GS.OtherDisplay)
                            ShowItemChangerSprite(boolName, dataReceivedEvent.From, null, "ShopIcons.Marker_W");
                    }
                    if (boolName == nameof(PlayerData.slyRescued))
                        ItemChanger.ItemChangerMod.Modules.Get<ItemChanger.Modules.SlyRescuedEvent>().SlyRescued = true;
                    if (boolName == nameof(PlayerData.foughtGrimm) && MapSyncMod.GS.GrimmChildLevelSync)
                    {
                        if (PlayerData.instance.grimmChildLevel<3)
                        {
                            PlayerData.instance.SetInt(nameof(PlayerData.grimmChildLevel), 3);
                        }
                    }
                }
                MapSyncMod.LogDebug($"PlayDataBool get [{boolName}]     form[{dataReceivedEvent.From}]");
            }
        }

        List<string> ShowDatas = new List<string> {
                nameof(PlayerData.troupeInTown),//
                nameof(PlayerData.brettaRescued),//bretta
                nameof(PlayerData.zoteRescuedBuzzer),//佐特1
                nameof(PlayerData.zoteRescuedDeepnest),
                nameof(PlayerData.zoteDefeated),
                nameof(PlayerData.falseKnightDefeated),
                nameof(PlayerData.megaMossChargerDefeated),//大型冲锋
                nameof(PlayerData.defeatedMantisLords),//击败三螳螂
                nameof(PlayerData.mageLordDefeated),//灵魂大师
                nameof(PlayerData.hornet1Defeated),//大黄蜂1
                //nameof(PlayerData.defeatedMegaBeamMiner),//水晶守卫
                nameof(PlayerData.defeatedMegaJelly),//教师大水母
                nameof(PlayerData.flukeMotherDefeated),//吸虫之母
                nameof(PlayerData.defeatedDungDefender),//芬达
                nameof(PlayerData.hornetOutskirtsDefeated),//二见
                nameof(PlayerData.collectorDefeated),//收藏家
                //nameof(PlayerData.foughtGrimm),//格林6火战?
                nameof(PlayerData.killedInfectedKnight),//表哥
                nameof(PlayerData.killedHiveKnight),//蜂巢
                //nameof(PlayerData.killedMageKnight),
                //nameof(PlayerData.killedMawlek),
                nameof(PlayerData.killedMimicSpider), //诺斯克
                //nameof(PlayerData.killedBigFly), //之母
                //nameof(PlayerData.killedDummy), //守护者
                //nameof(PlayerData.killedBlackKnight), //守护者
                //nameof(PlayerData.killedTraitorLord), //叛徒
                nameof(PlayerData.killedGrimm),

                nameof(PlayerData.greyPrinceDefeated),//灰色王子
                nameof(PlayerData.mageLordDreamDefeated),//梦境大师
                nameof(PlayerData.infectedKnightDreamDefeated),//梦表
                nameof(PlayerData.falseKnightDreamDefeated),//失败冠军
                nameof(PlayerData.whiteDefenderDefeated),//白芬达
                nameof(PlayerData.killedNightmareGrimm),//王格林
                nameof(PlayerData.colosseumBronzeCompleted),//竞技场1
                nameof(PlayerData.colosseumSilverCompleted),//2
                nameof(PlayerData.colosseumGoldCompleted),//3
                //nameof(PlayerData.defeatedDoubleBlockers),//双巴  没什么效果
            };
#if DEBUG
        public static
#endif
        List<string> BossDatas = new List<string>
        {
                nameof(PlayerData.nightmareLanternAppeared),//梦魇出现
                nameof(PlayerData.nightmareLanternLit),//梦魇灯
                nameof(PlayerData.troupeInTown),//
                nameof(PlayerData.divineInTown),//
                nameof(PlayerData.brettaRescued),//bretta
                nameof(PlayerData.zoteRescuedBuzzer),//佐特1
                nameof(PlayerData.zoteRescuedDeepnest),
                nameof(PlayerData.zoteDefeated),
                nameof(PlayerData.falseKnightDefeated),
                nameof(PlayerData.megaMossChargerDefeated),//大型冲锋
                nameof(PlayerData.killedMegaMossCharger),//草团子的神居
                nameof(PlayerData.defeatedMantisLords),//击败三螳螂
                nameof(PlayerData.mageLordDefeated),//灵魂大师
                nameof(PlayerData.hornet1Defeated),//大黄蜂1
                nameof(PlayerData.defeatedMegaBeamMiner),//水晶守卫
                nameof(PlayerData.defeatedMegaJelly),//教师大水母
                nameof(PlayerData.flukeMotherDefeated),//吸虫之母
                nameof(PlayerData.defeatedDungDefender),//芬达
                nameof(PlayerData.hornetOutskirtsDefeated),//二见
                nameof(PlayerData.collectorDefeated),//收藏家
                nameof(PlayerData.foughtGrimm),//格林6火战?
                nameof(PlayerData.killedInfectedKnight),//表哥
                nameof(PlayerData.killedHiveKnight),//蜂巢
                nameof(PlayerData.killedMageKnight),
                nameof(PlayerData.killedMawlek),
                nameof(PlayerData.killedMimicSpider), //诺斯克
                nameof(PlayerData.killedHeavyMantis), //叛徒?
                nameof(PlayerData.killedBigFly), //之母
                nameof(PlayerData.killedDummy), //守护者
                nameof(PlayerData.killedBlackKnight), //守护者
                nameof(PlayerData.killedTraitorLord), //叛徒
                nameof(PlayerData.killedGrimm),
                nameof(PlayerData.killedGhostAladar),
                nameof(PlayerData.killedGhostGalien),
                nameof(PlayerData.killedGhostHu),
                nameof(PlayerData.killedGhostMarkoth),
                nameof(PlayerData.killedGhostMarmu),
                nameof(PlayerData.killedGhostNoEyes),
                nameof(PlayerData.killedGhostXero),

                nameof(PlayerData.greyPrinceDefeated),//灰色王子
                nameof(PlayerData.mageLordDreamDefeated),//梦境大师
                nameof(PlayerData.infectedKnightDreamDefeated),//梦表
                nameof(PlayerData.falseKnightDreamDefeated),//失败冠军
                nameof(PlayerData.whiteDefenderDefeated),//白芬达
                nameof(PlayerData.defeatedNightmareGrimm),//王格林
                nameof(PlayerData.killedNightmareGrimm),
                nameof(PlayerData.colosseumBronzeCompleted),//竞技场1
                nameof(PlayerData.colosseumSilverCompleted),//2
                nameof(PlayerData.colosseumGoldCompleted),//3
                nameof(PlayerData.defeatedDoubleBlockers),//双巴  没什么效果
                nameof(PlayerData.falseKnightOrbsCollected),//
                nameof(PlayerData.greyPrinceOrbsCollected),//
                nameof(PlayerData.infectedKnightOrbsCollected),//
                nameof(PlayerData.mageLordOrbsCollected),//
                nameof(PlayerData.whiteDefenderOrbsCollected),//
                nameof(PlayerData.bossDoorCageUnlocked),//
                nameof(PlayerData.killedNailBros),//
                nameof(PlayerData.killedPaintmaster),//
                nameof(PlayerData.killedNailsage),//
                nameof(PlayerData.killedPaleLurker),//
                nameof(PlayerData.killedLobsterLancer),
        };
        List<string> LeverDatas = new List<string>
        {
                nameof(PlayerData.mineLiftOpened),//德特茅斯右上拉杆
                nameof(PlayerData.shamanPillar),//祖先山丘拉杆
                nameof(PlayerData.openedTownBuilding),//泥口站
                nameof(PlayerData.cityBridge1),//泪城大门拉杆
                nameof(PlayerData.cityBridge2),
                nameof(PlayerData.cityLift1),//城市仓库左电梯
                nameof(PlayerData.openedWaterwaysManhole),//打开下水道
                nameof(PlayerData.waterwaysGate),//下水道左门
                nameof(PlayerData.waterwaysAcidDrained),//下水道酸水管道
                nameof(PlayerData.city2_sewerDoor),//酸水上泪城的门
                nameof(PlayerData.abyssLighthouse),//灯塔开关
                nameof(PlayerData.openedRestingGrounds02),//泽若下
                nameof(PlayerData.deepnest26b_switch),//废弃电车回去拉杆
                nameof(PlayerData.whitePalaceOrb_1),//白宫电梯开关1
                nameof(PlayerData.whitePalaceOrb_2),//白宫电梯开关2
                nameof(PlayerData.whitePalaceOrb_3),//白宫电梯开关3
                nameof(PlayerData.whitePalace05_lever),//白宫开关1
                nameof(PlayerData.openedGardensStagStation),//王后花园车站大门
        };
        List<string> WallDatas = new List<string>
        {
                nameof(PlayerData.falseKnightWallBroken),//假骑士的墙
                nameof(PlayerData.crossroadsMawlekWall),//电饭煲的墙
                nameof(PlayerData.deepnestWall),//落穴右墙
                nameof(PlayerData.oneWayArchive),// 档案馆 单向墙
                nameof(PlayerData.dungDefenderWallBroken),//芬达墙
                nameof(PlayerData.brokeMinersWall),//深聚墙
                nameof(PlayerData.outskirtsWall),//蜂巢边境墙
                nameof(PlayerData.bathHouseWall),//欢乐之屋墙
                nameof(PlayerData.restingGroundsCryptWall),//安息电梯墙
        };
        List<string> OtherDatas = new List<string>{
                nameof(PlayerData.slyRescued),//斯莱 openedSlyShop    
                nameof(PlayerData.paidLegEater),//食腿86
                nameof(PlayerData.hornetFountainEncounter),//泪城大黄蜂相遇
                nameof(PlayerData.brokenMageWindow),
                nameof(PlayerData.brokenMageWindowGlass),
                nameof(PlayerData.abyssGateOpened),//王印大门
                //nameof(PlayerData.blueVineDoor),//蓝血大门     随机下无效
                nameof(PlayerData.watcherChandelier),//6丸子吊灯
                //nameof(PlayerData.lurienDefeated),//守梦者守望者? maskBrokenLurien? 无效
                //nameof(PlayerData.monomonDefeated),//[INFO]:[MapSyncMod] - monomonDefeated-True    [INFO]:[MapSyncMod] - maskBrokenMonomon-True
                //nameof(PlayerData.hegemolDefeated),//[INFO]:[MapSyncMod] - hegemolDefeated-True    [INFO]:[MapSyncMod] - maskBrokenHegemol-True
                nameof(PlayerData.deepnestBridgeCollapsed),//王后驿站下深巢落穴 steppedBeyondBridge
                nameof(PlayerData.gladeDoorOpened),//安息↗大门
                nameof(PlayerData.dreamReward2),//安息↗大门 先知奖励

                nameof(PlayerData.jijiDoorUnlocked),//jiji
                nameof(PlayerData.openedMageDoor_v2),//典雅门
                nameof(PlayerData.openedMageDoor),//??门
                nameof(PlayerData.openedLoveDoor),//心
                
                nameof(PlayerData.whitePalaceMidWarp),//白宫中庭传送(进入)
                //nameof(PlayerData.openedBlackEggPath),//染黑开口 随机下无效
                nameof(PlayerData.bathHouseOpened),//欢乐之屋门
                nameof(PlayerData.godseekerUnlocked),//寻神者解锁?
                
                nameof(PlayerData.marmOutsideConvo),//古董商?
                //nameof(PlayerData.metRelicDealerShop),//古董商?
                
                //nameof(PlayerData.killedOblobble), //无效

                //花园科尼法战 花园下面关门 没有  已修复LastPersistentBoolData   原因 场景内落到刺上 之后会无法收到首次SaveMyState的true
                };
    }
}
/*
nameof(PlayerData.killedAbyssCrawler),
nameof(PlayerData.killedAbyssTendril),
nameof(PlayerData.killedAcidFlyer),
nameof(PlayerData.killedAcidWalker),
nameof(PlayerData.killedAngryBuzzer),
nameof(PlayerData.killedBabyCentipede),
nameof(PlayerData.killedBeamMiner),
nameof(PlayerData.killedBeeHatchling),
nameof(PlayerData.killedBeeStinger),
nameof(PlayerData.killedBigBee),
nameof(PlayerData.killedBigBuzzer),
nameof(PlayerData.killedBigCentipede),
nameof(PlayerData.killedBigFly),
nameof(PlayerData.killedBindingSeal),
nameof(PlayerData.killedBlackKnight),
nameof(PlayerData.killedBlobble),
nameof(PlayerData.killedBlobFlyer),
nameof(PlayerData.killedBlocker),
nameof(PlayerData.killedBlowFly),
nameof(PlayerData.killedBouncer),
nameof(PlayerData.killedBurstingBouncer),
nameof(PlayerData.killedBurstingZombie),
nameof(PlayerData.killedBuzzer),
nameof(PlayerData.killedCeilingDropper),
nameof(PlayerData.killedCentipedeHatcher),
nameof(PlayerData.killedClimber),
nameof(PlayerData.killedColFlyingSentry),
nameof(PlayerData.killedColHopper),
nameof(PlayerData.killedColMiner),
nameof(PlayerData.killedColMosquito),
nameof(PlayerData.killedColRoller),
nameof(PlayerData.killedColShield),
nameof(PlayerData.killedColWorm),
nameof(PlayerData.killedCrawler),
nameof(PlayerData.killedCrystalCrawler),
nameof(PlayerData.killedCrystalFlyer),
nameof(PlayerData.killedDreamGuard),
nameof(PlayerData.killedDummy),
nameof(PlayerData.killedDungDefender),
nameof(PlayerData.killedEggSac),
nameof(PlayerData.killedElectricMage),
nameof(PlayerData.killedFalseKnight),
nameof(PlayerData.killedFatFluke),
nameof(PlayerData.killedFinalBoss),
nameof(PlayerData.killedFlameBearerLarge),
nameof(PlayerData.killedFlameBearerMed),
nameof(PlayerData.killedFlameBearerSmall),
nameof(PlayerData.killedFlipHopper),
nameof(PlayerData.killedFlukefly),
nameof(PlayerData.killedFlukeman),
nameof(PlayerData.killedFlukeMother),
nameof(PlayerData.killedFlyingSentryJavelin),
nameof(PlayerData.killedFlyingSentrySword),
nameof(PlayerData.killedFungCrawler),
nameof(PlayerData.killedFungifiedZombie),
nameof(PlayerData.killedFungoonBaby),
nameof(PlayerData.killedFungusFlyer),
nameof(PlayerData.killedGardenZombie),
nameof(PlayerData.killedGhostAladar),
nameof(PlayerData.killedGhostGalien),
nameof(PlayerData.killedGhostHu),
nameof(PlayerData.killedGhostMarkoth),
nameof(PlayerData.killedGhostMarmu),
nameof(PlayerData.killedGhostNoEyes),
nameof(PlayerData.killedGhostXero),
nameof(PlayerData.killedGiantHopper),
nameof(PlayerData.killedGodseekerMask),
nameof(PlayerData.killedGorgeousHusk),
nameof(PlayerData.killedGrassHopper),
nameof(PlayerData.killedGreatShieldZombie),
nameof(PlayerData.killedGreyPrince),
nameof(PlayerData.killedGrimm),
nameof(PlayerData.killedGrubMimic),
nameof(PlayerData.killedHatcher),
nameof(PlayerData.killedHatchling),
nameof(PlayerData.killedHealthScuttler),
nameof(PlayerData.killedHeavyMantis),
nameof(PlayerData.killedHiveKnight),
nameof(PlayerData.killedHollowKnight),
nameof(PlayerData.killedHollowKnightPrime),
nameof(PlayerData.killedHopper),
nameof(PlayerData.killedHornet),
nameof(PlayerData.killedHunterMark),
nameof(PlayerData.killedInfectedKnight),
nameof(PlayerData.killedInflater),
nameof(PlayerData.killedJarCollector),
nameof(PlayerData.killedJellyCrawler),
nameof(PlayerData.killedJellyfish),
nameof(PlayerData.killedLaserBug),
nameof(PlayerData.killedLazyFlyer),
nameof(PlayerData.killedLesserMawlek),
nameof(PlayerData.killedLobsterLancer),
nameof(PlayerData.killedMage),
nameof(PlayerData.killedMageBalloon),
nameof(PlayerData.killedMageBlob),
nameof(PlayerData.killedMageKnight),
nameof(PlayerData.killedMageLord),
nameof(PlayerData.killedMantis),
nameof(PlayerData.killedMantisFlyerChild),
nameof(PlayerData.killedMantisHeavyFlyer),
nameof(PlayerData.killedMantisLord),
nameof(PlayerData.killedMawlek),
nameof(PlayerData.killedMawlekTurret),
nameof(PlayerData.killedMegaBeamMiner),
nameof(PlayerData.killedMegaJellyfish),
nameof(PlayerData.killedMegaMossCharger),
nameof(PlayerData.killedMenderBug),
nameof(PlayerData.killedMimicSpider),
nameof(PlayerData.killedMinesCrawler),
nameof(PlayerData.killedMiniSpider),
nameof(PlayerData.killedMosquito),
nameof(PlayerData.killedMossCharger),
nameof(PlayerData.killedMossFlyer),
nameof(PlayerData.killedMossKnight),
nameof(PlayerData.killedMossKnightFat),
nameof(PlayerData.killedMossmanRunner),
nameof(PlayerData.killedMossmanShaker),
nameof(PlayerData.killedMossWalker),
nameof(PlayerData.killedMummy),
nameof(PlayerData.killedMushroomBaby),
nameof(PlayerData.killedMushroomBrawler),
nameof(PlayerData.killedMushroomRoller),
nameof(PlayerData.killedMushroomTurret),
nameof(PlayerData.killedNailBros),
nameof(PlayerData.killedNailsage),
nameof(PlayerData.killedNightmareGrimm),
nameof(PlayerData.killedOblobble),
nameof(PlayerData.killedOrangeBalloon),
nameof(PlayerData.killedOrangeScuttler),
nameof(PlayerData.killedPaintmaster),
nameof(PlayerData.killedPalaceFly),
nameof(PlayerData.killedPaleLurker),
nameof(PlayerData.killedPigeon),
nameof(PlayerData.killedPlantShooter),
nameof(PlayerData.killedPrayerSlug),
nameof(PlayerData.killedRoller),
nameof(PlayerData.killedRoyalCoward),
nameof(PlayerData.killedRoyalDandy),
nameof(PlayerData.killedRoyalGuard),
nameof(PlayerData.killedRoyalPlumper),
nameof(PlayerData.killedSentry),
nameof(PlayerData.killedSentryFat),
nameof(PlayerData.killedShootSpider),
nameof(PlayerData.killedSibling),
nameof(PlayerData.killedSlashSpider),
nameof(PlayerData.killedSnapperTrap),
nameof(PlayerData.killedSpiderCorpse),
nameof(PlayerData.killedSpiderFlyer),
nameof(PlayerData.killedSpitter),
nameof(PlayerData.killedSpittingZombie),
nameof(PlayerData.killedSuperSpitter),
nameof(PlayerData.killedTraitorLord),
nameof(PlayerData.killedVoidIdol_1),
nameof(PlayerData.killedVoidIdol_2),
nameof(PlayerData.killedVoidIdol_3),
nameof(PlayerData.killedWhiteDefender),
nameof(PlayerData.killedWhiteRoyal),
nameof(PlayerData.killedWorm),
nameof(PlayerData.killedZapBug),
nameof(PlayerData.killedZombieBarger),
nameof(PlayerData.killedZombieGuard),
nameof(PlayerData.killedZombieHive),
nameof(PlayerData.killedZombieHornhead),
nameof(PlayerData.killedZombieLeaper),
nameof(PlayerData.killedZombieMiner),
nameof(PlayerData.killedZombieRunner),
nameof(PlayerData.killedZombieShield),
nameof(PlayerData.killedZote),
nameof(PlayerData.killedZotelingBalloon),
nameof(PlayerData.killedZotelingBuzzer),
nameof(PlayerData.killedZotelingHopper)
*/
