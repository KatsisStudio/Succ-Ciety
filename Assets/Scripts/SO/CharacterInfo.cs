using UnityEngine;

namespace LewdieJam.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/PlayerInfo", fileName = "PlayerInfo")]
    public class CharacterInfo : ScriptableObject
    {
        public string Name;

        public float Speed;

        public int BaseHealth;

        public float Range;

        [Header("Attack")]
        public int AttackForce;
        public float PreAttackWaitTime, PostAttackWaitTime;
        public float MainAttackReloadTime, SubAttackReloadTime, DashReloadTime;
        public GameObject MainAttackVfx, SubAttackVfx;

        [Header("Dash (player only)")]
        public float DashDuration;
        public float DashSpeedMultiplier;

        [Header("Enemy Behavior")]
        public bool CanBeCharmed;
        public bool IsBoss;
        public float DetectionRange;
        public int EnergyDropped;

        [Header("Attack Projectile (if relevant)")]
        public float ProjectileSpeed;
        public float ProjectileMaxDistance;
        public float HookSpeed;
    }

}