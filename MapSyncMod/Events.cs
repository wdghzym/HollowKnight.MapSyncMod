using MonoMod.ModInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MapSyncMod
{
    public static class Events
    {
        public static event Action<string, string, float, float> BenchDeploying;

        internal static void BenchDeployingInternal(string name, string scene, float x, float y)
        {
            if (name != null && name != "")
                BenchDeploying?.Invoke(name, scene, x, y);
        }

        public static event Action<string, string, float, float> DreamGateing;

        internal static void DreamGateingInternal(string name, string scene, float x, float y)
        {
            if (name != null && name != "")
                DreamGateing?.Invoke(name, scene, x, y);
        }
    }
}
