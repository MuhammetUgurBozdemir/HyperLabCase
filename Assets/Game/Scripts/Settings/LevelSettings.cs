using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Settings
{
    [CreateAssetMenu(fileName = nameof(LevelSettings), menuName = "Settings/LevelSettings")]
    public class LevelSettings : ScriptableObject
    {
        public TextAsset[] levels;
    }
}