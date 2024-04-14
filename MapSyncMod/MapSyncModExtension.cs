using ItemSyncMod.Menu;
using MultiWorldLib.ExportedAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MultiWorldLib.ExportedAPI.ExportedExtensionsMenuAPI.MenuStateEvents;
using MenuChanger.MenuElements;
using MapChanger;

namespace MapSyncMod
{
    internal class MapSyncModExtension : ExportedExtensionsMenuAPI
    {
        ToggleButton MapSyncButton, BenchSyncButton;
        public MapSyncModExtension()
        {
            Init();
        }

        public void Init()
        {
            Events.OnEnterGame += OnEnterGame;
            MapSyncHandler = new OnExtensionMenuConstructionHandler(MapSyncOnExtensionMenuConstruction);
            BenchSyncHandler= new OnExtensionMenuConstructionHandler(BenchSyncOnExtensionMenuConstruction);
            ExportedExtensionsMenuAPI.AddExtensionsMenu(MapSyncHandler);
            ExportedExtensionsMenuAPI.AddExtensionsMenu(BenchSyncHandler);
        }
        private OnExtensionMenuConstructionHandler MapSyncHandler, BenchSyncHandler;
        public void UnInit()
        {
            Events.OnEnterGame -= OnEnterGame;
            ExportedExtensionsMenuAPI.RemoveExtensionsMenu(MapSyncHandler);
            ExportedExtensionsMenuAPI.RemoveExtensionsMenu(BenchSyncHandler);
        }


        private void OnEnterGame()
        {
            MapSyncMod.LogDebug($"MapSyncModExtension OnEnterGame");

            var readyMetadata = ItemSyncMod.ItemSyncMod.ISSettings.readyMetadata;
            if (readyMetadata == null) return;            

            MapSyncMod.LogDebug($"readyMetadata count {readyMetadata.Count}");

            MapSyncMod.Instance.MapSync.MapSyncPlayers.Clear();
            MapSyncMod.Instance.BenchSync.BenchSyncPlayers.Clear();
            for (int playerid = 0; playerid < readyMetadata.Count; playerid++)
            {
                MapSyncMod.LogDebug($"items {readyMetadata[playerid].Count}");                
                foreach (var item in readyMetadata[playerid])
                {
                    MapSyncMod.LogDebug($"playerid {playerid} mwplayerid {ItemSyncMod.ItemSyncMod.ISSettings.MWPlayerId}");
                    MapSyncMod.LogDebug($"key[{item.Key}] value[{item.Value}]");   
                }
                if (playerid == ItemSyncMod.ItemSyncMod.ISSettings.MWPlayerId) continue;
                if (readyMetadata[playerid].ContainsKey(nameof(MapSync)))
                {
                    MapSyncMod.Instance.MapSync.MapSyncPlayers.Add(playerid);
                    MapSyncMod.LogDebug($"addMapSyncPlayers playerid[{playerid}]");
                }
                if (readyMetadata[playerid].ContainsKey(nameof(BenchSync)))
                {
                    MapSyncMod.Instance.BenchSync.BenchSyncPlayers.Add(playerid);
                    MapSyncMod.LogDebug($"addBenchSyncPlayers playerid[{playerid}]");
                }
            }
        }

        private BaseButton MapSyncOnExtensionMenuConstruction(MenuChanger.MenuPage menuPage)
        {
            MapSyncMod.LogDebug($"MapSyncOnExtensionMenuConstruction");
            MenuStateEvents.OnAddReadyMetadata += MapSync_OnAddReadyMetadata;
            MapSyncButton = new ToggleButton(menuPage, " ");
            MapSyncButton.SetValue(true);
            return MapSyncButton;
        }


        private BaseButton BenchSyncOnExtensionMenuConstruction(MenuChanger.MenuPage menuPage)
        {
            MapSyncMod.LogDebug($"BenchSyncOnExtensionMenuConstruction");
            MenuStateEvents.OnAddReadyMetadata += BenchSync_OnAddReadyMetadata;
            BenchSyncButton = new ToggleButton(menuPage, " ");
            BenchSyncButton.SetValue(true);
            return BenchSyncButton;
        }

        private void MapSync_OnAddReadyMetadata(Dictionary<string, string> metadata)
        {
            MapSyncMod.LogDebug($"MapSync_OnAddReadyMetadata");
            if (metadata == null) return;
            MapSyncMod.LogDebug($"MapSync_OnAddReadyMetadata nonull {metadata.Count}");

            if (!metadata.ContainsKey(MapSyncMod.Instance.GetName()))
            {
                metadata.Add(MapSyncMod.Instance.GetName(), MapSyncMod.Instance.GetVersion());
            }
            if (!metadata.ContainsKey(nameof(MapSync)))// && MapSyncButton.Value)
            {
                metadata.Add(nameof(MapSync), MapSyncMod.Instance.GetVersion());
            }
            MapSyncMod.LogDebug($"MapSync_OnAddReadyMetadata add");
        }
        private void BenchSync_OnAddReadyMetadata(Dictionary<string, string> metadata)
        {
            MapSyncMod.LogDebug($"BenchSync_OnAddReadyMetadata");
            if (metadata == null) return;
            MapSyncMod.LogDebug($"BenchSync_OnAddReadyMetadata nonull {metadata.Count}");

            if (!metadata.ContainsKey(MapSyncMod.Instance.GetName()))
            {
                metadata.Add(MapSyncMod.Instance.GetName(), MapSyncMod.Instance.GetVersion());
            }
            if (!metadata.ContainsKey(nameof(BenchSync))) //&& BenchSyncButton.Value)
            {
                metadata.Add(nameof(BenchSync), MapSyncMod.Instance.GetVersion());
            }
            MapSyncMod.LogDebug($"BenchSync_OnAddReadyMetadata add");
        }
    }
}
