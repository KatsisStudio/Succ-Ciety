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
    }

    public enum UpgradableStat
    {
        BaseHealth,
        AtkSpeed,
        AtkPower,
        EnergyGained
    }
}