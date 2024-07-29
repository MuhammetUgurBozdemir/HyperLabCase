using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Game.Scripts.Block;
using Game.Scripts.Controllers;
using Game.Scripts.Screen;
using Game.Scripts.Settings;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Grid
{
    public class GridView : MonoBehaviour
    {
        private GridData _data;
        public Vector2 pos;
        public List<BlockView> BlockViews;

        public float tempYPos;

        private DiContainer _diContainer;
        private PrefabSettings _prefabSettings;
        private LevelController _levelController;
        private BlockView.Factory _factory;
        private GameScreenView _gameScreenView;

        [Inject]
        private void Construct(DiContainer diContainer, PrefabSettings prefabSettings, LevelController levelController,
            BlockView.Factory factory,
            [Inject(Id = "GameScreenView")] GameScreenView gameScreenView)
        {
            _diContainer = diContainer;
            _prefabSettings = prefabSettings;
            _levelController = levelController;
            _factory = factory;
            _gameScreenView = gameScreenView;
        }

        public void Init(GridData data, bool isSpawned = false)
        {
            _data = data;
            transform.position = new Vector3(_data.pos.x, 0, _data.pos.y);

            pos = _data.pos;

            tempYPos = BlockViews.Count * .15f;
            
            for (int i = 0; i < data.blockAmount; i++)
            {
                BlockView view = _factory.Create(new BlockView.Args(data, BlockViews));

                view.transform.SetParent(transform);
                BlockViews.Add(view);
                view.ApplyColor(data, BlockViews);

                Vector3 newPosition = new Vector3(_data.pos.x, .15f * (i + 1), data.pos.y);

                if (isSpawned)
                {
                    newPosition.y += 10;
                    view.transform.position = newPosition;
                    view.transform
                        .DOMove(new Vector3(_data.pos.x, tempYPos+.15f * (i + 1), _data.pos.y), 0.5f)
                        .SetEase(Ease.InSine);
                }
                else
                {
                    view.transform.position = newPosition;
                }
            }
        }

        public void StartCoroutineFromController()
        {
            StartCoroutine(MoveAddedBlocksToPosAndCheckForDestroy());
        }

        private IEnumerator MoveAddedBlocksToPosAndCheckForDestroy()
        {
            var views = GroupAndSortColorObjects(BlockViews);
            var yPos = .15f;

            if (views.Any())
            {
                _levelController.isMoving = true;
            }

            foreach (var blockViews in views)
            {
                var groupedList = blockViews
                    .OrderByDescending(x => x.transform.position.y)
                    .GroupBy(x => x.data.pos);

                foreach (var grouping in groupedList)
                {
                    foreach (var blockView in grouping)
                    {
                        Vector2 axis = new Vector2(blockView.transform.position.x, blockView.transform.position.z) -
                                       new Vector2(transform.position.x, transform.position.z);

                        blockView.transform.DOJump(new Vector3(_data.pos.x, yPos, _data.pos.y), 1, 1, .3f)
                            .SetEase(Ease.InSine)
                            .OnComplete(() => blockView.transform.DORotate(Vector3.zero, 0));

                        blockView.transform.DORotate(new Vector3(-180 * axis.y, 0, 180 * axis.x), .20f)
                            .SetEase(Ease.InSine);

                        yPos += .15f;

                        blockView.parentList.Remove(blockView);
                        blockView.parentList = BlockViews;

                        yield return new WaitForSeconds(.10f);
                    }

                    yield return new WaitForSeconds(.10f / blockViews.Count());
                }
            }

            yield return new WaitForSeconds(.20f / views.Count());

            var largeGroups = views.Where(x => x.Count() > 4).SelectMany(x => x)
                .OrderByDescending(x => x.transform.position.y).ToList();

            if (largeGroups.Any())
            {
                _levelController.CheckIsTargetReached(largeGroups.Count, largeGroups[0].data.blockColor);
            }

            if (largeGroups.Count > 0)
            {
                _levelController.SpawnNewBlock();
            }

            foreach (var blockView in largeGroups)
            {
                BlockViews.Remove(blockView);
                blockView.DespawnView();
                _gameScreenView.UpdateCounter();
                yield return new WaitForSeconds(.1f);
            }

            _levelController.isMoving = false;
        }

        private IOrderedEnumerable<IGrouping<Color, BlockView>> GroupAndSortColorObjects(List<BlockView> matchedObjects)
        {
            var groupedObjects = matchedObjects
                .GroupBy(obj => obj.data.blockColor)
                .OrderBy(group => group.Count());

            return groupedObjects;
        }

        public void OnClick()
        {
            _levelController.CheckGridsForMatchedColor(this, _data.blockColor);
        }

        public void Dispose()
        {
            Destroy(gameObject);
        }
    }
}