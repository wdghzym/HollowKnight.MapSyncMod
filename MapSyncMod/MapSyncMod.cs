using Benchwarp;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger;
using ItemSyncMod.SyncFeatures.TransitionsFoundSync;
using MapChanger;
using MapChanger.UI;
using Modding;
using MultiWorldLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Profiling.Memory.Experimental;
using UnityEngine.SceneManagement;
using static MultiWorldLib.ExportedAPI.ExportedExtensionsMenuAPI;
using static tk2dSpriteCollectionDefinition;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.GridBrushBase;
using static UnityEngine.UI.SaveSlotButton;

namespace MapSyncMod
{
    public class MapSyncMod : Mod, IGlobalSettings<GlobalSetting>, IMenuMod
    {
        public static MapSyncMod Instance;
        public static GlobalSetting GS = new();
        public MapSync MapSync;
        public BenchSync BenchSync;
        public PlayDataBoolSync PlayDataBoolSync;
        public PlayDataIntSync PlayDataIntSync;
        public SceneDataBoolSync SceneDataBoolSync;
        public BossDoorSync BossDoorSync;
        internal MapSyncModExtension mapSyncModExtension;


        public MapSyncMod() => Instance = this;


        public override string GetVersion() 
        {
            var version = GetType().Assembly.GetName().Version.ToString();
#if DEBUG
            version += "-debug";
#endif
            return version;
        }

        public override int LoadPriority() => 5;

        public override void Initialize()
        {
            base.Initialize();
            Interop.FindInteropMods();
            Localization.HookLocalization();

            MapSync = new MapSync();
            BenchSync = new BenchSync();
            PlayDataBoolSync = new PlayDataBoolSync();
            PlayDataIntSync = new PlayDataIntSync();
            SceneDataBoolSync = new SceneDataBoolSync();
            BossDoorSync = new BossDoorSync();
            mapSyncModExtension = new MapSyncModExtension();
            //On.GameManager.LoadGame += GameManager_LoadGame;
            //On.GameManager.ReturnToMainMenu += GameManager_ReturnToMainMenu;
            MapChanger.Events.OnEnterGame += OnEnterGame;
            MapChanger.Events.OnQuitToMenu += OnQuitToMenu;
            /*
            */
            On.SceneData.SaveMyState_PersistentBoolData += SceneData_SaveMyState_PersistentBoolData;
            On.SceneData.SaveMyState_PersistentIntData += SceneData_SaveMyState_PersistentIntData;
            On.PlayerData.SetBool += PlayerData_SetBool;
            //On.PlayerData.SetInt += PlayerData_SetInt;
            
            //On.PlayerData
        }

        private void PlayerData_SetInt(On.PlayerData.orig_SetInt orig, PlayerData self, string intName, int value)
        {
            switch (intName)
            {
                case "journalNotesCompleted":
                case "journalEntriesTotal":
                case "previousDarkness":
                case "geo":
                case "MPCharge":
                case "":
                case "journalEntriesCompleted":
                    break;
                default:
                    if (PlayerData.instance.GetInt(intName) != value)
                        ShowMessage($"{intName}-{value}");
                    break;
            }
            orig.Invoke(self, intName, value);
        }

        private void PlayerData_SetBool(On.PlayerData.orig_SetBool orig, PlayerData self, string boolName, bool value)
        {
            switch (boolName)
            {
                case "isInvincible":
                case "disablePause":
                case "atBench":
                //case "":
                case "hazardRespawnFacingRight":
                    break;
                default:
                    if (!PlayerData.instance.GetBool(boolName) && value)
                        ShowMessage($"{boolName}-{value}");
                    break;
            }
            orig.Invoke(self, boolName, value);
        }
        /* 
        */
        private void SceneData_SaveMyState_PersistentBoolData(On.SceneData.orig_SaveMyState_PersistentBoolData orig, SceneData self, PersistentBoolData persistentBoolData)
        {
            //if (persistentBoolData?.activated == true)
                //if (GameManager.instance.sceneData.FindMyState(persistentBoolData)?.activated != true)
                    //if (GameManager.instance.sceneData.FindMyState(persistentBoolData) is not null)
                    //    if (GameManager.instance.sceneData.FindMyState(persistentBoolData).activated != persistentBoolData.activated)
                    ShowMessage($"PersistentBoolData {persistentBoolData.activated}-{persistentBoolData.sceneName.L()}\n  {persistentBoolData.semiPersistent}-{persistentBoolData.id} last{GameManager.instance.sceneData.FindMyState(persistentBoolData)?.activated}");

            orig.Invoke(self, persistentBoolData);
 }
        private void SceneData_SaveMyState_PersistentIntData(On.SceneData.orig_SaveMyState_PersistentIntData orig, SceneData self, PersistentIntData persistentIntData)
        {
            orig.Invoke(self, persistentIntData);
            //if (GameManager.instance.sceneData.FindMyState(persistentIntData) is not null)
             //   if (GameManager.instance.sceneData.FindMyState(persistentIntData).value != persistentIntData.value)
                ShowMessage($"PersistentIntData {persistentIntData.value}-{persistentIntData.sceneName.L()}\n  {persistentIntData.semiPersistent}-{persistentIntData.id}");
        }

