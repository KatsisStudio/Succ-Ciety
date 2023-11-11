using UnityEngine;

namespace LewdieJam.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/PlayerInfo", fileName = "PlayerInfo")]
    public class CharacterInfo : ScriptableObject
    {
        public float Speed;

        public int BaseHealth;

        public float Range;

        [Header("Attack")]
        public int AttackForce;
        public float PreAttackWaitTime, PostAttackWaitTime;

        [Header("Dash (player only)")]
        public float DashDuration;
        public float DashSpeedMultiplier;
    }
}