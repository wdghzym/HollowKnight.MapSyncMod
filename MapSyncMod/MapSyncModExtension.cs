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


            for (int playerid = 0; playerid < readyMetadata.Count; playerid++)
            {
                MapSyncMod.LogDebug($"items {readyMetadata[playerid].Count}");                
                foreach (var item in readyMetadata[playerid])
                {
                    MapSyncMod.LogDebug($"playerid {playerid} mwplayerid {ItemSyncMod.ItemSyncMod.ISSettings.MWPlayerId}");
                    MapSyncMod.LogDebug($"key[{item.Key}] value[{item.Value}]");   
                }
                if (playerid == ItemSyncMod.ItemSyncMod.ISSettings.MWPlayerId) continue;
                //readyMetadata[playerid].TryGetValue(nameof(MapSync), out value);
                //if (readyMetadata[playerid].ContainsKey(nameof(MapSync)))
                if(readyMetadata[playerid].TryGetValue(nameof(MapSync), out string value))
                {
                    switch (value)
                    {
                        case "1.0.0.0-debug":
                        case "1.0.0.0":
                            MapSyncMod.Instance.MapSync.mapSync1000.SyncPlayers.Add(playerid);
                            MapSyncMod.LogDebug($"addMapSyncPlayers1000 playerid[{playerid}]");
                            break;
                        default:
                            MapSyncMod.Instance.MapSync.SyncPlayers.Add(playerid);
                            MapSyncMod.LogDebug($"addMapSyncPlayers playerid[{playerid}]");
                            break;
                    }
                }
                //if (readyMetadata[playerid].ContainsKey(nameof(BenchSync)))
                if (readyMetadata[playerid].TryGetValue(nameof(BenchSync), out value))
                {
                    switch (value)
                    {
                        case "1.0.0.0-debug":
                        case "1.0.0.0":
                            MapSyncMod.Instance.BenchSync.benchSync1000.SyncPlayers.Add(playerid);
                            MapSyncMod.LogDebug($"addBenchSyncPlayers1000 playerid[{playerid}]");
                            break;
                        default:
                            MapSyncMod.Instance.BenchSync.SyncPlayers.Add(playerid);
                            MapSyncMod.LogDebug($"addBenchSyncPlayers playerid[{playerid}]");
                            break;
                    }
                }
                if (readyMetadata[playerid].ContainsKey(nameof(PlayDataBoolSync)))
                {
                    MapSyncMod.Instance.PlayDataBoolSync.SyncPlayers.Add(playerid);
                    MapSyncMod.LogDebug($"addPlayDataBoolSync playerid[{playerid}]");
                }
                if (readyMetadata[playerid].ContainsKey(nameof(SceneDataBoolSync)))
                {
                    MapSyncMod.Instance.SceneDataBoolSync.SyncPlayers.Add(playerid);
                    MapSyncMod.LogDebug($"addSceneDataBoolSync playerid[{playerid}]");
                }
                if (readyMetadata[playerid].ContainsKey(nameof(PlayDataIntSync)))
                {
                    MapSyncMod.Instance.PlayDataIntSync.SyncPlayers.Add(playerid);
                    MapSyncMod.LogDebug($"addPlayDataIntSync playerid[{playerid}]");
                }
                if (readyMetadata[playerid].ContainsKey(nameof(BossDoorSync)))
                {
                    MapSyncMod.Instance.BossDoorSync.SyncPlayers.Add(playerid);
                    MapSyncMod.LogDebug($"addBossDoorSync playerid[{playerid}]");
                }
                
            }
        }

        private BaseButton MapSyncOnExtensionMenuConstruction(MenuChanger.MenuPage menuPage)
        {
            MapSyncMod.LogDebug($"MapSyncOnExtensionMenuConstruction");//x4??
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

            if (!metadata.ContainsKey(nameof(PlayDataBoolSync)))
            {
                metadata.Add(nameof(PlayDataBoolSync), MapSyncMod.Instance.GetVersion());
            }
            if (!metadata.ContainsKey(nameof(SceneDataBoolSync)))
            {
                metadata.Add(nameof(SceneDataBoolSync), MapSyncMod.Instance.GetVersion());
            }
            if (!metadata.ContainsKey(nameof(PlayDataIntSync)))
            {
                metadata.Add(nameof(PlayDataIntSync), MapSyncMod.Instance.GetVersion());
            }
            if (!metadata.ContainsKey(nameof(BossDoorSync)))
            {
                metadata.Add(nameof(BossDoorSync), MapSyncMod.Instance.GetVersion());
            }

            MapSyncMod.LogDebug($"BenchSync_OnAddReadyMetadata add");
        }
    }
}
