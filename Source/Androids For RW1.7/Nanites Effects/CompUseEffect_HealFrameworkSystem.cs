using System;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class CompUseEffect_HealFrameworkSystem : CompUseEffect
    {
        public static void Apply(Pawn user)
        {
            CompAndroidState cas = Utils.getCachedCAS(user);
            if (cas != null)
            {
                int CGT = Find.TickManager.TicksGame;
                cas.frameworkNaniteEffectGTStart = CGT;
                cas.frameworkNaniteEffectGTEnd = CGT + (Rand.Range(Settings.minHoursNaniteFramework, Settings.maxHoursNaniteFramework) * 2500);
            }
        }

        public override void DoEffect(Pawn user)
        {
            base.DoEffect(user);

            Apply(user);
            
        }

        public override AcceptanceReport CanBeUsedBy(Pawn p)
        {
            if ( !p.IsAndroidTier())
            {
                return "ATPP_CanOnlyBeUsedByAndroid".Translate();
            }
            CompAndroidState cas = Utils.getCachedCAS(p);
            if (cas != null && cas.frameworkNaniteEffectGTEnd != -1)
            {
                return false;
            }

            return base.CanBeUsedBy(p);
        }
    }
}