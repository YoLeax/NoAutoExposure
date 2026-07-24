# NoAutoExposure

A Beat Saber 1.40.8 port of
[whatdahopper-archive/NoAutoExposure](https://github.com/whatdahopper-archive/NoAutoExposure).
It disables the bloom-based auto-exposure/global-intensity pass while preserving
ACES tone mapping by default.

The mod rescans after scene loads so newly loaded environments and Beat Saber's
internal restart are covered.

Use the `No Auto-Exposure` Gameplay Setup tab to enable or disable the effect
without restarting Beat Saber.

## Requirements

- Beat Saber 1.40.8
- BSIPA 4.3.6 or newer compatible version
- BeatSaberMarkupLanguage 1.12.5 or newer compatible version
- SiraUtil 3.2.1 or newer compatible version
- SongCore 3.15.3 or newer compatible version

## Configuration

The generated `UserData/No Auto-Exposure.json` file contains:

```json
{
  "Enabled": true,
  "DisableToneMapping": false
}
```

Set `DisableToneMapping` to `true` to also replace the ACES tone-mapping pass
with the plain tent upsample pass. Restart Beat Saber after changing it.

## Compatibility note

Modern lightshows may be authored around Beat Saber's current auto-exposure.
Disabling it intentionally changes their appearance.

## Development

The project uses the current SDK-style BSMT layout and targets .NET Framework
4.7.2. Set `GameDirectory` in the ignored
`NoAutoExposure/NoAutoExposure.csproj.user` file:

```xml
<Project>
  <PropertyGroup>
    <GameDirectory>C:\Path\To\Beat Saber</GameDirectory>
  </PropertyGroup>
</Project>
```

Build from the repository root:

```powershell
dotnet build .\NoAutoExposure.sln -c Release
```

BSMT generates the manifest, copies the plugin to the configured game instance,
and creates a distributable ZIP under
`NoAutoExposure/bin/Release/net472/zip/`.
