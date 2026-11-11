using System;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    [DefOf]
    public static class GameConditionDefOfAT
    {
        public static GameConditionDef SolarFlare;

        static GameConditionDefOfAT()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(GameConditionDefOfAT));
        }
    }
}
