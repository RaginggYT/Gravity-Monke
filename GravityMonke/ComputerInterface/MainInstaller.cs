using ComputerInterface;
using ComputerInterface.Interfaces;
using Zenject;

namespace GravityMonke.ComputerInterface
{
    internal class MainInstaller : Installer
    {
        public override void InstallBindings()
        {
            // Bind your mod entry like this
            Container.Bind<IComputerModEntry>().To<GravityEntry>().AsSingle();
        }
    }
}
