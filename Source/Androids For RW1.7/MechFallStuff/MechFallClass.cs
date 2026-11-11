using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class MechFall : OrbitalStrike
    {
        public override void StartStrike()
        {
            base.StartStrike();
            MechFallMoteMaker.MakeMechFallMote(base.Position, base.Map);
        }

        protected override void Tick()
        {
            if (base.TicksPassed >= this.duration)
            {
                this.SpawnDude();
            }
            if (base.TicksPassed == this.duration-5)
            {
                this.CreateExplosion();
            }
            if (base.TicksPassed == this.duration - 15)
            {
                this.CreateExplosion();
            }
            if (base.TicksPassed == this.duration - 160)
            {
                this.CreatePod();
            }
            base.Tick();
            if (base.Destroyed)
            {
                return;
            }
        }

        private void CreateExplosion()
        {
            DamageDef smoke = DamageDefOf.Smoke;
            Thing instigator = this.instigator;
            ThingDef def = this.def;
            ThingDef weaponDef = this.weaponDef;
            GenExplosion.DoExplosion(base.Position, base.Map, 1f, DamageDefOf.Bomb, instigator, weapon: weaponDef, projectile: def);
            GenExplosion.DoExplosion(base.Position, base.Map, 1f, DamageDefOf.Bomb, instigator, weapon: weaponDef, projectile: def);
        }

        private void CreatePod()
        {
            ActiveTransporterInfo info = new ActiveTransporterInfo
            {
                openDelay = 10,
                leaveSlag = true
            };
            DropPodUtility.MakeDropPodAt(base.Position, base.Map, info);
        }

        public void SpawnDude()
        {
            PawnGenerationRequest request = new PawnGenerationRequest(kind: PawnKindDefOf.M7MechPawn, faction: Faction.OfPlayer, context: PawnGenerationContext.NonPlayer);
            Pawn pawn = PawnGenerator.GeneratePawn(request);
            FilthMaker.TryMakeFilth(base.Position, base.Map, RimWorld.ThingDefOf.Filth_RubbleBuilding, 30);

            GenSpawn.Spawn(pawn, base.Position, base.Map);
        }
    }
}
