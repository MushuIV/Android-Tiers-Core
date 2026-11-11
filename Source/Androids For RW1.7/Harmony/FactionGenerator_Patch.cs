using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace MOARANDROIDS
{
    internal class FactionGenerator_Patch
    {
        [HarmonyPatch(typeof(FactionGenerator), nameof(FactionGenerator.GenerateFactionsIntoWorldLayer))]
        public class GenerateFactionsIntoWorldLayer_Patch
        {
            [HarmonyPostfix]
            public static void Listener()
            {
                try
                {
                    Utils.GCATPP?.checkRemoveAndroidFactions();
                }
                catch(Exception e)
                {
                    Log.Message("[ATPP] FactionGenerator.GenerateFactionsIntoWorldLayer : " + e.Message + " - " + e.StackTrace);
                }
            }
        }
    }
}
