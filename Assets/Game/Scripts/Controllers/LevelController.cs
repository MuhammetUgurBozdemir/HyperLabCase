using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game.Scripts.Block;
using Game.Scripts.Grid;
using Game.Scripts.Settings;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Controllers
{
    public class LevelController : IInitializable, IDisposable
    {
        private readonly Dictionary<Vector2, GridView> _gridDictionary = new Dictionary<Vector2, GridView>();
        private int _gridCounter;
        private int _counter;

        readonly Vector2[] _neighborOffsets = new Vector2[]
        {
            new Vector2(1, 0),
            new Vector2(-1, 0),
            new Vector2(0, 1),
            new Vector2(0, -1)
        };

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
                _gridDictionary.Add(view.pos, view);
            }
        }

        public void CheckGridsForMatchedColor(GridView view, Color color)
        {
            _counter = 0;
            
            foreach (var offset in _neighborOffsets)
            {
                Vector2 neighborPos = view.pos + offset;

                if (!_gridDictionary.TryGetValue(neighborPos, out GridView neighborGrid)) continue;

                view.BlockViews.AddRange(neighborGrid.BlockViews);
                neighborGrid.BlockViews.Clear();

                _counter++;
            }

            view.StartCoroutineFromController();
        }

        public void SpawnNewBlock()
        {
            var counter = 0;
            GridView tempGridView = null;

            foreach (KeyValuePair<Vector2, GridView> gridView in _gridDictionary)
            {
                foreach (var offset in _neighborOffsets)
                {
                    Vector2 neighborPos = gridView.Value.pos + offset;

                    if (!_gridDictionary.TryGetValue(neighborPos, out GridView neighborGrid)) continue;

                    if (gridView.Value.BlockViews.Count == 0)
                    {
                        counter++;
                    }
                }

                if (counter > _gridCounter)
                {
                    _gridCounter = counter;
                    tempGridView = gridView.Value;
                }

                counter = 0;
            }

            if (tempGridView == null)
            {
                tempGridView = _gridDictionary[Vector2.zero];
            }
            
            GridData levelGrid = new GridData
            {
                blockColor = default,
                blockAmount = _counter,
                pos = tempGridView.pos
            };

            tempGridView.Init(levelGrid);
            
            _counter = 0;
        }

        public void Dispose()
        {
        }
    }
}