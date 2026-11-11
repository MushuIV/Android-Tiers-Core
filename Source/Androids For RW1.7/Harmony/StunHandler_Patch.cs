using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System;

namespace MOARANDROIDS
{
    internal class StunHandler_Patch
    {
        [HarmonyPatch(typeof(StunHandler), "CanBeStunnedByDamage")]
        public class CanBeStunnedByDamage_Patch
        {
            [HarmonyPostfix]
            public static void Listener(DamageDef def, ref bool __result, Thing ___parent)
            {
                if (!__result && def == DamageDefOf.EMP && ___parent is Pawn pawn)
                {
                    if (Utils.ExceptionAndroidWithoutSkinList.Contains(pawn.def.defName) || pawn.IsCyberAnimal())
                    {
                        __result = true;
                    }
                }
            }
        }
    }
}
