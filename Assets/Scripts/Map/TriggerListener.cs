using UnityEngine;
using UnityEngine.Events;

namespace LewdieJam.Map
{
    public class TriggerListener : MonoBehaviour
    {
        public UnityEvent<Collider> OnTriggerEnterCallback { get; } = new();
        public UnityEvent<Collider> OnTriggerExitCallback { get; } = new();

        public void OnTriggerEnter(Collider other)
        {
            OnTriggerEnterCallback.Invoke(other);
        }

        public void OnTriggerExit(Collider other)
        {
            OnTriggerExitCallback.Invoke(other);
        }
    }
}
