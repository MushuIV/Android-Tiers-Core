using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace MOARANDROIDS
{
    internal class FloatMenuMakerMap_Patch
    {
        [HarmonyPatch(typeof(FloatMenuMakerMap), nameof(FloatMenuMakerMap.GetOptions))]
        public class GetOptions_Patch
        {
            [HarmonyPrefix]
            public static void ListenerPrefix()
            {
                Utils.insideAddHumanlikeOrders = true;
            }

            [HarmonyPostfix]
            public static void ListenerPostfix()
            {
                Utils.insideAddHumanlikeOrders = false;
            }
        }
    }
}
