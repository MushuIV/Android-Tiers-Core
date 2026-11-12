using System;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    [DefOf]
    public static class FleshTypeDefOfAT
    {
        static FleshTypeDefOfAT()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(FleshTypeDefOfAT));
        }

        public static FleshTypeDef AndroidTier;
    }
}
