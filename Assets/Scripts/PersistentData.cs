using System.Collections.Generic;

namespace LewdieJam
{
    public static class PersistentData
    {
        public static int Energy { set; get; } = 1000;

        public static Dictionary<string, int> Stats { get; } = new();

        public static int GetStatValue(string key)
        {
            if (Stats.ContainsKey(key)) return Stats[key];
            return 0;
        }
    }
}
