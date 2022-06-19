using BepInEx;

namespace R2API;

[BepInPlugin(TempVisualEffectAPI.PluginGUID, TempVisualEffectAPI.PluginName, TempVisualEffectAPI.PluginVersion)]
public sealed class TempVisualEffectPlugin : BaseUnityPlugin
{
    private void OnEnable()
    {
        TempVisualEffectAPI.SetHooks();
    }

    private void OnDisable()
    {
        TempVisualEffectAPI.UnsetHooks();
    }
}