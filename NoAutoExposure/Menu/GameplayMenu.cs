using System;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.GameplaySetup;
using Zenject;

namespace NoAutoExposure.Menu
{
    internal sealed class GameplayMenu : IInitializable, IDisposable
    {
        private const string MenuName = "No Auto-Exposure";
        private const string ResourcePath = "NoAutoExposure.Menu.gameplayMenu.bsml";

        private readonly Config _config;

        public GameplayMenu(Config config)
        {
            _config = config;
        }

        public void Initialize()
        {
            GameplaySetup.Instance.AddTab(MenuName, ResourcePath, this);
            Plugin.Log.Info("Registered Gameplay Setup tab.");
        }

        public void Dispose()
        {
            if (GameplaySetup.Instance != null)
            {
                GameplaySetup.Instance.RemoveTab(MenuName);
            }
        }

        [UIValue("enabled")]
        private bool Enabled
        {
            get => _config.Enabled;
            set => _config.Enabled = value;
        }
    }
}
