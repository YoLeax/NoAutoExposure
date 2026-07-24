using Zenject;

namespace NoAutoExposure.Installers
{
    internal sealed class AppInstaller : Installer
    {
        private readonly Config _config;

        public AppInstaller(Config config)
        {
            _config = config;
        }

        public override void InstallBindings()
        {
            Container.BindInstance(_config).AsSingle();
            Container.BindInterfacesTo<AutoExposureController>().AsSingle();
        }
    }
}
