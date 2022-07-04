using System;

namespace HoloMeSDK
{
    class HologramVisibilityActions : HoloMono
    {
        public Action OnBecameVisible;
        public Action OnBecameInvisible;

        protected override void OnBecameVisibleOverride()
        {
            OnBecameVisible?.Invoke();
        }

        protected override void OnBecameInvisibleOverride()
        {
            OnBecameInvisible?.Invoke();
        }

        public void ClearEvents()
        {
            OnBecameVisible = null;
            OnBecameInvisible = null;
        }
    }
}
