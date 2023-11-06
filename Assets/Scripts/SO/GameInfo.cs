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
        public int MaxEnergyMultiplierGain;
        public AnimationCurve EnergyCurveGain;

        [Header("Attack")]
        [Tooltip("Max atk multiplier used against current atk")]
        public int MaxAtkMultiplerGain;
        public AnimationCurve AtkCurveGain;
        [Tooltip("Atk speed multiplier, should be under 1")]
        public float MinAtkSpeedGain;
        public AnimationCurve AtkSpeedCurveGain;
    }

    public enum UpgradableStat
    {
        BaseHealth,
        AtkSpeed,
        AtkPower,
        EnergyGained
    }
}