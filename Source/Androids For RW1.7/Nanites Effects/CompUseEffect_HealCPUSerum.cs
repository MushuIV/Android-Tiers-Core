using System;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class CompUseEffect_HealCPUSerum : CompUseEffect
    {
        public static void Apply(Pawn user)
        {
            int nb = 0;

            bool chance = false;

            if (!Rand.Chance(Settings.percentageNanitesFail))
            {
                nb = user.health.hediffSet.hediffs.RemoveAll((Hediff h) => Utils.AndroidOldAgeHediffCPU.Contains(h.def.defName));
                if (nb > 0)
                {
                    Utils.refreshHediff(user);
                }
                chance = true;
            }

            if (nb == 0)
            {
                if (chance)
                    Messages.Message("ATPP_NoBrokenStuffFound".Translate(user.LabelShort), user, MessageTypeDefOf.NegativeEvent, true);
                else
                    Messages.Message("ATPP_BrokenStuffRepairFailed".Translate(user.LabelShort), user, MessageTypeDefOf.NegativeEvent, true);
            }
            else
            {
                Messages.Message("ATPP_BrokenCPURepaired".Translate(user.LabelShort), user, MessageTypeDefOf.PositiveEvent, true);
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
            else if (!p.haveAndroidOldAgeHediff(Utils.AndroidOldAgeHediffCPU))
            {
                return "ATPP_CannotBeUsedBecauseNoOldAgeIssues".Translate();
            }

            return base.CanBeUsedBy(p);
        }
    }
}