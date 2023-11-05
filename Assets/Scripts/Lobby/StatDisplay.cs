using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LewdieJam.Lobby
{
    public class StatDisplay : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _value;

        [SerializeField]
        private Button _add;

        public void UpdateValue(int value)
        {
            _value.text = value.ToString();
        }
    }
}
