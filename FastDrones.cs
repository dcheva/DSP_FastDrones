using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;


namespace FastDrones
{
    [BepInPlugin("com.dkoppstein.plugin.DSP.FastDrones", "FastDrones", "0.0.1")]
    [BepInProcess("DSPGAME.exe")]
    public class FastDronesPlugin : BaseUnityPlugin
    {
        public static ManualLogSource Logger;

        private void Start() {
            FastDronesPlugin.Logger = this.get_Logger();
            PluginConfig.Init(this.get_Config());
            new Harmony("com.dkoppstein.plugin.DSP.FastDrones").PatchAll(typeof(FastDronesPlugin.MechaDronePatch));
            FastDronesPlugin.Logger.LogInfo((object) string.Format("IncreaseMaxWarpSpeed loaded with speed updated to {0} {1}", (object) PluginConfig.WarpSpeed.get_Value(), (object) PluginConfig.SpeedType.get_Value()));
        }


        public FastDronesPlugin() => base.\u002Ector();

        [HarmonyPatch(typeof(MechaDrone))]
        public class MechaDronePatch
        {
          internal static float getNewDroneSpeed()
          {
            double num = PluginConfig.WarpSpeed.get_Value();
            return num
          }

          [HarmonyPrefix]
          [HarmonyPatch("Update")]
          public static bool ChangeDroneSpeedPrefix(MechaDrone __drone,
                                                    PrebuildData[] prebuildPool,
                                                    Vector3 playerPos,
                                                    float dt,
                                                    ref double energy,
                                                    ref double energyChange,
                                                    double energyRate)
          {
            Console.WriteLine(string.Format("Drone speed before prefix is: {0}", (object) __drone.speed));
            Console.WriteLine(string.Format("Energy rate before prefix is: {0}", (object) energyRate));
            __drone.speed += FastDronesPlugin.IncrementDroneSpeed;
            energyRate = double (energyRAte / FastDronesPlugin.DecreaseDroneEnergyFactor / 5)
            Console.WriteLine(string.Format("New drone speed after prefix is: {0}", (object) __drone.speed));
            Console.WriteLine(string.Format("New energy rate after prefix is: {0}", (object) energyRate));
            return true;
          }
          [HarmonyPostfix]
          [HarmonyPatch("Update")]
          public static bool ChangeDroneSpeedPostfix(MechaDrone __drone,
                                                     PrebuildData[] prebuildPool,
                                                     Vector3 playerPos,
                                                     float dt,
                                                     ref double energy,
                                                     ref double energyChange,
                                                     double energyRate)
          {
            Console.WriteLine(string.Format("Drone speed before postfix is: {0}", (object) __drone.speed));
            __drone.speed -= FastDronesPlugin.IncrementDroneSpeed;
            Console.WriteLine(string.Format("New warp speed after postfix is: {0}", (object) __drone.speed));
            return true;
          }
        }
      }
    }
}
