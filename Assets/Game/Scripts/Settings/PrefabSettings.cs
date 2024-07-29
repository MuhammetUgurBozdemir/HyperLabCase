using Game.Scripts.Block;
using Game.Scripts.Grid;
using UnityEngine;

namespace Game.Scripts.Settings
{
    [CreateAssetMenu(fileName = nameof(PrefabSettings), menuName = "Settings/PrefabSettings")]
    public class PrefabSettings : ScriptableObject
    {
        public BlockView blockView;
        public GridView gridView;
    }
}