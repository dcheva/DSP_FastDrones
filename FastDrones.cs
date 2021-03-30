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
        private static readonly float INCREMENT_DRONE_SPEED = 42.0F;
        private static readonly double DECREASE_DRONE_ENERGY_FACTOR = 50.0;

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
            public static void ChangeDroneSpeedPrefix(ref MechaDrone __instance,
                                                      PrebuildData[] prebuildPool,
                                                      Vector3 playerPos,
                                                      float dt,
                                                      ref double energy,
                                                      ref double energyChange,
                                                      ref double energyRate)
            {
                //FileLog.Log(string.Format("Drone speed before prefix is: {0}", (object)__instance.speed));
                //FileLog.Log(string.Format("Energy rate before prefix is: {0}", (object) energyRate));
                __instance.speed += FastDronesPlugin.INCREMENT_DRONE_SPEED;
                energyRate = (double)energyRate / (double)FastDronesPlugin.DECREASE_DRONE_ENERGY_FACTOR;
                //FileLog.Log(string.Format("New drone speed after prefix is: {0}", (object)__instance.speed));
                //FileLog.Log(string.Format("New energy rate after prefix is: {0}", (object) energyRate));
            }
            [HarmonyPostfix]
            [HarmonyPatch("Update")]
            public static void ChangeDroneSpeedPostfix(ref MechaDrone __instance,
                                                      PrebuildData[] prebuildPool,
                                                      Vector3 playerPos,
                                                      float dt,
                                                      ref double energy,
                                                      ref double energyChange,
                                                      ref double energyRate)
            {
                //FileLog.Log(string.Format("Drone speed before postfix is: {0}", (object)__instance.speed));
                //FileLog.Log(string.Format("Energy rate before postfix is: {0}", (object) energyRate));
                __instance.speed -= FastDronesPlugin.INCREMENT_DRONE_SPEED;
                //FileLog.Log(string.Format("New drone speed after postfix is: {0}", (object)__instance.speed));
                //FileLog.Log(string.Format("New energy rate after postfix is: {0}", (object) energyRate));
            }
        }
    }
}
