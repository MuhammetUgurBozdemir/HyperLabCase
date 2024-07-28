using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts.Block
{
    public class BlockView : MonoBehaviour
    {
        [SerializeField] private new Renderer renderer;
        private MaterialPropertyBlock _materialPropertyBlock;
        private static readonly int Color1 = Shader.PropertyToID("_Color");
        [SerializeField] private new ParticleSystem particleSystem;


        public GridData data;

        public void ApplyColor(GridData data)
        {
            this.data = data;
            _materialPropertyBlock ??= new MaterialPropertyBlock();

            _materialPropertyBlock.SetColor(Color1, data.blockColor);
            renderer.SetPropertyBlock(_materialPropertyBlock);
        }

        public void DestroyView()
        {
            particleSystem.gameObject.SetActive(true);
            particleSystem.transform.SetParent(null);
            transform.DOScale(Vector3.zero, .1f).OnComplete(()=>Destroy(gameObject));
        }
    }
}