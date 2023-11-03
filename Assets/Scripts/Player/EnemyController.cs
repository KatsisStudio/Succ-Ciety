namespace LewdieJam.Player
{
    public class EnemyController : ACharacter
    {
        private void Awake()
        {
            AwakeParent();
        }

        public override void Die()
        {
            GameManager.Instance.Energy++;
        }
    }
}
