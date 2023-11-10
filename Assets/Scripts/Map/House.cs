using LewdieJam.Game;
using LewdieJam.Player;
using LewdieJam.SO;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace LewdieJam.Map
{
    public class House : MonoBehaviour, IInteractible
    {
        [SerializeField]
        private HouseInfo _info;

        [SerializeField]
        private TMP_Text _requirement;

        private bool CanEnterHouse
            => PersistentData.Energy + PersistentData.PendingEnergy >= _info.EnergyRequired;

        private string OpenOkText => "Press E to open the house";
        public string NotEnoughEnergyText => "You don't have enough energy";
        public string OpenInfoText => $"{_info.EnergyRequired} energy required";

        private void Awake()
        {
            _requirement.text = OpenInfoText;

            var tl = GetComponentInChildren<TriggerListener>();
            Assert.IsNotNull(tl);
            tl.OnTriggerEnterCallback.AddListener(OnTriggerEnterEvt);
            tl.OnTriggerExitCallback.AddListener(OnTriggerExitEvt);
        }

        private void OnTriggerEnterEvt(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (CanEnterHouse)
                {
                    _requirement.color = Color.green;
                    _requirement.text = OpenOkText;
                }
                else
                {
                    _requirement.color = Color.red;
                    _requirement.text = NotEnoughEnergyText;
                }
                other.GetComponent<PlayerController>().CurrentInteraction = this;
            }
        }

        private void OnTriggerExitEvt(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _requirement.color = Color.white;
                _requirement.text = OpenInfoText;
                other.GetComponent<PlayerController>().CurrentInteraction = null;
            }
        }

        public bool CanInteract(PlayerController pc)
        {
            return CanEnterHouse;
        }

        public void Interact()
        {
            if (_info.EnergyRequired > PersistentData.PendingEnergy)
            {
                var rest = _info.EnergyRequired - PersistentData.PendingEnergy;
                PersistentData.PendingEnergy = 0;
                PersistentData.Energy -= rest;
            }
            else
            {
                PersistentData.PendingEnergy -= _info.EnergyRequired;
            }
            GameManager.Instance.ToggleHScene(true);
        }
    }
}
