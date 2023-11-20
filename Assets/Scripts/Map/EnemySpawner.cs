using LewdieJam.Player;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace LewdieJam.Map
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] _spawnTargets;

        [SerializeField]
        private int _spawnCount = 5;
        [SerializeField]
        private float _spawnRange = 3f;

        private const int _maxIterations = 100;

        public readonly List<GameObject> _spawned = new();

        public void Unregister(GameObject go)
        {
            _spawned.Remove(go);
            if (!_spawned.Any())
            {
                StartCoroutine(WaitAndSpawns());
            }
        }

        private void Awake()
        {
            SpawnAll();
        }

        private void SpawnAll()
        {
            int itCount = 0;
            int nbSpawns = 0;
            while (itCount < _maxIterations && nbSpawns < _spawnCount)
            {
                var pos2d = Random.insideUnitCircle * _spawnRange;
                var pos = transform.position + new Vector3(pos2d.x, .5f, pos2d.y);
                if (Physics.OverlapSphereNonAlloc(pos, 1f, null, 1 << 8) == 0) // Make sure the enemies don't spawn inside each other
                {
                    var t = _spawnTargets[Random.Range(0, _spawnTargets.Length)];
                    var en = Instantiate(t, pos, t.transform.rotation);
                    en.GetComponent<AEnemyController>().Spawner = this;
                    _spawned.Add(en);
                    nbSpawns++;
                }

                itCount++;
            }
            Assert.AreNotEqual(_maxIterations, itCount, "Spawner reached the max number of iterations, this probably mean the range isn't big enough or the spawn count is too high");
        }

        private IEnumerator WaitAndSpawns()
        {
            yield return new WaitForSeconds(10f);
            while (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) < 20f)
            {
                yield return new WaitForSeconds(1f);
            }
            SpawnAll();
        }
    }
}
