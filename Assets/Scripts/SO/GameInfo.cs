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
        public float[] HornLevels;

        [Header("Energy")]
        [Tooltip("Max energy multiplier used against based energy gain")]
        public float MaxEnergyMultiplierGain;
        public AnimationCurve EnergyCurveGain;

        [Header("Attack")]
        [Tooltip("Max atk multiplier used against current atk")]
        public float MaxAtkMultiplerGain;
        public AnimationCurve AtkCurveGain;

        [Header("Charm")]
        [Tooltip("Max atk multiplier used against current atk")]
        public float CharmAtkMultiplerGain;
        public AnimationCurve CharmCurveGain;

        [Header("Health")]
        public int MaxHealthMultiplerGain;
        public AnimationCurve MaxHealthCurveGain;
    }

    public enum UpgradableStat
    {
        BaseHealth,
        CharmPower,
        AtkPower,
        EnergyGained
    }
}