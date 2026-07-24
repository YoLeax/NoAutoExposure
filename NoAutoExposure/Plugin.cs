using IPA;
using IPA.Config.Stores;
using IPA.Loader;
using NoAutoExposure.Installers;
using SiraUtil.Zenject;
using IPAConfig = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;

namespace NoAutoExposure
{
    [Plugin(RuntimeOptions.DynamicInit), NoEnableDisable]
    internal sealed class Plugin
    {
        internal const string CapabilityName = "No Auto-Exposure";

        internal static IPALogger Log { get; private set; } = null!;
        internal static Config Conf { get; private set; } = null!;

        [Init]
        public Plugin(
            IPALogger logger,
            IPAConfig conf,
            Zenjector zenjector,
            PluginMetadata pluginMetadata)
        {
            Log = logger;
            Conf = conf.Generated<Config>();

            zenjector.UseLogger(Log);
            zenjector.Install<AppInstaller>(Location.App, Conf);
            zenjector.Install<MenuInstaller>(Location.Menu);
            Log.Info($"{pluginMetadata.Name} {pluginMetadata.HVersion} initialized.");
        }
    }
}
