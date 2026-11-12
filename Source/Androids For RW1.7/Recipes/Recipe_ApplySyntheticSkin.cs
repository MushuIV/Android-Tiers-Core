using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MOARANDROIDS
{
    public class Recipe_ApplySyntheticSkin : Recipe_SurgeryAndroids
    {
        public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
        {
            if (!(thing is Pawn pawn) || pawn.Dead)
            {
                return false;
            }

            string defName = pawn.def.defName;
            return defName == "ATPP_Android2ITX" || defName == "ATPP_Android2KITX" || defName == "ATPP_Android3ITX" || defName == "ATPP_Android4ITX";
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
                case "ATPP_Android2ITX":
                    targetDefName = "ATPP_Android2TX";
                    targetKindDefName = "ATPP_Android2TXKind";
                    break;
                case "ATPP_Android2KITX":
                    targetDefName = "ATPP_Android2KTX";
                    targetKindDefName = "ATPP_Android2KTXKind";
                    break;
                case "ATPP_Android3ITX":
                    targetDefName = "ATPP_Android3TX";
                    targetKindDefName = "ATPP_Android3TXKind";
                    break;
                case "ATPP_Android4ITX":
                    targetDefName = "ATPP_Android4TX";
                    targetKindDefName = "ATPP_Android4TXKind";
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

                    base.applyFrameworkColor(pawn);

                    pawn.Drawer?.renderer?.SetAllGraphicsDirty();
                    PortraitsCache.SetDirty(pawn);
                    GlobalTextureAtlasManager.TryMarkPawnFrameSetDirty(pawn);

                    Messages.Message("ATPP_AppliedSyntheticSkinSuccess".Translate(pawn.LabelShortCap), pawn, MessageTypeDefOf.PositiveEvent);
                }
            }
        }
    }
}
