using Game.Scripts.Block;
using Game.Scripts.Settings;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Grid
{
    public class GridView : MonoBehaviour
    {
        private GridData _data;
        
        
        private DiContainer _diContainer;
        private PrefabSettings _prefabSettings;
        
        [Inject]
        private void Construct( DiContainer diContainer,
            PrefabSettings prefabSettings)
        {
            _diContainer = diContainer;
            _prefabSettings = prefabSettings;
        }
        
        public void Init(GridData data)
        {
            _data = data;
            transform.position = data.pos;

            for (int i = 0; i < data.blockAmount; i++)
            {
                BlockView view = _diContainer.InstantiatePrefabForComponent<BlockView>(_prefabSettings.blockView);
                view.transform.SetParent(transform);
                view.transform.position = new Vector3(_data.pos.x, .15f * (i + 1), data.pos.z);
                view.ApplyColor(data.blockColor);
            }
        }
    }
}
