using MonoMod.ModInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapSyncMod
{
    internal class RecentItems
    {
        [ModImportName("RecentItems")]
        private static class RecentItemsImport
        {
            public static Action<string , string> ShowItemChangerSprite = null;
        }

        static RecentItems()
        {
            typeof(RecentItemsImport).ModInterop();
        }

        internal static void ShowItemChangerSprite(string message, string spriteKey)
        {
            RecentItemsImport.ShowItemChangerSprite?.Invoke(message, spriteKey);
        }
    }
}
