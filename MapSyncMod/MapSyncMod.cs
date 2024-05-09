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
        public SceneDataBoolSync SceneDataBoolSync;
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
            SceneDataBoolSync = new SceneDataBoolSync();
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
            if (PlayerData.instance.GetInt(intName) != value)
                ShowMessage($"{intName}-{value}");
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
                    ShowMessage($"PersistentBoolData {persistentBoolData.activated}-{persistentBoolData.sceneName.BL()}\n  {persistentBoolData.semiPersistent}-{persistentBoolData.id} last{GameManager.instance.sceneData.FindMyState(persistentBoolData)?.activated}");

            orig.Invoke(self, persistentBoolData);
 }
        private void SceneData_SaveMyState_PersistentIntData(On.SceneData.orig_SaveMyState_PersistentIntData orig, SceneData self, PersistentIntData persistentIntData)
        {
            orig.Invoke(self, persistentIntData);
            //if (GameManager.instance.sceneData.FindMyState(persistentIntData) is not null)
             //   if (GameManager.instance.sceneData.FindMyState(persistentIntData).value != persistentIntData.value)
                ShowMessage($"PersistentIntData {persistentIntData.value}-{persistentIntData.sceneName.BL()}\n  {persistentIntData.semiPersistent}-{persistentIntData.id}");
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
            GameManager.instance.sceneData.SaveMyState(new PersistentBoolData
            {
                activated = true,
                sceneName = "Town",
                semiPersistent = false,
                id = "Death Respawn Trigger 1"
            });
            */

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
                    Name = "Other Sync (When Scene Changed)".L(),
                    Description= "lever, wall, battle, door, and other".L(),
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
                }
                ];
        }
    }
}
