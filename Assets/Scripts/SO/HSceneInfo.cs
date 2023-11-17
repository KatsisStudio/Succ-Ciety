using LewdieJam.Achievement;
using UnityEngine;

namespace LewdieJam.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/HSceneInfo", fileName = "HSceneInfo")]
    public class HSceneInfo : ScriptableObject
    {
        public string Name;
        public TextAsset Story;
        public Sprite[] Sprites;
        public AchievementID Achievement;
    }
}