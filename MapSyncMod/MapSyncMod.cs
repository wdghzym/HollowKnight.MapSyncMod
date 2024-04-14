
using HutongGames.PlayMaker.Actions;
using ItemSyncMod.SyncFeatures.TransitionsFoundSync;
using MapChanger;
using MapChanger.UI;
using Modding;
using MultiWorldLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
using static UnityEngine.UI.SaveSlotButton;

namespace MapSyncMod
{
    public class MapSyncMod : Mod
    {
        public static MapSyncMod Instance;
        public MapSync MapSync;
        public BenchSync BenchSync;
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

            MapSync = new MapSync();
            BenchSync = new BenchSync();
            mapSyncModExtension = new MapSyncModExtension();
            //On.GameManager.LoadGame += GameManager_LoadGame;
            //On.GameManager.ReturnToMainMenu += GameManager_ReturnToMainMenu;
            Events.OnEnterGame += OnEnterGame;
            Events.OnQuitToMenu += OnQuitToMenu;

        }

        private void OnEnterGame()
        {
            MapSyncMod.LogDebug($"MapSyncMod OnEnterGame");
            
            ItemSyncMod.ItemSyncMod.Connection.OnConnectedPlayersChanged += OnConnectedPlayersChanged;
            //ItemSyncMod.ItemSyncMod.ISSettings.AddSentData
            //MultiWorldLib.ExportedAPI.ExportedExtensionsMenuAPI.
            //ItemSyncMod.ItemSyncMod.Connection.SendDataToAllConnected()
            //ItemSyncMod.ItemSyncMod.Connection.  
            //ItemSyncMod.ItemSyncMod.ISSettings.AddSentData
        }

        private void SceneData_SaveMyState_PersistentBoolData(On.SceneData.orig_SaveMyState_PersistentBoolData orig, SceneData self, PersistentBoolData persistentBoolData)
        {
            throw new NotImplementedException();
        }

        private void OnConnectedPlayersChanged(Dictionary<int, string> plays)
        {
            MapSyncMod.LogDebug($"OnConnectedPlayersChanged[{plays.Count}]");
        }

        private void OnQuitToMenu()
        {
            MapSyncMod.LogDebug($"OnQuitToMenu");
        }
        internal static void LogDebug(string msg)
        {
#if DEBUG
            Instance.Log(msg);
#endif
        }

    }
}
