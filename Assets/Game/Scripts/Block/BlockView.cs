using UnityEngine;

namespace Game.Scripts.Block
{
    public class BlockView : MonoBehaviour
    {
        [SerializeField] private new Renderer renderer;
        private MaterialPropertyBlock _materialPropertyBlock;
        private static readonly int Color1 = Shader.PropertyToID("_Color");

        public void ApplyColor(Color color)
        {
            _materialPropertyBlock ??= new MaterialPropertyBlock();
            
            _materialPropertyBlock.SetColor(Color1,color);
            renderer.SetPropertyBlock(_materialPropertyBlock);
        }

    }
}
