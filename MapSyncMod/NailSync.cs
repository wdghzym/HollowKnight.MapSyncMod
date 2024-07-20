using MultiWorldLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapSyncMod
{
    public class NailUpgradeSync : BaseSync
    {
        public  NailUpgradeSync() :base("MapSyncMod-NailSync")
        {

        }
        public struct NailUpgrade
        {
            public int NailSmithUpgrades;
            public string From;
        }
    }
}
