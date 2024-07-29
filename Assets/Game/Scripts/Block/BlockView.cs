using System.Collections.Generic;
using DG.Tweening;
using Game.Scripts.Grid;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Game.Scripts.Block
{
    public class BlockView : MonoBehaviour, IPoolable<BlockView.Args, IMemoryPool>
    {
        [SerializeField] private new Renderer renderer;
        private MaterialPropertyBlock _materialPropertyBlock;
        private static readonly int Color1 = Shader.PropertyToID("_Color");
        [SerializeField] private new ParticleSystem particleSystem;

        public List<BlockView> parentList;

        public GridData data;

        private IMemoryPool _pool;

        public void ApplyColor(GridData data, List<BlockView> list)
        {
            parentList = list;
            this.data = data;
            _materialPropertyBlock ??= new MaterialPropertyBlock();

            _materialPropertyBlock.SetColor(Color1, data.blockColor);
            renderer.SetPropertyBlock(_materialPropertyBlock);
        }

        public void OnDespawned()
        {
            var particle = particleSystem.transform;
            particleSystem.gameObject.SetActive(true);
            particleSystem.transform.SetParent(null);
            transform.DOScale(Vector3.zero, .1f).OnComplete(() =>
            {
                particle.transform.DOScale(new Vector3(.15f,.15f,.15f), 0);
                particle.transform.SetParent(transform);
            });
        }

        public void OnSpawned(Args args, IMemoryPool p2)
        {
            ApplyColor(args.Data, args.List);
            _pool = p2;
            transform.DOScale(Vector3.one, 0);
        }

        public void DespawnView()
        {
            _pool.Despawn(this);
        }

        public class Factory : PlaceholderFactory<Args, BlockView>
        {
        }

        public class Pool : MonoPoolableMemoryPool<Args, IMemoryPool, BlockView>
        {
        }

        public readonly struct Args
        {
            public readonly GridData Data;
            public readonly List<BlockView> List;

            public Args(GridData data, List<BlockView> list) : this()
            {
                Data = data;
                List = list;
            }
        }
    }
}