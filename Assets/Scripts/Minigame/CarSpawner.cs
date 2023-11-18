using System.Collections;
using UnityEngine;

namespace LewdieJam.Minigame
{
    public class CarSpawner : MonoBehaviour
    {
        [SerializeField]
        private float _min, _max;

        [SerializeField]
        private GameObject _car;

        [SerializeField]
        private float _carSpeed;

        [SerializeField]
        private bool _lookRight;

        private void Awake()
        {
            StartCoroutine(Spawn());
        }

        private IEnumerator Spawn()
        {
            while (true)
            {
                var car = Instantiate(_car, transform.position, _car.transform.rotation);
                car.GetComponent<Car>().Speed = _carSpeed;
                if (!_lookRight)
                {
                    car.transform.rotation = Quaternion.Euler(_car.transform.rotation.eulerAngles.x, _car.transform.rotation.eulerAngles.y, _car.transform.rotation.eulerAngles.z + 180f);
                }
                Destroy(car, 10f);

                yield return new WaitForSeconds(Random.Range(_min, _max));
            }
        }
    }
}
