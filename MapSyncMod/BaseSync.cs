using MapChanger;
using MultiWorldLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapSyncMod
{
    public class BaseSync
    {
        public readonly string MESSAGE_LABEL;
        public List<int> SyncPlayers = new List<int>();
        public BaseSync(string mESSAGE_LABEL)
        {
            //MESSAGE_LABEL = $"{nameof(MapSyncMod)}-{mESSAGE_LABEL}";
            MESSAGE_LABEL = mESSAGE_LABEL;
            Init();
        }
        public virtual void Init()
        {
            Events.OnEnterGame += OnEnterGameInternal;
            Events.OnQuitToMenu += OnQuitToMenuInternal;
        }
        public virtual void UnInit()
        {
            Events.OnEnterGame -= OnEnterGameInternal;
            Events.OnQuitToMenu -= OnQuitToMenuInternal;
            OnQuitToMenu();
        }
        private void OnEnterGameInternal()
        {
            try
            {
                if (!ItemSyncMod.ItemSyncMod.ISSettings.IsItemSync) return;
                ItemSyncMod.ItemSyncMod.Connection.OnDataReceived += OnDataReceivedInternal;
                OnEnterGame();
            }
            catch (Exception e) { MapSyncMod.Instance.LogError($"{e.Message} \n{e.StackTrace}"); }
        }
        protected virtual void OnEnterGame() { }
        private void OnQuitToMenuInternal()
        {
            try
            {
                ItemSyncMod.ItemSyncMod.Connection.OnDataReceived -= OnDataReceivedInternal;
                OnQuitToMenu();
                SyncPlayers.Clear();
            }
            catch (Exception e) { MapSyncMod.Instance.LogError($"{e.Message} \n{e.StackTrace}"); }
        }
        protected virtual void OnQuitToMenu() { }
        private void OnDataReceivedInternal(DataReceivedEvent dataReceivedEvent)
        {
            try
            {
                //MapSyncMod.LogDebug($"get data {dataReceivedEvent.Label}");
                if (dataReceivedEvent.Label != MESSAGE_LABEL) return;
                dataReceivedEvent.Handled = true;
                OnDataReceived(dataReceivedEvent);
            }
            catch (Exception e) { MapSyncMod.Instance.LogError($"{e.Message} \n{e.StackTrace}"); }
        }
        protected virtual void OnDataReceived(DataReceivedEvent dataReceivedEvent) { }
        protected void ShowItemChangerSprite(string name, string playername, string from, string spriteKey)
        {
            if (name is null) return;
            if (!Interop.HasRecentItemsDisplay()) return;
            ShowItemChangerSprite2(name, playername, from, spriteKey);
        }
        private void ShowItemChangerSprite2(string name, string playername, string from, string spriteKey)
        {
            if (playername is not null)
            {
                if (from is null)
                    RecentItemsDisplay.Export.ShowItemChangerSprite($"{name.L()}\nfrom {playername}", spriteKey);
                else
                    RecentItemsDisplay.Export.ShowItemChangerSprite($"{name.L()}\nfrom {playername} \nin {from.L()}", spriteKey);
            }
            else
            {
                if (from is null)
                    RecentItemsDisplay.Export.ShowItemChangerSprite($"{name.L()}", spriteKey);
                else
                    RecentItemsDisplay.Export.ShowItemChangerSprite($"{name.L()}\nfrom {from.L()}", spriteKey);

            }
        }
    }
}
