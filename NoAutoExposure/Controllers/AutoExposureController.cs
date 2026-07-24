using System;
using System.Collections.Generic;
using IPA.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace NoAutoExposure
{
    internal sealed class AutoExposureController : IInitializable, IDisposable
    {
        private static readonly FieldAccessor<BloomPrePassBloomTextureEffectSO, PyramidBloomRendererSO.Pass>.Accessor
            FinalUpsamplePassAccessor =
                FieldAccessor<BloomPrePassBloomTextureEffectSO, PyramidBloomRendererSO.Pass>
                    .GetAccessor("_finalUpsamplePass");

        private readonly Config _config;
        private readonly Dictionary<BloomPrePassBloomTextureEffectSO, PyramidBloomRendererSO.Pass>
            _originalFinalUpsamplePasses =
                new Dictionary<BloomPrePassBloomTextureEffectSO, PyramidBloomRendererSO.Pass>();

        private bool _initialized;
        private bool _active;
        private PyramidBloomRendererSO.Pass _replacementPass;

        public AutoExposureController(Config config)
        {
            _config = config;
        }

        public void Initialize()
        {
            _config.StateChanged += HandleConfigChanged;
            SceneManager.sceneLoaded += HandleSceneLoaded;
            _initialized = true;
            ApplyConfig();
        }

        public void Dispose()
        {
            _initialized = false;
            _config.StateChanged -= HandleConfigChanged;
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            Deactivate();
        }

        private void HandleConfigChanged()
        {
            ApplyConfig();
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (_active)
            {
                PatchLoadedEffects();
            }
        }

        private void ApplyConfig()
        {
            if (!_initialized)
            {
                return;
            }

            if (!_config.Enabled)
            {
                Deactivate();
                return;
            }

            var replacementPass = _config.DisableToneMapping
                ? PyramidBloomRendererSO.Pass.UpsampleTent
                : PyramidBloomRendererSO.Pass.UpsampleTentAndACESToneMapping;

            if (_active && replacementPass != _replacementPass)
            {
                RestorePatchedEffects();
            }

            _replacementPass = replacementPass;

            if (!_active)
            {
                _active = true;
                SongCore.Collections.RegisterCapability(Plugin.CapabilityName);
                Plugin.Log.Info("Auto-exposure suppression enabled.");
            }

            PatchLoadedEffects();
        }

        private void Deactivate()
        {
            if (!_active)
            {
                return;
            }

            RestorePatchedEffects();
            SongCore.Collections.DeregisterCapability(Plugin.CapabilityName);
            _active = false;
            Plugin.Log.Info("Auto-exposure suppression disabled.");
        }

        private void PatchLoadedEffects()
        {
            var textureEffects = Resources.FindObjectsOfTypeAll<BloomPrePassBloomTextureEffectSO>();
            var patchedCount = 0;

            for (var i = 0; i < textureEffects.Length; i++)
            {
                var textureEffect = textureEffects[i];
                if (_originalFinalUpsamplePasses.ContainsKey(textureEffect))
                {
                    continue;
                }

                var finalUpsamplePass = FinalUpsamplePassAccessor(ref textureEffect);
                if (finalUpsamplePass !=
                    PyramidBloomRendererSO.Pass.UpsampleTentAndACESToneMappingGlobalIntensity)
                {
                    continue;
                }

                _originalFinalUpsamplePasses.Add(textureEffect, finalUpsamplePass);
                FinalUpsamplePassAccessor(ref textureEffect) = _replacementPass;
                patchedCount++;
                Plugin.Log.Info($"Patched {textureEffect.name}.");
            }

            if (patchedCount > 0)
            {
                Plugin.Log.Info($"Patched {patchedCount} newly loaded auto-exposure effect(s).");
            }
        }

        private void RestorePatchedEffects()
        {
            foreach (var originalPass in _originalFinalUpsamplePasses)
            {
                var textureEffect = originalPass.Key;
                if (textureEffect == null)
                {
                    continue;
                }

                FinalUpsamplePassAccessor(ref textureEffect) = originalPass.Value;
                Plugin.Log.Info($"Restored {textureEffect.name}.");
            }

            _originalFinalUpsamplePasses.Clear();
        }
    }
}
