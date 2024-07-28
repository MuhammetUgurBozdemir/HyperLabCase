
using Game.Scripts.Block;
using Game.Scripts.Controllers;
using Game.Scripts.Grid;
using Game.Scripts.Settings;
using Zenject;

namespace Game.Scripts.Installers
{
    public class GameSceneInstaller : MonoInstaller<GameSceneInstaller>
    {
        #region Injection

        private PrefabSettings _prefabSettings;

        [Inject]
        private void Construct(PrefabSettings prefabSetting
        )
        {
            _prefabSettings = prefabSetting;
        }

        #endregion

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<LevelController>().AsSingle();
            
            InstallProducts();
        }

        
        private void InstallProducts()
        {
            Container.BindFactory<BlockView.Args, BlockView, BlockView.Factory>()
                .FromPoolableMemoryPool<BlockView.Args, BlockView, BlockView.Pool>(poolbinder =>
                    poolbinder
                        .WithInitialSize(20)
                        .FromComponentInNewPrefab(_prefabSettings.blockView)
                        .UnderTransformGroup("Blockes"));
            
        }
    }
}