using BepInEx;
using HarmonyLib;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;


namespace com.dkoppstein.plugin.DSP.FastDrones
{
    [BepInPlugin("com.dkoppstein.plugin.DSP.FastDrones", "FastDrones", "0.0.1")]
    [BepInProcess("DSPGAME.exe")]
    public class FastDronesPlugin : BaseUnityPlugin
    {
        private static readonly float INCREMENT_DRONE_SPEED = 45.0F;
        private static readonly double DECREASE_DRONE_ENERGY_FACTOR = 5.0;

        private void Start() 
        {
            new Harmony("com.dkoppstein.plugin.DSP.FastDrones").PatchAll(typeof(FastDronesPlugin.MechaDronePatch));
        }


        public FastDronesPlugin() : base()
        {
            return;
        }

        [HarmonyPatch(typeof(MechaDrone))]
        public class MechaDronePatch
        {
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
            __drone.speed += FastDronesPlugin.INCREMENT_DRONE_SPEED;
            energyRate = (double)energyRate / (double)FastDronesPlugin.DECREASE_DRONE_ENERGY_FACTOR;
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
            __drone.speed -= FastDronesPlugin.INCREMENT_DRONE_SPEED;
            Console.WriteLine(string.Format("New warp speed after postfix is: {0}", (object) __drone.speed));
            return true;
          }
        }
     
    }
}
