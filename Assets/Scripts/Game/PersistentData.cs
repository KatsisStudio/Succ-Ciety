using LewdieJam.SO;
using System.Collections.Generic;

namespace LewdieJam.Game
{
    public static class PersistentData
    {
        public static int Energy { set; get; }

        public static Dictionary<UpgradableStat, int> Stats { get; } = new();

        public static Attachment Attachments { set; get; }

        public static int GetStatValue(UpgradableStat key)
        {
            if (Stats.ContainsKey(key)) return Stats[key];
            return 0;
        }
    }
}