        void ShowMessage(string message)
        {
#if DEBUG
            LogDebug(message);
            //RecentItemsDisplay.Export.ShowItemChangerSprite(message, "ShopIcons.BenchPin");
#endif
        }
        private void OnEnterGame()
        {
            MapSyncMod.LogDebug($"MapSyncMod OnEnterGame");

#if DEBUG
            //ItemSyncMod.ItemSyncMod.Connection.OnConnectedPlayersChanged += OnConnectedPlayersChanged;
            //ItemSyncMod.ItemSyncMod.ISSettings.AddSentData
            //MultiWorldLib.ExportedAPI.ExportedExtensionsMenuAPI.
            //ItemSyncMod.ItemSyncMod.Connection.SendDataToAllConnected()
            //ItemSyncMod.ItemSyncMod.Connection.  
            //ItemSyncMod.ItemSyncMod.ISSettings.AddSentData
            //On.HeroController.
            /*
            //GameObject.Find(objectName).LocateMyFSM(wallData.fsmType).SetState("BreakSameScene");
            foreach (var item in PlayerDatas)
            {
                try
                {
                    PlayerData.instance.SetBool(item, true);
                }
                catch (Exception ex) { LogDebug(item + " " + ex.Message); }
            }
            */
            //ItemChanger.Events.AddFsmEdit(sceneName, new(objectName, fsmType), ModifyWallBehaviour);

            //PlayerData.instance.SetBool(nameof(PlayerData.slyRescued), true);
            //PlayerData.instance.SetBool(nameof(PlayerData.openedSlyShop), true);
            //ItemChanger.ItemChangerMod.Modules.Get<ItemChanger.Modules.SlyRescuedEvent>().SlyRescued = true;

            //PlayerData.instance.SetBool(nameof(PlayerData.killedInfectedKnight), true);
            //PlayerData.instance.SetBool(nameof(PlayerData.killedHiveKnight), true);
            /*
            PlayerData.instance.SetBool(nameof(PlayerData.killedMawlek), true);
            PlayerData.instance.SetBool(nameof(PlayerData.killedOblobble), true);//无效
            PlayerData.instance.SetBool(nameof(PlayerData.colosseumBronzeCompleted), true);//
            PlayerData.instance.SetBool(nameof(PlayerData.colosseumGoldCompleted), true);//
            PlayerData.instance.SetBool(nameof(PlayerData.colosseumSilverCompleted), true);//

            PlayerData.instance.SetBool(nameof(PlayerData.killedMimicSpider), true);//诺斯克
            PlayerData.instance.SetBool(nameof(PlayerData.foughtGrimm), true);//无效
            PlayerData.instance.SetBool(nameof(PlayerData.killedGrimm), true);
            PlayerData.instance.SetBool(nameof(PlayerData.killedGhostAladar), true);
            PlayerData.instance.SetBool(nameof(PlayerData.killedGhostGalien), true);
            PlayerData.instance.SetBool(nameof(PlayerData.killedGhostHu), true);
            PlayerData.instance.SetBool(nameof(PlayerData.killedGhostMarkoth), true);
            PlayerData.instance.SetBool(nameof(PlayerData.killedGhostMarmu), true);
            PlayerData.instance.SetBool(nameof(PlayerData.killedGhostNoEyes), true);
            PlayerData.instance.SetBool(nameof(PlayerData.killedGhostXero), true);
            */

            /*
            PlayerData.instance.SetBool(nameof(PlayerData.killedGrimm), true);
            PlayerData.instance.SetBool(nameof(PlayerData.bossDoorCageUnlocked), true);
            //PlayerData.instance.SetBool(nameof(PlayerData.), true);
            /*
            foreach (var item in PlayDataBoolSync.PlayerDatas)
            {
                PlayerData.instance.SetBool(item, true);
            }
            foreach (var item in PlayDataBoolSync.NpcDatas)
            {
                PlayerData.instance.SetBool(item, true);
            }
            foreach (var item in PlayDataBoolSync.BossDatas)
            {
                PlayerData.instance.SetBool(item, true);
            }
            foreach (var item in PlayDataIntSync.BossDatas)
            {
                PlayerData.instance.SetInt(item, 2);
            }
            GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
            {
                activated = true,
                sceneName = "Town",
                semiPersistent = false,
                id = "Death Respawn Trigger 1"
            });
            */
#endif
        }

