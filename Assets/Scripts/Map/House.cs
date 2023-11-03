using LewdieJam.SO;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace LewdieJam.Map
{
    public class House : MonoBehaviour
    {
        [SerializeField]
        private HouseInfo _info;

        [SerializeField]
        private TMP_Text _requirement;

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
                if (GameManager.Instance.Energy >= _info.EnergyRequired)
                {
                    _requirement.color = Color.green;
                    _requirement.text = OpenOkText;
                }
                else
                {
                    _requirement.color = Color.red;
                    _requirement.text = NotEnoughEnergyText;
                }
            }
        }

        private void OnTriggerExitEvt(Collider other)
        {
            _requirement.color = Color.white;
            _requirement.text = OpenInfoText;
        }
    }
}
