using LewdieJam.Persistency;
using UnityEngine;

namespace LewdieJam
{
    public class HouseManager : MonoBehaviour
    {
        public static HouseManager Instance { get; private set; }

        [SerializeField]
        private Transform _vanillaHouse, _bigBreastsHouse, _futanariHouse, _pregnantHouse;

        private void Awake()
        {
            Instance = this;
        }

        public Transform GetNextTarget()
        {
            var visited = PersistencyManager.Instance.SaveData.VisitedHouses;
            if (!visited.Contains(Game.Attachment.None)) return _vanillaHouse;
            if (!visited.Contains(Game.Attachment.LargeBreasts)) return _bigBreastsHouse;
            if (!visited.Contains(Game.Attachment.Futanari)) return _futanariHouse;
            if (!visited.Contains(Game.Attachment.Pregnant)) return _pregnantHouse;
            return null;
        }
    }
}