        private void OnConnectedPlayersChanged(Dictionary<int, string> plays)
        {
            MapSyncMod.LogDebug($"OnConnectedPlayersChanged[{plays.Count}]");
        }

        private void OnQuitToMenu()
        {
            MapSyncMod.LogDebug($"OnQuitToMenu");
        }
        internal static new void LogDebug(string msg)
        {
#if DEBUG
            Instance.Log(msg);
#endif
        }

        public void OnLoadGlobal(GlobalSetting s) => GS = s;

        public GlobalSetting OnSaveGlobal() => GS;
        
        public bool ToggleButtonInsideMenu => true;
        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            return [
                new IMenuMod.MenuEntry
                {
                    Name = "Map Sync".L(),
                    Values = new string[] { "Enabled".L(), "Disabled".L() },
                    Saver = ms => GS.MapSync = ms == 0,
                    Loader = () => GS.MapSync ? 0 : 1
                },
                new IMenuMod.MenuEntry
                {
                    Name = "Bench Sync".L(),
                    Values = new string[] { "Enabled".L(), "Disabled".L() },
                    Saver = bs => GS.BenchSync = bs == 0,
                    Loader = () => GS.BenchSync ? 0 : 1
                },
                new IMenuMod.MenuEntry
                {
                    Name = "Battle Boss Sync".L(),
                    Description = "battle scene and boss".L(),
                    Values = new string[] { "Enabled".L(), "Disabled".L() },
                    Saver = bs => GS.BossSync = bs == 0,
                    Loader = () => GS.BossSync ? 0 : 1
                },
                new IMenuMod.MenuEntry
                {
                    Name = "Pantheons Sync".L(),
                    Description = "Pantheons first Completed".L(),
                    Values = new string[] { "Enabled".L(), "Disabled".L() },
                    Saver = bs => GS.BossDoorSync = bs == 0,
                    Loader = () => GS.BossDoorSync ? 0 : 1
                },
                new IMenuMod.MenuEntry
                {
                    Name = "Lever Sync".L(),

                    Values = new string[] { "Enabled".L(), "Disabled".L() },
                    Saver = bs => GS.LeverSync = bs == 0,
                    Loader = () => GS.LeverSync ? 0 : 1
                },
                new IMenuMod.MenuEntry
                {
                    Name = "Lever Sync Display".L(),
                    Description = "Display in top right".L(),
                    Values = new string[] { "Enabled".L(), "Disabled".L() },
                    Saver = od => GS.LeverDisplay = od == 0,
                    Loader = () => GS.LeverDisplay ? 0 : 1
                },
                new IMenuMod.MenuEntry
                {
                    Name = "Break Wall Sync".L(),

                    Values = new string[] { "Enabled".L(), "Disabled".L() },
                    Saver = bs => GS.WallSync = bs == 0,
                    Loader = () => GS.WallSync ? 0 : 1
                },
                new IMenuMod.MenuEntry
                {
                    Name = "Break Wall Sync Display".L(),
                    Description = "Display in top right".L(),
                    Values = new string[] { "Enabled".L(), "Disabled".L() },
                    Saver = od => GS.WallDisplay = od == 0,
                    Loader = () => GS.WallDisplay ? 0 : 1
                },
                new IMenuMod.MenuEntry
                {
                    Name = "Other Sync".L(),
                    Description= "sly Floor Chest Door ..".L(),
                    Values = new string[] { "Enabled".L(), "Disabled".L() },
                    Saver = os => GS.OtherSync = os == 0,
                    Loader = () => GS.OtherSync ? 0 : 1
                },
                new IMenuMod.MenuEntry
                {
                    Name = "Other Sync Display".L(),
                    Description = "Display in top right".L(),
                    Values = new string[] { "Enabled".L(), "Disabled".L() },
                    Saver = od => GS.OtherDisplay = od == 0,
                    Loader = () => GS.OtherDisplay ? 0 : 1
                },
                new IMenuMod.MenuEntry
                {
                    Name = "Override Localization".L(),
                    Description = "Keeps Benchwarp in English regardless of game language".L(),
                    Values = new string[] { "Enabled".L(), "Disabled".L() },
                    Saver = od => GS.OverrideLocalization = od == 0,
                    Loader = () => GS.OverrideLocalization ? 0 : 1
                }
#if DEBUG
                ,
                new IMenuMod.MenuEntry
                {
                    Name = "unlock all boss".L(),
                    Values = new string[] { "Enabled".L(), "Disabled".L() },
                    Saver = od => {
                        foreach (var item in PlayDataBoolSync.BossDatas)
                        {
                            PlayerData.instance.SetBool(item, true);
                        }
                    },
                    Loader = () => GS.OtherDisplay ? 0 : 1
                }
#endif
                ];
        }
    }
}
