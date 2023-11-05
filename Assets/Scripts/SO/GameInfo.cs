using UnityEngine;

namespace LewdieJam.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/GameInfo", fileName = "GameInfo")]
    public class GameInfo : ScriptableObject
    {
        public int MinBuyCost;
        public int MaxBuyCost;
        public int MaxLevel;
        public AnimationCurve CostProgression;
    }

    public enum UpgradableStat
    {
        BaseHealth,
        AtkSpeed,
        AtkPower,
        EnergyGained
    }
}