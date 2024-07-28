using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Game.Scripts.Block;
using Game.Scripts.Controllers;
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


        private DiContainer _diContainer;
        private PrefabSettings _prefabSettings;
        private LevelController _levelController;

        [Inject]
        private void Construct(DiContainer diContainer, PrefabSettings prefabSettings, LevelController levelController)
        {
            _diContainer = diContainer;
            _prefabSettings = prefabSettings;
            _levelController = levelController;
        }

        public void Init(GridData data)
        {
            _data = data;
            transform.position = new Vector3(_data.pos.x, 0, _data.pos.y);

            pos = _data.pos;

            for (int i = 0; i < data.blockAmount; i++)
            {
                BlockView view = _diContainer.InstantiatePrefabForComponent<BlockView>(_prefabSettings.blockView);
                view.transform.SetParent(transform);
                view.transform.position = new Vector3(_data.pos.x, .15f * (i + 1), data.pos.y);
                BlockViews.Add(view);
                view.ApplyColor(data);
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
                        
                        yield return new WaitForSeconds(.10f);
                    }

                    yield return new WaitForSeconds(.10f / blockViews.Count());
                }
            }

            yield return new WaitForSeconds(.20f / views.Count());

            var largeGroups = views.Where(x => x.Count() > 4).SelectMany(x => x).OrderByDescending(x=>x.transform.position.y).ToList();

            if (largeGroups.Count > 0)
            {
                _levelController.SpawnNewBlock();
            }
            foreach (var blockView in largeGroups)
            {
                BlockViews.Remove(blockView);
                blockView.DestroyView();
                yield return new  WaitForSeconds(.1f);
            }
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
    }
}