using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickSack.Shared
{
    public class ToasterState
    {
        public string ToastText { get; private set; } = "";

        public event Action OnChange;

        public ToasterState()
        {
        }

        private void NotifyStateChange() => OnChange?.Invoke();

        public void SetToastMessage(string toastMsg)
        {
            ToastText = toastMsg ?? string.Empty;
            NotifyStateChange();
        }
    }
}
