using UnityEngine;

namespace LewdieJam.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/PlayerInfo", fileName = "PlayerInfo")]
    public class CharacterInfo : ScriptableObject
    {
        [Tooltip("Speed of the player")]
        public float Speed;

        public int BaseHealth;

        public float Range;

        public int AttackForce;

        [Tooltip("Dash (player only)")]
        public float DashDuration;
        public float DashSpeedMultiplier;
    }
}