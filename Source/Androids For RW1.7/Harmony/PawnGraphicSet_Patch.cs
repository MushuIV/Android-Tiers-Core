// In RimWorld 1.5+, PawnGraphicSet was removed in favor of PawnRenderTree and PawnRenderNodes.
// HAR 1.5+ handles body types and apparel rendering directly via the new render tree pipeline.
#if RW_LEGACY_GRAPHICS
using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;

namespace MOARANDROIDS
{
    internal class PawnGraphicSet_Patch
    {
        [HarmonyPatch(typeof(PawnGraphicSet), "ResolveApparelGraphics")]
        public class ResolveApparelGraphicsPrefix_Patch
        {
            [HarmonyPrefix]
            public static bool Prefix(Pawn ___pawn)
            {
                if (Utils.ExceptionBodyTypeDefnameAndroidWithSkinMale.Contains(___pawn.story.bodyType.defName))
                {
                    Utils.insideResolveApparelGraphicsLastBodyTypeDef = ___pawn.story.bodyType;
                    ___pawn.story.bodyType = BodyTypeDefOf.Male;
                }
                else if (Utils.ExceptionBodyTypeDefnameAndroidWithSkinFemale.Contains(___pawn.story.bodyType.defName))
                {
                    Utils.insideResolveApparelGraphicsLastBodyTypeDef = ___pawn.story.bodyType;
                    ___pawn.story.bodyType = BodyTypeDefOf.Female;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(PawnGraphicSet), "ResolveApparelGraphics")]
        public class ResolveApparelGraphicsPostfix_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(Pawn ___pawn)
            {
                if(Utils.insideResolveApparelGraphicsLastBodyTypeDef != null)
                {
                    ___pawn.story.bodyType = Utils.insideResolveApparelGraphicsLastBodyTypeDef;
                    Utils.insideResolveApparelGraphicsLastBodyTypeDef = null;
                }
            }
        }

        [HarmonyPatch(typeof(PawnGraphicSet), "ResolveAllGraphics")]
        public class PawnGraphicSetPostfix_Patch
        {
            private static Traverse Pawn_StoryTracker_Traverse = Traverse.Create<Pawn_StoryTracker>();

            [HarmonyPostfix]
            public static void Postfix(PawnGraphicSet __instance)
            {
                if (Utils.RIMMSQOL_LOADED && Utils.ExceptionAndroidWithSkinList.Contains(__instance.pawn.def.defName))
                {
                    Utils.lastResolveAllGraphicsHeadGraphicPath = __instance.pawn.story.HeadGraphicPath;
                    Traverse.Create(__instance.pawn.story).Field("headGraphicPath").SetValue(null);
                }
            }
        }
    }
}
#endif