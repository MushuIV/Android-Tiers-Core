using System;
using System.Collections.Generic;
using AlienRace;
using UnityEngine;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class Recipe_SurgeryAndroids : RecipeWorker
    {
        protected bool CheckSurgeryFailAndroid(Pawn surgeon, Pawn patient, List<Thing> ingredients, BodyPartRecord part, Bill bill)
        {
            float num = 1f;
            if (!patient.RaceProps.IsMechanoid)
            {
                num *= surgeon.GetStatValue(StatDefOf.AndroidSurgerySuccessChance, true);
            }
            if (patient.InBed())
            {
                num *= patient.CurrentBed().GetStatValue(StatDefOf.AndroidSurgerySuccessChance, true);
            }
            num *= this.recipe.surgerySuccessChanceFactor;
            if (surgeon.InspirationDef != null && surgeon.InspirationDef.defName == "Inspired_Surgery" && !patient.RaceProps.IsMechanoid)
            {
                if (num < 1f)
                {
                    num = 1f - (1f - num) * 0.1f;
                }
                surgeon.mindState.inspirationHandler.EndInspiration(surgeon.InspirationDef);
            }
            if (!Rand.Chance(num))
            {
                if (Rand.Chance(this.recipe.deathOnFailedSurgeryChance))
                {
                    HealthUtility.GiveRandomSurgeryInjuries(patient, 65, part);
                    if (!patient.Dead)
                    {
                        patient.Kill(null, null);
                    }
                    Messages.Message("MessageMedicalOperationFailureFatalAndroid".Translate(surgeon.LabelShort, patient.LabelShort, this.recipe.label), patient, MessageTypeDefOf.NegativeHealthEvent);
                }
                else if (Rand.Chance(0.5f))
                {
                    if (Rand.Chance(0.1f))
                    {
                        Messages.Message("MessageMedicalOperationFailureRidiculousAndroid".Translate(surgeon.LabelShort, patient.LabelShort), patient, MessageTypeDefOf.NegativeHealthEvent);
                        HealthUtility.GiveRandomSurgeryInjuries(patient, 20, null);
                    }
                    else
                    {
                        Messages.Message("MessageMedicalOperationFailureCatastrophicAndroid".Translate(surgeon.LabelShort, patient.LabelShort), patient, MessageTypeDefOf.NegativeHealthEvent);
                        HealthUtility.GiveRandomSurgeryInjuries(patient, 30, part);
                    }
                }
                else
                {
                    Messages.Message("MessageMedicalOperationFailureMinorAndroid".Translate(surgeon.LabelShort, patient.LabelShort), patient, MessageTypeDefOf.NegativeHealthEvent);
                    HealthUtility.GiveRandomSurgeryInjuries(patient, 15, part);
                }
                if (!patient.Dead)
                {
                    this.TryGainBotchedSurgeryThought(patient, surgeon);
                }
                return true;
            }
            return false;
        }

        // Token: 0x06001266 RID: 4710 RVA: 0x0008C4AD File Offset: 0x0008A8AD
        private void TryGainBotchedSurgeryThought(Pawn patient, Pawn surgeon)
        {
            if (!patient.RaceProps.Humanlike)
            {
                return;
            }
            patient.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.BotchedMySurgery, surgeon);
        }

        protected void applyFrameworkColor(Pawn pawn)
        {
            CompAndroidState cas = Utils.getCachedCAS(pawn);
            if (cas == null)
                return;

            //Renouvellement délais de rouille
            cas.clearRusted();
        }

        private static readonly (string UnskinnedKind, string SkinnedKind)[] SyntheticSkinKindPairs =
        {
            ("ATPP_Android2ITXKind", "ATPP_Android2TXKind"),
            ("ATPP_Android2KITXKind", "ATPP_Android2KTXKind"),
            ("ATPP_Android3ITXKind", "ATPP_Android3TXKind"),
            ("ATPP_Android4ITXKind", "ATPP_Android4TXKind"),
            ("ATPP_AndroidTX2IRaiderFactionSpecific", "ATPP_AndroidTX2RaiderFactionSpecific"),
            ("ATPP_AndroidTX2KIRaiderFactionSpecific", "ATPP_AndroidTX2KRaiderFactionSpecific"),
            ("ATPP_AndroidTX3IRaiderFactionSpecific", "ATPP_AndroidTX3RaiderFactionSpecific"),
            ("ATPP_AndroidTX4IRaiderFactionSpecific", "ATPP_AndroidTX4RaiderFactionSpecific"),
            ("ATPP_AndroidTX2ICollectiveSoldier", "ATPP_AndroidTX2CollectiveSoldier"),
            ("ATPP_AndroidTX2KICollectiveSoldier", "ATPP_AndroidTX2KCollectiveSoldier"),
            ("ATPP_AndroidTX3ICollectiveSoldier", "ATPP_AndroidTX3CollectiveSoldier"),
            ("ATPP_AndroidTX4ICollectiveSoldier", "ATPP_AndroidTX4CollectiveSoldier")
        };

        private static string resolveTargetKindDefName(string currentKindDefName, bool applyingSkin)
        {
            if (string.IsNullOrEmpty(currentKindDefName))
                return null;

            foreach (var pair in SyntheticSkinKindPairs)
            {
                if (applyingSkin && pair.UnskinnedKind == currentKindDefName)
                    return pair.SkinnedKind;
                if (!applyingSkin && pair.SkinnedKind == currentKindDefName)
                    return pair.UnskinnedKind;
            }
            return null;
        }

        protected void syncPawnAfterSyntheticSkinChange(Pawn pawn, ThingDef targetDef, string fallbackKindDefName, bool applyingSkin)
        {
            pawn.def = targetDef;

            string mappedKindDefName = resolveTargetKindDefName(pawn.kindDef?.defName, applyingSkin);
            if (mappedKindDefName != null)
            {
                PawnKindDef mappedKind = DefDatabase<PawnKindDef>.GetNamed(mappedKindDefName, false);
                if (mappedKind != null)
                    pawn.kindDef = mappedKind;
            }
            else if (fallbackKindDefName != null && (pawn.kindDef == null || pawn.kindDef.race != targetDef))
            {
                PawnKindDef fallbackKind = DefDatabase<PawnKindDef>.GetNamed(fallbackKindDefName, false);
                if (fallbackKind != null)
                    pawn.kindDef = fallbackKind;
            }

            if (applyingSkin)
            {
                applyFrameworkColor(pawn);
            }

            if (pawn.story != null)
            {
                Utils.changeTXBodyType(pawn, 0);
                Utils.changeHARCrownType(pawn, "Average_Normal");
                pawn.story.skinColorOverride = null;
                pawn.story.SkinColorBase = Color.clear;

                if (!applyingSkin || pawn.story.hairDef == null)
                {
                    pawn.story.hairDef = HairDefOf.Bald;
                }
            }

            if (!applyingSkin && pawn.style != null)
            {
                pawn.style.beardDef = BeardDefOf.NoBeard;
                pawn.style.FaceTattoo = TattooDefOf.NoTattoo_Face;
                pawn.style.BodyTattoo = TattooDefOf.NoTattoo_Body;
            }

            AlienPartGenerator.AlienComp alienComp = pawn.TryGetComp<AlienPartGenerator.AlienComp>();
            if (alienComp != null)
            {
                alienComp.ColorChannels = null;
                _ = alienComp.ColorChannels;
                alienComp.UpdateColors();
                alienComp.RegenerateColorChannelLinks();
            }

            CompAndroidState cas = Utils.getCachedCAS(pawn);
            cas?.refreshDefState();

            if (pawn.health?.hediffSet?.hediffs != null)
            {
                foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
                {
                    if (hediff is Hediff_SolarFlare solarFlare)
                        solarFlare.resetCache();
                }
            }

            List<StatDef> allStats = DefDatabase<StatDef>.AllDefsListForReading;
            for (int i = 0; i < allStats.Count; i++)
            {
                allStats[i].Worker?.ClearCacheForThing(pawn);
            }

            pawn.Drawer?.renderer?.SetAllGraphicsDirty();
            PortraitsCache.SetDirty(pawn);
            GlobalTextureAtlasManager.TryMarkPawnFrameSetDirty(pawn);
        }


        // Token: 0x04000B58 RID: 2904
        private const float CatastrophicFailChance = 0.5f;

        // Token: 0x04000B59 RID: 2905
        private const float RidiculousFailChanceFromCatastrophic = 0.1f;

        // Token: 0x04000B5A RID: 2906
        private const float InspiredSurgeryFailChanceFactor = 0.1f;

        // Token: 0x04000B5B RID: 2907
        private static readonly SimpleCurve MedicineMedicalPotencyToSurgeryChanceFactor = new SimpleCurve
        {
            {
                new CurvePoint(0f, 0.7f),
                true
            },
            {
                new CurvePoint(1f, 1f),
                true
            },
            {
                new CurvePoint(2f, 1.3f),
                true
            }
        };
    }
}
