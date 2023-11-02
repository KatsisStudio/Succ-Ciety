using UnityEngine;
using YuriGameJam2023.SO;

namespace LewdieJam
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { private set; get; }

        [SerializeField]
        private GameInfo _info;

        private void Awake()
        {
            Instance = this;
        }
    }
}
