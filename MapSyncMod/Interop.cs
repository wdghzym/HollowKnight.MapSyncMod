using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace MapSyncMod
{
    internal static class Interop
    {
        private static readonly Dictionary<string, Assembly> interopMods = new()
        {
            { "BenchRando", null},
            { "Benchwarp", null},
            { "RecentItemsDisplay", null}
        };

        internal static void FindInteropMods()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (interopMods.ContainsKey(assembly.GetName().Name))
                {
                    interopMods[assembly.GetName().Name] = assembly;
                }
            }
            MapSyncMod.LogDebug($"HasRecentItemsDisplay {HasRecentItemsDisplay()}");
            MapSyncMod.LogDebug($"HasBenchRando {HasBenchRando()}");
        }

        internal static bool HasBenchRando()
        {
            return interopMods["BenchRando"] is not null;
        }

        internal static bool HasBenchwarp()
        {
            return interopMods["Benchwarp"] is not null;
        }
        internal static bool HasRecentItemsDisplay()
        {
            return interopMods["RecentItemsDisplay"] is not null;
        }
    }
}
