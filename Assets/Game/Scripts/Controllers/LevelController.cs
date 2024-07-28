using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game.Scripts.Block;
using Game.Scripts.Grid;
using Game.Scripts.Settings;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Game.Scripts.Controllers
{
    public class LevelController : IInitializable, IDisposable
    {
        private readonly Dictionary<Vector2, GridView> _gridDictionary = new Dictionary<Vector2, GridView>();
        private int _gridCounter;
        private int _counter;
        private int currentLevel;
        private LevelData _levelData;

        public bool isMoving = false;

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
            InitLevel(0);
        }

        private void InitLevel(int index)
        {
            string json = _levelSettings.levels[index].text;

            LevelData data = JsonUtility.FromJson<LevelData>(json);
            _levelData = data;

            foreach (GridData dataLevelGrid in _levelData.levelGrids)
            {
                GridView view = _diContainer.InstantiatePrefabForComponent<GridView>(_prefabSettings.gridView);
                view.Init(dataLevelGrid);
                _gridDictionary.Add(view.pos, view);
            }
        }

        public void CheckGridsForMatchedColor(GridView view, Color color)
        {
            if (isMoving) return;

            _counter = 0;

            foreach (var offset in _neighborOffsets)
            {
                Vector2 neighborPos = view.pos + offset;

                if (!_gridDictionary.TryGetValue(neighborPos, out GridView neighborGrid)) continue;

                if (neighborGrid.BlockViews.Count > 0)
                {
                    _counter++;
                }
                view.BlockViews.AddRange(neighborGrid.BlockViews);
            }

            if (_counter == 0) return;

            view.StartCoroutineFromController();
        }

        public void CheckIsTargetReached(int amount, Color color)
        {
            _levelData.Targets.Find(clr => clr.Color == color).target -= amount;

            Debug.Log(_levelData.Targets.Find(clr => clr.Color == color).target);

            bool isCompleted = _levelData.Targets.Any(cnt => cnt.target > 0);

            if (isCompleted) return;

            currentLevel++;

            if (currentLevel >= _levelSettings.levels.Count())
            {
                currentLevel = 0;
            }

            ClearLevel();
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

            List<Color> listColor = new List<Color>();

            foreach (var target in _levelData.Targets)
            {
                for (int i = 0; i < target.target; i++)
                {
                    listColor.Add(target.Color);
                }
            }

            GridData levelGrid = new GridData
            {
                blockColor = listColor[Random.Range(0, listColor.Count)],
                blockAmount = _counter,
                pos = tempGridView.pos
            };

            tempGridView.Init(levelGrid, true);

            _counter = 0;
        }

        public void ClearLevel()
        {
            foreach (var keyValuePair in _gridDictionary)
            {
                keyValuePair.Value.Dispose();
            }

            _gridDictionary.Clear();

            InitLevel(currentLevel);
        }

        public void Dispose()
        {
        }
    }
}