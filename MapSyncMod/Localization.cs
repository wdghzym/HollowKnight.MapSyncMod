using Language;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapSyncMod
{
    public static class Localization
    {
        private static Dictionary<string, string> _map;

        internal static void HookLocalization()
        {
            On.Language.Language.DoSwitch += OnSwitchLanguage;
            SetLanguage(Language.Language.CurrentLanguage());
        }

        internal static void UnhookLocalization()
        {
            On.Language.Language.DoSwitch -= OnSwitchLanguage;
            _map = null;
        }

        private static void OnSwitchLanguage(On.Language.Language.orig_DoSwitch orig, LanguageCode newLang)
        {
            orig(newLang);
            SetLanguage(newLang);
        }

        private static void SetLanguage(LanguageCode code)
        {
            if (GetBenchwarpLanguageCode(code) is string name)
            {
                try
                {
                    _map = JsonUtil.Deserialize<Dictionary<string, string>>($"{nameof(MapSyncMod)}.Resources.Langs.{name}.json");
                }
                catch (Exception e)
                {
                    MapSyncMod.Instance.Log($"Error changing language to {code}: {e}");
                    _map = null;
                }
            }
            else
            {
                _map = null;
            }
        }

        private static string GetBenchwarpLanguageCode(LanguageCode newLang)
        {
            //return newLang.ToString().ToLower();
            return newLang switch
            {
                LanguageCode.ZH => "zh",
                _ => null
            };
        }

        public static string Localize(this string text)
        {
            if (MapSyncMod.GS.OverrideLocalization) return text;

            return _map is not null && _map.TryGetValue(text, out string newText) ? newText : text;
        }
        //public static string BL(this string text) => Benchwarp.Localization.Localize(text);
        public static string BL(this string text) => Benchwarp.Localization.Localize(Localize(text));
        public static string L(this string text) => Localize(text);
    }
}
