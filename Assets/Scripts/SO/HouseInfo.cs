﻿using LewdieJam.Game;
using UnityEngine;

namespace LewdieJam.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/HouseInfo", fileName = "HouseInfo")]
    public class HouseInfo : ScriptableObject
    {
        public int EnergyRequired;
        public Attachment RequiredAttachment;
        public HSceneInfo HScene;
    }
}