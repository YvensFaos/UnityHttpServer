using System;

namespace Server
{
    [Serializable]
    public struct ToggleStatus
    {
        public int index;
        public bool active;
    }
}