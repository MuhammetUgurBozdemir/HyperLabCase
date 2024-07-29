using System;
using System.Collections.Generic;
using Game.Scripts.Block;
using Game.Scripts.Controllers;
using Game.Scripts.Settings;
using Game.Scripts.UiView;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Screen
{
    public class GameScreenView : MonoBehaviour
    {
        private List<TargetView> _targetViews = new List<TargetView>();
        [SerializeField] private TargetView targetPrefab;
        [SerializeField] private Transform holder;
        [SerializeField] private CounterText counterText;
        [SerializeField] private Transform CompletedPanel;
        [SerializeField] private TextMeshProUGUI moveCount;

        private DiContainer _diContainer;
        private CameraController _cameraController;
        private LevelController _levelController;

        [Inject]
        private void Construct([Inject(Id = "CameraController")] CameraController cameraController,
            LevelController levelController, DiContainer diContainer)
        {
            _levelController = levelController;
            _cameraController = cameraController;
            _diContainer = diContainer;
        }

        public void Init()
        {
            foreach (Target levelDataTarget in _levelController._levelData.Targets)
            {
                var obj = _diContainer.InstantiatePrefabForComponent<TargetView>(targetPrefab, holder);
                obj.Init(levelDataTarget.Color, levelDataTarget.target);
                _targetViews.Add(obj);
            }

            UpdateMoveCount();
        }

        public void UpdateMoveCount()
        {
            moveCount.text = _levelController._levelData.moveCount.ToString();
        }

        public void LevelCompleted()
        {
            CompletedPanel.gameObject.SetActive(true);

            for (int i = _targetViews.Count - 1; i >= 0; i--)
            {
                Destroy(_targetViews[i].gameObject);
            }

            _targetViews.Clear();
        }

        public void PlayButton()
        {
            _levelController.ClearLevel();
            CompletedPanel.gameObject.SetActive(false);
        }

        public void UpdateCounter()
        {
            counterText.gameObject.SetActive(true);
            counterText.UpdateCounter();
        }

        public void UpdateTargets(int count, int index)
        {
            _targetViews[index].UpdateText(count);
        }

        public void CameraTransition()
        {
            _cameraController.CameraTransition();
        }
    }
}