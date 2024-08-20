using MonoMod.ModInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapSyncMod
{
    //https://github.com/MonoMod/MonoMod/blob/a6ac339af45e50bc4ba63ea37ec60b1ffe604d4d/docs/Utils/ModInterop.md?plain=1#L19
    [ModExportName(nameof(MapSyncMod))]
    public static class Export
    {
        public static void AddBenchDeploying(Action<string, string, float, float> func)
        {
            Events.BenchDeploying += func;
        }
        public static void AddDreamGateing(Action<string, string, float, float> func)
        {
            Events.DreamGateing += func;
        }
        public static void SubBenchDeploying(Action<string, string, float, float> func)
        {
            Events.BenchDeploying -= func;
        }
        public static void SubDreamGateing(Action<string, string, float, float> func)
        {
            Events.DreamGateing -= func;
        }
        public static void AddNewSceneing(Action<string, string> func)
        {
            Events.NewSceneing += func;
        }
        public static void SubNewSceneing(Action<string, string> func)
        {
            Events.NewSceneing -= func;
        }

        public static void SkipBench(bool skip)
        {
            MapSyncMod.Instance.BenchDeploySync.SkipBench = skip;
            MapSyncMod.LogDebug($"SkipBench {skip}");
        }

        public static void SendBenchToPlayers(string benchScene, float benchX, float benchY)
        {
            MapSyncMod.Instance.BenchDeploySync.SendBenchToPlayers(benchScene, benchX, benchY);
        }
    }
}
