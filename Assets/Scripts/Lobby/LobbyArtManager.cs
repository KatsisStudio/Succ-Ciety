using LewdieJam.Game;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace LewdieJam.Lobby
{
    public class LobbyArtManager : MonoBehaviour
    {
        [SerializeField]
        private Image _baseImage, _hornImage;

        [SerializeField]
        private AttachmentArtInfo[] _info;

        [SerializeField]
        private Sprite[] _horns;

        public void ToggleAttachment(Attachment attachment)
        {
            _baseImage.sprite = _info.First(x => x.Attachment == attachment).Sprite;
        }

        public void SetHornLevel(int level)
        {
            if (level > 0) level--;
            _hornImage.sprite = _horns[level];
        }
    }

    [System.Serializable]
    public class AttachmentArtInfo
    {
        public Attachment Attachment;
        public Sprite Sprite;
    }
}
