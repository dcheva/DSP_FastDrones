using BepInEx;
using HarmonyLib;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace com.dkoppstein.plugin.DSP.FastDrones
{
    [BepInPlugin("com.dkoppstein.plugin.DSP.FastDrones", "FastDrones", "0.0.5")]
    [BepInProcess("DSPGAME.exe")]
    public class FastDronesPlugin : BaseUnityPlugin
    {
        private static readonly float DRONE_SPEED = 299.0F; // max of MAX_DIST_DELTA / min(dt) (probably 0.01)
        private static readonly double DECREASE_DRONE_ENERGY_FACTOR = 50.0; 
        private static readonly float MAX_DIST_DELTA = 3f; // minimum distance from drone to player/target before state changes

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
            public static bool ChangeDroneSpeedPrefix(ref MechaDrone __instance,
                                                      Vector3 playerPos,
                                                      ref int __result,
                                                      ref double energyRate, 
                                                      out float __state)
            {
                //FileLog.Log(string.Format("Drone speed before prefix is: {0}", (object)__instance.speed));
                //FileLog.Log(string.Format("Energy rate before prefix is: {0}", (object) energyRate));
                __state = __instance.speed; // remember original speed
                __instance.speed = FastDronesPlugin.DRONE_SPEED;
                energyRate = (double)energyRate / (double)FastDronesPlugin.DECREASE_DRONE_ENERGY_FACTOR;
                //FileLog.Log(string.Format("New drone speed after prefix is: {0}", (object)__instance.speed));
                //FileLog.Log(string.Format("New energy rate after prefix is: {0}", (object) energyRate));

                // thanks to Appun for the following code and figuring out 3f
                Vector3 toTargetVector = new Vector3(__instance.target.x - __instance.position.x, __instance.target.y - __instance.position.y, __instance.target.z - __instance.position.z);
                float distToTarget = (float)Math.Sqrt((double)(toTargetVector.x * toTargetVector.x + toTargetVector.y * toTargetVector.y + toTargetVector.z * toTargetVector.z));

                if (__instance.stage == 2 && distToTarget < FastDronesPlugin.MAX_DIST_DELTA) // drone is building something and near target
                    {
                        __result = 1; // build the thing
                        return false;
                    }

                Vector3 toPlayerVector = new Vector3(playerPos.x - __instance.position.x, playerPos.y - __instance.position.y, playerPos.z - __instance.position.z);
                float distToPlayer = (float)Math.Sqrt((double)(toPlayerVector.x * toPlayerVector.x + toPlayerVector.y * toPlayerVector.y + toPlayerVector.z * toPlayerVector.z));

                if (__instance.stage == 3 && distToPlayer < FastDronesPlugin.MAX_DIST_DELTA) // drone is coming home and near home
                {
                    __instance.Reset(); 
                    __result = 0; 
                    return false;
                }

                return true;
            }


            [HarmonyPostfix]
            [HarmonyPatch("Update")]
            public static void ChangeDroneSpeedPostfix(ref MechaDrone __instance,
                                                      float __state)
            {
                //FileLog.Log(string.Format("Drone speed before postfix is: {0}", (object)__instance.speed));
                //FileLog.Log(string.Format("Energy rate before postfix is: {0}", (object) energyRate));
                __instance.speed = __state; // reset speed back to original so as not to interfere with objects
                //FileLog.Log(string.Format("New drone speed after postfix is: {0}", (object)__instance.speed));
                //FileLog.Log(string.Format("New energy rate after postfix is: {0}", (object) energyRate));
            }
        }
    }
}
