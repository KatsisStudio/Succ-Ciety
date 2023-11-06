using UnityEngine;
using UnityEngine.Events;

namespace LewdieJam.Lobby
{
    public class SuccubusClickCallback : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent _onClick;

        private void OnMouseUpAsButton()
        {
            _onClick?.Invoke();
        }
    }
}
