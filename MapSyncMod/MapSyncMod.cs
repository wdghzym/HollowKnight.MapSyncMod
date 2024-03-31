using ItemSyncMod.SyncFeatures.TransitionsFoundSync;
using MapChanger;
using MapChanger.UI;
using Modding;
using MultiWorldLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.SceneManagement;
using static tk2dSpriteCollectionDefinition;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.SaveSlotButton;

namespace MapSyncMod
{
    public class MapSyncMod : Mod
    {
        internal static MapSyncMod Instance;
        public MapSync MapSync;
        public BenchSysnc BenchSysnc;
        public MapSyncMod() { Instance = this; }
        public override string GetVersion()
        {
            return GetType().Assembly.GetName().Version.ToString();
        }
        public override int LoadPriority() => 5;
        public override void Initialize()
        {
            base.Initialize();
            MapSync = new MapSync();
            BenchSysnc = new BenchSysnc();
            //On.GameManager.LoadGame += GameManager_LoadGame;
            //On.GameManager.ReturnToMainMenu += GameManager_ReturnToMainMenu;
        }

        private void OnEnterGame()
        {
#if DEBUG
            MapSyncMod.Instance.Log($"OnEnterGame");
#endif
            //ItemSyncMod.ItemSyncMod.Connection.OnConnectedPlayersChanged
            //ItemSyncMod.ItemSyncMod.Connection.  
            //ItemSyncMod.ItemSyncMod.ISSettings.AddSentData
        }

        private void OnQuitToMenu()
        {
#if DEBUG
            MapSyncMod.Instance.Log($"OnQuitToMenu");
#endif

        }

    }
}
