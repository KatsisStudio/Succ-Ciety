using UnityEngine;
using UnityEngine.Assertions;

namespace LewdieJam.Map
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject _spawnTarget;

        private const int _spawnCount = 5;
        private const float _spawnRange = 3f;
        private const int _maxIterations = 100;

        private void Awake()
        {
            int itCount = 0;
            int nbSpawns = 0;
            while (itCount < _maxIterations && nbSpawns < _spawnCount)
            {
                var pos2d = Random.insideUnitCircle * _spawnRange;
                var pos = transform.position + new Vector3(pos2d.x, .5f, pos2d.y);
                if (Physics.OverlapSphereNonAlloc(pos, 1f, null, 1 << 8) == 0) // Make sure the enemies don't spawn inside each other
                {
                    Instantiate(_spawnTarget, pos, _spawnTarget.transform.rotation);
                    nbSpawns++;
                }

                itCount++;
            }
            Assert.AreNotEqual(_maxIterations, itCount, "Spawner reached the max number of iterations, this probably mean the range isn't big enough or the spawn count is too high");
        }
    }
}
