using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;

namespace GravityMonke.Patches
{
    [HarmonyPatch(typeof(GorillaLocomotion.Player))]
    [HarmonyPatch("Awake", MethodType.Normal)]
    class PlayerPatch
    {
        private static void Postfix(GorillaLocomotion.Player __instance)
        {
            __instance.gameObject.AddComponent<GravityMonke.WD.WristDisplay>();
        }

    }
}
