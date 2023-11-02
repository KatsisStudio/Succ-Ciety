using UnityEngine;

namespace LewdieJam
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { private set; get; }

        [SerializeField]
        private SO.GameInfo _info;

        private void Awake()
        {
            Instance = this;
        }
    }
}
