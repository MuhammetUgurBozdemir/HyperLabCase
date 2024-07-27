
using Game.Scripts.Controllers;
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
        }

        
        // private void InstallVfxs()
        // {
        //     Container.BindFactory<ParticleView.Args, ParticleView, ParticleView.Factory>()
        //         .FromPoolableMemoryPool<ParticleView.Args, ParticleView, ParticleView.Pool>(poolbinder =>
        //             poolbinder
        //                 .WithInitialSize(20)
        //                 .FromComponentInNewPrefab(_prefabSettings.particleView)
        //                 .UnderTransformGroup("Vfxs"));
        //     
        // }
    }
}