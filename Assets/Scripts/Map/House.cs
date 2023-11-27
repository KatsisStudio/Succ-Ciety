using LewdieJam.Game;
using LewdieJam.Persistency;
using LewdieJam.Player;
using LewdieJam.SO;
using LewdieJam.VN;
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

        private bool HaveEnoughEnergy => PersistencyManager.Instance.SaveData.Energy + PersistencyManager.Instance.SaveData.PendingEnergy >= _info.EnergyRequired;
        private bool HaveRightAttachments => _info.RequiredAttachment == Attachment.None || PersistencyManager.Instance.SaveData.Attachments == _info.RequiredAttachment;

        private bool CanEnterHouse
            => HaveEnoughEnergy && HaveRightAttachments;

        private string OpenOkText => "Press E to open the house";
        public string IncorrectAttachmentText => "You don't have the right attachment";
        public string NotEnoughEnergyText => "You don't have enough energy";
        public string OpenInfoText => $"{_info.EnergyRequired} energy required" + (_info.RequiredAttachment != Attachment.None ? $"\n{_info.RequiredAttachment} required" : string.Empty);

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
                UpdateCanEnterUI();
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

        private void UpdateCanEnterUI()
        {
            if (CanEnterHouse)
            {
                _requirement.color = Color.green;
                _requirement.text = OpenOkText;
            }
            else
            {
                _requirement.color = Color.red;
                if (!HaveRightAttachments)
                {
                    _requirement.text = IncorrectAttachmentText;
                }
                else
                {
                    _requirement.text = NotEnoughEnergyText;
                }
            }
        }

        public bool CanInteract(PlayerController pc)
        {
            return CanEnterHouse;
        }

        public void Interact()
        {
            if (_info.EnergyRequired > PersistencyManager.Instance.SaveData.PendingEnergy)
            {
                var rest = _info.EnergyRequired - PersistencyManager.Instance.SaveData.PendingEnergy;
                PersistencyManager.Instance.SaveData.PendingEnergy = 0;
                PersistencyManager.Instance.SaveData.Energy -= rest;
            }
            else
            {
                PersistencyManager.Instance.SaveData.PendingEnergy -= _info.EnergyRequired;
            }
            GameManager.Instance.UpdateUI();
            VNManager.Instance.ShowOpenDoorQuestion(_info.HScene);
            if (!PersistencyManager.Instance.SaveData.VisitedHouses.Contains(_info.RequiredAttachment))
            {
                PersistencyManager.Instance.SaveData.VisitedHouses.Add(_info.RequiredAttachment);
            }
            PersistencyManager.Instance.Save();
            UpdateCanEnterUI();
        }
    }
}
