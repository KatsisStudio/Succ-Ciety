using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LewdieJam.Achievement
{
    public class AchievementManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _achievementPanel;

        [SerializeField]
        private TMP_Text _title, _description;

        public static AchievementManager Instance { get; private set; }

        private static List<AchievementID> _unlockedAchievements = new();

        private void Awake()
        {
            Instance = this;
        }

        public static bool IsUnlocked(AchievementID id)
            => _unlockedAchievements.Contains(id);

        public void Unlock(AchievementID achievement)
        {
            if (IsUnlocked(achievement))
            {
                return;
            }
            var data = Achievements[achievement];
            _title.text = data.Name;
            _description.text = data.Description;
            _achievementPanel.SetActive(true);
            _unlockedAchievements.Add(achievement);
            StartCoroutine(WaitAndClosePopup());
        }

        private IEnumerator WaitAndClosePopup()
        {
            yield return new WaitForSeconds(5f);
            _achievementPanel.SetActive(false);
        }

        public Dictionary<AchievementID, Achievement> Achievements { get; } = new()
        {
            { AchievementID.VanillaScene, new() { Name = "Still Better than Mint", Description = "Play the vanilla scene" } },
            { AchievementID.BigBoobsScene, new() { Name = "I'm Playing for the Plot", Description = "Play the big boobs scene" } },
            { AchievementID.FutanariScene, new() { Name = "Magical Wand", Description = "Play the futanari scene" } },
            { AchievementID.PregnantScene, new() { Name = "Knocking at the Door", Description = "Play the pregnant scene" } },
            { AchievementID.DontEnterScene, new() { Name = "Well Educated", Description = "Refuse to enter inside a house" } },
            { AchievementID.DoubleHook, new() { Name = "Hooked for You", Description = "Get hooked by an enemy while already being stunned" } },,
            { AchievementID.Dickus, new() { Name = "Big Nose", Description = "Beat Dickus" } }
        };
    }

    public enum AchievementID
    {
        VanillaScene,
        BigBoobsScene,
        FutanariScene,
        PregnantScene,
        DontEnterScene,
        DoubleHook,
        Dickus
    }

    public record Achievement
    {
        public string Name;
        public string Description;
    }
}
