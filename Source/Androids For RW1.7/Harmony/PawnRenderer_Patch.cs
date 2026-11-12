using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace MOARANDROIDS
{
    internal class ATPP_PawnRenderer_Patch
    {
        [HarmonyPatch(typeof(PawnRenderer), nameof(PawnRenderer.LayingFacing))]
        public class LayingFacing_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn ___pawn, ref Rot4 __result)
            {
                if (___pawn != null && (___pawn.def.defName == Utils.M7 || ___pawn.def.defName == Utils.M8))
                    __result = Rot4.South;
            }
        }

        [HarmonyPatch(typeof(PawnRenderer), "RenderPawnInternal")]
        public class ATPP_RenderPawnInternal_Patch
        {
            [HarmonyPostfix]
            public static void Listener(PawnRenderer __instance, PawnDrawParms parms)
            {
                try
                {
                    Pawn pawn = parms.pawn;
                    if (pawn == null || parms.dead || parms.flags.FlagSet(PawnRenderFlags.HeadStump))
                        return;

                    bool flagPortrait = parms.Portrait;
                    bool state = false;
                    CompAndroidState cas = Utils.getCachedCAS(pawn);

                    if (Utils.ExceptionAndroidWithGlowingEyes.Contains(pawn.def.defName)
                        || (Utils.ExceptionAndroidWithSkinList.Contains(pawn.def.defName) && cas != null && (cas.TXHurtedHeadSet || cas.TXHurtedHeadSet2)))
                    {
                        if (!flagPortrait)
                        {
                            Pawn_JobTracker jobs = pawn.jobs;
                            if (jobs != null && jobs.curDriver != null)
                            {
                                state = !jobs.curDriver.asleep;
                            }
                        }
                        else
                        {
                            state = flagPortrait;
                        }
                    }

                    if (state && cas != null)
                    {
                        Rot4 headFacing = parms.facing;
                        Rot4 bodyFacing = parms.facing;
                        Vector3 rootLoc = parms.matrix.GetColumn(3);
                        Quaternion quaternion = parms.matrix.rotation;

                        Vector3 a = rootLoc;
                        if (bodyFacing != Rot4.North)
                        {
                            a.y += 0.0281250011f;
                            rootLoc.y += 0.0234375f;
                        }
                        else
                        {
                            a.y += 0.0234375f;
                            rootLoc.y += 0.0281250011f;
                        }
                        Vector3 b = quaternion * __instance.BaseHeadOffsetAt(headFacing);
                        Vector3 loc = a + b + new Vector3(0f, 0.01f, 0f);
                        if (headFacing != Rot4.North)
                        {
                            Mesh mesh = HumanlikeMeshPoolUtility.GetHumanlikeHeadSetForPawn(pawn).MeshAt(headFacing);
                            bool isHorizontal = headFacing.IsHorizontal;
                            int type = 1;
                            if (cas.TXHurtedHeadSet)
                                type = 2;
                            else if(cas.TXHurtedHeadSet2 || pawn.def.defName == Utils.TX2I || pawn.def.defName == Utils.TX2KI || pawn.def.defName == Utils.TX3I || pawn.def.defName == Utils.TX4I)
                                type = 3;

                            //Couleur yeux standard TX2
                            Color color = new Color(0.9450f, 0.76862f, 0.05882f);

                            //Couleur yeux TX3/TX4 standard (bleu cyan)
                            if(pawn.def.defName == Utils.TX3 || pawn.def.defName == Utils.TX4 || pawn.def.defName == Utils.TX3I || pawn.def.defName == Utils.TX4I)
                            {
                                color = new Color(0f, 0.972549f, 0.972549f);
                            }

                            // yeux rouges si drafté OU ennemis OU TX2K
                            if (pawn.Drafted || (pawn.Faction != null && pawn.Faction.HostileTo(Faction.OfPlayer)) || pawn.def.defName == Utils.TX2K || pawn.def.defName == Utils.TX2KI)
                                color = new Color(0.75f, 0f, 0f, 1f);

                            string gender = "M";
                            if (pawn.gender == Gender.Female)
                                gender = "F";

                            if (isHorizontal)
                            {
                                GenDraw.DrawMeshNowOrLater(mesh, loc, quaternion, Tex.getEyeGlowEffect(color, gender, type, 0).MatSingle, parms.DrawNow);
                            }
                            else
                            {
                                GenDraw.DrawMeshNowOrLater(mesh, loc, quaternion, Tex.getEyeGlowEffect(color, gender, type, 1).MatSingle, parms.DrawNow);
                            }
                        }
                    }
                }
                catch(Exception e)
                {
                    Log.Message("[ATPP] PawnRenderer.RenderPawnInternal " + e.Message + " " + e.StackTrace);
                }
            }
        }
    }
}
