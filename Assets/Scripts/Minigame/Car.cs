using UnityEngine;

namespace LewdieJam.Minigame
{
    public class Car : MonoBehaviour
    {
        public float Speed { set; private get; }

        private void FixedUpdate()
        {
            GetComponent<Rigidbody>().velocity = -transform.up * Speed;
        }
    }
}
