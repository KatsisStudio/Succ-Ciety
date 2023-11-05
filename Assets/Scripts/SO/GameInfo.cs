using UnityEngine;

namespace LewdieJam.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/GameInfo", fileName = "GameInfo")]
    public class GameInfo : ScriptableObject
    {
        [Header("Stats")]
        public int MinBuyCost;
        public int MaxBuyCost;
        public int MaxLevel;
        public AnimationCurve CostProgression;

        [Header("Horn")]
        public int MaxHornLevel;

        [Header("Energy")]
        [Tooltip("How much additional energy you earn per energy stat upgrade (multiplied by base amount gained")]
        public int EnergyMultiplierPerLevel;
    }

    public enum UpgradableStat
    {
        BaseHealth,
        AtkSpeed,
        AtkPower,
        EnergyGained
    }
}