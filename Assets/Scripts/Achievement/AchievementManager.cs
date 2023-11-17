using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

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
        private static List<int> _tokensFound = new();

        public int CurrentTokenCount { private set; get; }
        public int TokenFoundCount => _tokensFound.Count;

        private void Awake()
        {
            var tokens = FindObjectsByType<Token>(FindObjectsSortMode.None);
            Assert.AreEqual(tokens.Length, tokens.Distinct().Count(), "Some spawned tokens have dupplicated IDs");

            CurrentTokenCount = tokens.Length;

            foreach (var t in tokens)
            {
                if (_tokensFound.Contains(t.ID))
                {
                    Destroy(t.gameObject);
                }
            }

            Instance = this;
        }

        public void GrabToken(int id)
        {
            _tokensFound.Add(id);
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
            { AchievementID.PregnantScene, new() { Name = "Big 'n Round", Description = "Play the pregnant scene" } },
            { AchievementID.TentacleScene, new() { Name = "Exploring New Holes", Description = "Play the secret scene" } },
            { AchievementID.DontEnterScene, new() { Name = "Well Educated", Description = "Refuse to enter inside a house" } },
            { AchievementID.DoubleHook, new() { Name = "Hooked for You", Description = "Get hooked by an enemy while already being stunned" } },
            { AchievementID.Dickus, new() { Name = "Big Nose", Description = "Beat Dickus" } }
        };
    }

    public enum AchievementID
    {
        VanillaScene,
        BigBoobsScene,
        FutanariScene,
        PregnantScene,
        TentacleScene,
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
