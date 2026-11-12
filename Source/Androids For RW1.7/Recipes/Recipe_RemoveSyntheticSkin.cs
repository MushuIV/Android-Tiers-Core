using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MOARANDROIDS
{
    public class Recipe_RemoveSyntheticSkin : Recipe_SurgeryAndroids
    {
        public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
        {
            if (!(thing is Pawn pawn) || pawn.Dead)
            {
                return false;
            }

            string defName = pawn.def.defName;
            return defName == "ATPP_Android2TX" || defName == "ATPP_Android2KTX" || defName == "ATPP_Android3TX" || defName == "ATPP_Android4TX";
        }

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (billDoer != null)
            {
                if (base.CheckSurgeryFailAndroid(billDoer, pawn, ingredients, part, bill))
                {
                    return;
                }
                TaleRecorder.RecordTale(TaleDefOf.DidSurgery, new object[] { billDoer, pawn });
            }

            string targetDefName = null;
            string targetKindDefName = null;

            switch (pawn.def.defName)
            {
                case "ATPP_Android2TX":
                    targetDefName = "ATPP_Android2ITX";
                    targetKindDefName = "ATPP_Android2ITXKind";
                    break;
                case "ATPP_Android2KTX":
                    targetDefName = "ATPP_Android2KITX";
                    targetKindDefName = "ATPP_Android2KITXKind";
                    break;
                case "ATPP_Android3TX":
                    targetDefName = "ATPP_Android3ITX";
                    targetKindDefName = "ATPP_Android3ITXKind";
                    break;
                case "ATPP_Android4TX":
                    targetDefName = "ATPP_Android4ITX";
                    targetKindDefName = "ATPP_Android4ITXKind";
                    break;
            }

            if (targetDefName != null)
            {
                ThingDef targetDef = DefDatabase<ThingDef>.GetNamed(targetDefName, false);
                if (targetDef != null)
                {
                    pawn.def = targetDef;

                    if (targetKindDefName != null && pawn.kindDef != null && pawn.kindDef.defName.Contains("TX"))
                    {
                        PawnKindDef pk = DefDatabase<PawnKindDef>.GetNamed(targetKindDefName, false);
                        if (pk != null)
                        {
                            pawn.kindDef = pk;
                        }
                    }

                    // Recover synthetic skin gland if pawn is on map
                    if (pawn.Map != null)
                    {
                        ThingDef glandDef = DefDatabase<ThingDef>.GetNamed("ATPP_SyntheticSkinGland", false);
                        if (glandDef != null)
                        {
                            Thing gland = ThingMaker.MakeThing(glandDef);
                            GenPlace.TryPlaceThing(gland, pawn.Position, pawn.Map, ThingPlaceMode.Near);
                        }
                    }

                    pawn.Drawer?.renderer?.SetAllGraphicsDirty();
                    PortraitsCache.SetDirty(pawn);
                    GlobalTextureAtlasManager.TryMarkPawnFrameSetDirty(pawn);

                    Messages.Message("ATPP_RemovedSyntheticSkinSuccess".Translate(pawn.LabelShortCap), pawn, MessageTypeDefOf.NeutralEvent);
                }
            }
        }
    }
}
