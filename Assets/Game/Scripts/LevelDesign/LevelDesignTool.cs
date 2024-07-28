using System;
using System.Collections.Generic;
using Game.Scripts.LevelDesign;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Windows;
using Directory = System.IO.Directory;
using File = System.IO.File;

namespace Game.Scripts.LevelDesign
{
    public class LevelDesignTool : MonoBehaviour
    {
        public List<Target> Targets;

        [OnValueChanged("CreateGrids")] public int length;

        [SerializeField] private Transform prefab;

        [SerializeField] public List<LevelDesignGrid> levelDesignGrids;


        private void CreateGrids()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }

            levelDesignGrids.Clear();

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    var obj = Instantiate(prefab, new Vector3(i - length / 2, 0, j), Quaternion.identity,
                        this.transform);
                    levelDesignGrids.Add(obj.GetComponent<LevelDesignGrid>());
                }
            }
        }

        [Button]
        public void ExportLevel()
        {
            LevelData data = new LevelData();

            foreach (LevelDesignGrid levelDesignGrid in levelDesignGrids)
            {
                GridData grid = new GridData
                {
                    blockAmount = levelDesignGrid.objects.Count,
                    blockColor = levelDesignGrid.color,
                    pos = new Vector2(levelDesignGrid.transform.position.x, levelDesignGrid.transform.position.z)
                };
                data.levelGrids.Add(grid);
            }

            foreach (Target target in Targets)
            {
                data.Targets.Add(target);
            }

            string fileName =
                "Level" + (Directory.GetFiles(Application.dataPath + "/Game/Levels/", "*.json").Length + 1);

            string levelJson = JsonUtility.ToJson(data);
            File.WriteAllText(Application.dataPath + "/Game/Levels/" + fileName + ".json", levelJson);
        }

        // [Button]
        // public void LoadLevel()
        // {
        //     string json = File.ReadAllText(Application.dataPath + "/Game/Levels/level1.json");
        //     LevelData data = JsonUtility.FromJson<LevelData>(json);
        //
        //     loadedGrids = data.levelGrids;
        // }

        [Button]
        private void ClearLevel()
        {
            CreateGrids();
        }
    }
}

[Serializable]
public class LevelData
{
    public List<GridData> levelGrids = new List<GridData>();
    public List<Target> Targets=new List<Target>();
}

[Serializable]
public class GridData
{
    public Color blockColor;
    public int blockAmount;
    public Vector2 pos;
}

[Serializable]
public class Target
{
    public int target;
    public Color Color;
}