using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;
using System;

namespace MOARANDROIDS
{
        public class StockGenerator_SlaveAndroids : StockGenerator
        {
            public override IEnumerable<Thing> GenerateThings(PlanetTile forTile, Faction faction = null)
            {
                if (this.respectPopulationIntent && Rand.Value > StorytellerUtilityPopulation.PopulationIntent)
                {
                    yield break;
                }
                int count = this.countRange.RandomInRange;
                for (int i = 0; i < count; i++)
                {
                    Faction androidFaction;
                    if (!(from fac in Find.FactionManager.AllFactionsVisible
                          where fac != Faction.OfPlayer && fac.def.humanlikeFaction
                          select fac).TryRandomElement(out androidFaction))
                    {
                        yield break;
                    }

                    Random rnd = new Random();
                    PawnKindDef android;
                    if (randomisePawnKind == true)
                    {
                        switch (rnd.Next(1, 4))
                        {
                            case 1:
                                 android = PawnKindDefOf.AndroidT1ColonistGeneral;
                                 break;
                            case 2:
                                 android = PawnKindDefOf.AndroidT2ColonistGeneral;
                                 break;
                            case 3:
                                android = PawnKindDefOf.AndroidT3ColonistGeneral;
                                break;
                            case 4:
                                android = PawnKindDefOf.AndroidT4ColonistGeneral;
                                break;
                            default:
                                android = PawnKindDefOf.AndroidT1ColonistGeneral;
                                break;
                        }
                    } else
                    {
                        android = pawnKind;
                    }
                    
                    Faction fac1 = androidFaction;
                PawnGenerationRequest request = new PawnGenerationRequest(kind: android, faction: fac1, context: PawnGenerationContext.NonPlayer, tile: forTile, forceGenerateNewPawn: false, canGeneratePawnRelations: false, mustBeCapableOfViolence: false, colonistRelationChanceFactor: 1f, forceAddFreeWarmLayerIfNeeded: !this.trader.orbital);
                    yield return PawnGenerator.GeneratePawn(request);
                }
                yield break;
            }

            public override bool HandlesThingDef(ThingDef thingDef)
            {
                return thingDef.category == ThingCategory.Pawn && thingDef.race.Humanlike && thingDef.tradeability != Tradeability.None;
            }

            private bool respectPopulationIntent = false;
            private PawnKindDef pawnKind = null;
            private bool randomisePawnKind = false;
    }
}