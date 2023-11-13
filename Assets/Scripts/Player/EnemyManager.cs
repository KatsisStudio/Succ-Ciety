using LewdieJam.Player;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class EnemyManager : MonoBehaviour
    {
        public static EnemyManager Instance {  get; private set; }

        private readonly List<EnemyController> _enemies = new();

        private void Awake()
        {
            Instance = this;
        }

        public IEnumerable<EnemyController> ToEnumerable()
        {
            foreach (var e in _enemies)
            {
                yield return e;
            }
        }

        public void Register(EnemyController character)
        {
            _enemies.Add(character);
        }

        public void RefreshAllTargets()
        {
            foreach (var e in _enemies)
            {
                e.RefreshTarget();
            }
        }

        public void Unregister(EnemyController character)
        {
            _enemies.Remove(character);
            foreach (var e in _enemies)
            {
                e.OnEnemyUnregistered(character);
            }
        }
    }
}
