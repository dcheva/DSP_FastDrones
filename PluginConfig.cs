using BepInEx.Configuration;

namespace com.dkoppstein.plugin.DSP.FastDrones
{
  public static class PluginConfig
  {
    private static readonly string GENERAL_SECTION = "General";
    public static ConfigEntry<double> MechaDroneSpeed;
    public static ConfigEntry<double> MechaEnergyPerMeter;

    internal static void Init(ConfigFile config)
    {
      PluginConfig.IncrementDroneSpeed = (ConfigEntry<double>) config.Bind<double>(PluginConfig.IncrementDroneSpeed, "MechaDroneSpeed", 45.0, "Adds this amount to drone speed");
      PluginConfig.DecreaseDroneEnergyFactor = (ConfigEntry<double>) config.Bind<double>(PluginConfig.DecreaseDroneEnergyFactor, "MechaDroneSpeed", 5.0, "Decreases drone energy usage by this factor");
    }
  }
}
