using System;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using ModestTree;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Game.Scripts.Controllers
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Vector3[] camPoses;
        [SerializeField] private Vector3[] camRot;
        private int _index = 0;


        private DiContainer _diContainer;
        private LevelController _levelController;

        [Inject]
        private void Construct(LevelController levelController)
        {
            _levelController = levelController;
        }

        private void Start()
        {
            var length = _levelController._levelData.length;

            transform.DOMove(camPoses[0], .3f);
            transform.DORotate(camRot[0], .3f);
        }

        public void CameraTransition()
        {
            Debug.Log(_index);

            _index = (_index < 3) ? _index + 1 : 0;

            transform.DOMove(camPoses[_index], .3f);
            transform.DORotate(camRot[_index], .3f);
        }
    }
}