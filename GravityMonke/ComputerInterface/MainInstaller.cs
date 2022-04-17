using ComputerInterface;
using ComputerInterface.Interfaces;
using Zenject;

namespace GravityMonke.ComputerInterface
{
    internal class MainInstaller : Installer
    {
        public override void InstallBindings()
        {
            base.Container.Bind<IComputerModEntry>().To<GravityEntry>().AsSingle();
        }
    }
}
