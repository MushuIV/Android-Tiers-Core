using System;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public static class DownedT5Utility
    {
        public static Pawn GenerateT5(int tile)
        {
            PawnKindDef AndroidT5Colonist = PawnKindDefOf.AndroidT5Colonist;
            Faction ofplayer = Faction.OfAncients;
            PawnGenerationRequest request = new PawnGenerationRequest(kind: AndroidT5Colonist, faction: ofplayer, context: PawnGenerationContext.NonPlayer, tile: tile, forceGenerateNewPawn: false, canGeneratePawnRelations: false, mustBeCapableOfViolence: false, colonistRelationChanceFactor: 20f);
            Pawn pawn = PawnGenerator.GeneratePawn(request);
            HealthUtility.DamageUntilDowned(pawn);
            Hediff hediff = HediffMaker.MakeHediff(MOARANDROIDS.HediffDefOf.RebootingSequenceAT, pawn, null);
            hediff.Severity = 1f;
            pawn.health.AddHediff(hediff);
            return pawn;
        }
        private const float RelationWithColonistWeight = 0.8f;

        private const float ChanceToRedressWorldPawn = 0f;
    }
}