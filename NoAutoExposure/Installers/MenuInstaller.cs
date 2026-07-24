using NoAutoExposure.Menu;
using Zenject;

namespace NoAutoExposure.Installers
{
    internal sealed class MenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<GameplayMenu>().AsSingle();
        }
    }
}
