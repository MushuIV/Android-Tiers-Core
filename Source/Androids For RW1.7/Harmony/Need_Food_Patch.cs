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
    internal class Need_Food_Patch
    {
        /*
         * Mise en place d'un Maxlevel raisonable car avec l'algo de base de RW il st demeusuré de part la taille des M7 et donc la batterie (food) met BC de temps a s'écouler
         */
        [HarmonyPatch(typeof(Need_Food), nameof(Need_Food.FoodFallPerTickAssumingCategory))]
        public class FoodFall_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn ___pawn, ref float __result)
            {
                if (___pawn != null && ___pawn.def == Utils.M7Mech && ___pawn.IsSurrogateAndroid())
                {
                    __result = 1.5f * 2.6666667E-05f;
                }
            }
        }
    }
}
