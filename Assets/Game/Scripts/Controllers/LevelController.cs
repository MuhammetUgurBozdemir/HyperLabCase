using System;
using System.Collections.Generic;
using System.IO;
using Game.Scripts.Block;
using Game.Scripts.Grid;
using Game.Scripts.Settings;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Controllers
{
    public class LevelController : IInitializable, IDisposable
    {
        private List<BlockView> grids=new List<BlockView>();

        private LevelSettings _levelSettings;
        private DiContainer _diContainer;
        private PrefabSettings _prefabSettings;

        [Inject]
        private void Construct(LevelSettings levelSettings, DiContainer diContainer,
            PrefabSettings prefabSettings)
        {
            _levelSettings = levelSettings;
            _prefabSettings = prefabSettings;
            _diContainer = diContainer;
        }

        public void Initialize()
        {
            string json = _levelSettings.levels[0].text;
            
            LevelData data = JsonUtility.FromJson<LevelData>(json);

            foreach (GridData dataLevelGrid in data.levelGrids)
            {
                GridView view = _diContainer.InstantiatePrefabForComponent<GridView>(_prefabSettings.gridView);
                view.Init(dataLevelGrid);
            }
        }

        public void Dispose()
        {
        }
    }
}