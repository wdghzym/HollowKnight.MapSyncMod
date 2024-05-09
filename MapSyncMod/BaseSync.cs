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
        public readonly string MESSAGE_LABEL ;
        public List<int> SyncPlayers = new List<int>();
        public BaseSync(string mESSAGE_LABEL)
        {
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
            if (!ItemSyncMod.ItemSyncMod.ISSettings.IsItemSync) return;
            ItemSyncMod.ItemSyncMod.Connection.OnDataReceived += OnDataReceivedInternal;
            OnEnterGame();
        }
        protected virtual void OnEnterGame() { }
        private void OnQuitToMenuInternal()
        {
            ItemSyncMod.ItemSyncMod.Connection.OnDataReceived -= OnDataReceivedInternal;
            OnQuitToMenu();
            SyncPlayers.Clear();
        }
        protected virtual void OnQuitToMenu() { }
        private void OnDataReceivedInternal(DataReceivedEvent dataReceivedEvent) 
        {
            if (dataReceivedEvent.Label != MESSAGE_LABEL) return;
            dataReceivedEvent.Handled = true;
            OnDataReceived(dataReceivedEvent);
        }
        protected virtual void OnDataReceived(DataReceivedEvent dataReceivedEvent) { }
    }
}
