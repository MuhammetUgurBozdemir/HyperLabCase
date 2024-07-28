using System;
using System.Collections.Generic;
using Game.Scripts.Block;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Scripts.LevelDesign
{
    public class LevelDesignGrid : MonoBehaviour
    {
        [OnValueChanged("CreateObjects")] 
        public Color32 color;

        [OnValueChanged("CreateObjects")] [SerializeField]
        private int objectAmount;

        [SerializeField] private Transform prefab;

        public List<BlockView> objects;

        private float _yPos = .15f;

        private void CreateObjects()
        {
            _yPos = .15f;
            objects.Clear();

            for (int i = transform.childCount - 1; i >= 1; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }

            for (int i = 0; i < objectAmount; i++)
            {
                var obj = Instantiate(prefab, new Vector3(transform.position.x, _yPos, transform.position.z),
                    Quaternion.Euler(90, 0, 0), this.transform);
                obj.GetComponent<BlockView>().ApplyColor(new GridData
                {
                    blockColor = color
                });
                objects.Add(obj.GetComponent<BlockView>());
                _yPos += .15f;
            }
        }
    }
}