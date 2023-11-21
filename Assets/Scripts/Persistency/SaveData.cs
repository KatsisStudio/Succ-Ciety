using LewdieJam.Achievement;
using LewdieJam.Game;
using LewdieJam.SO;
using System.Collections.Generic;

namespace LewdieJam.Persistency
{
    public class SaveData
    {

        public List<AchievementID> UnlockedAchievements { set; get; } = new();
        public List<int> TokensFound { set; get; } = new();

        public int PendingEnergy { set; get; }
        public int Energy { set; get; }
        public Dictionary<UpgradableStat, int> Stats { set; get; } = new();
        public Attachment Attachments { set; get; }

        public List<Attachment> VisitedHouses { set; get; } = new();
        public bool DidWinGame =>VisitedHouses.Count == 4; // TODO: Don't hardcode?

        public void GrabToken(int id)
        {
            TokensFound.Add(id);
            PersistencyManager.Instance.Save();
        }

        public bool WasIDFound(int id)
            =>TokensFound.Contains(id);

        public int TokenFoundCount => TokensFound.Count;

        public bool IsUnlocked(AchievementID id)
            => UnlockedAchievements.Contains(id);

        public void Unlock(AchievementID id)
        {
            UnlockedAchievements.Add(id);
            PersistencyManager.Instance.Save();
        }

        public int GetStatValue(UpgradableStat key)
        {
            if (Stats.ContainsKey(key)) return Stats[key];
            return 0;
        }
    }
}
