using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LewdieJam.Lobby
{
    public class LobbyManager : MonoBehaviour
    {
        [SerializeField]
        private StatDisplay _healthStat, _atkPowerStat, _atkSpeedStat, _energyStat;

        [SerializeField]
        private TMP_Text _energy;

        [SerializeField]
        private TMP_Text _hornLevel;

        public void LoadGame()
        {
            SceneManager.LoadScene("Main");
        }

        private void Awake()
        {
            _energy.text = PersistentData.Energy.ToString();
            _hornLevel.text = PersistentData.HornLevel.ToString();
            _healthStat.UpdateValue(PersistentData.BaseHealth);
            _atkPowerStat.UpdateValue(PersistentData.AtkPower);
            _atkSpeedStat.UpdateValue(PersistentData.AtkSpeed);
            _energyStat.UpdateValue(PersistentData.EnergyScaling);
        }
    }
}
