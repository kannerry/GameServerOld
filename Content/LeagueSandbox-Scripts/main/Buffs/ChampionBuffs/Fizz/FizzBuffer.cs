﻿using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.GameObjects.Stats;
using System.Numerics;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace Buffs
{
    internal class FizzBuffer : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        private IBuff ThisBuff;
        private ISpell Spelll;
        private IAttackableUnit Target;
        private IObjAiBase Owner;
        private float ticks = 0;
        private float damage;

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            ThisBuff = buff;
            Owner = ownerSpell.CastInfo.Owner;
            Spelll = ownerSpell;

            var APratio = Owner.Stats.AbilityPower.Total * 0.2f;
            damage = 24f + (14f * (ownerSpell.CastInfo.SpellLevel - 1)) + APratio;

            buff.SetStatusEffect(StatusFlags.Targetable, false);
            buff.SetStatusEffect(StatusFlags.Ghosted, true);

            Target = unit;

            ApiEventManager.OnSpellPostCast.AddListener(this, ownerSpell.CastInfo.Owner.GetSpell("FizzJump"), EOnSpellPostCast);
        }

        public void EOnSpellPostCast(ISpell spell)
        {
            bool triggeredSpell = true;
            var owner = spell.CastInfo.Owner;
            var oowner = owner as IAttackableUnit;
            var trueCoords = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            var startPos = owner.Position;
            var to = trueCoords - startPos;
            if (to.Length() > 400)
            {
                trueCoords = GetPointFromUnit(owner, 400f);
            }
            //StopAnimation(owner, "Spell3a", false);
            //PauseAnimation(owner, true);
            PlayAnimation(owner, "Spell3d", 0.9f);
            ForceMovement(owner, null, trueCoords, 1200, 0, 0, 0);

            CreateTimer(0.5f, () =>
            {
                AddParticle(owner, null, "Fizz_TrickSlamTwo.troy", trueCoords);
            });

            var buff = owner.GetBuffWithName("FizzTrickSlam");
            buff.DeactivateBuff();
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            buff.SetStatusEffect(StatusFlags.Targetable, true);
            buff.SetStatusEffect(StatusFlags.Ghosted, false);
            ApiEventManager.OnSpellPostCast.RemoveListener(this);
        }

        public void OnUpdate(float diff)
        {
            ticks += diff;
            if (ticks >= 750.0f)
            {
                if (!Owner.HasBuff("FizzTrickSlam"))
                {
                    var trueCoords = new Vector2(Spelll.CastInfo.TargetPosition.X, Spelll.CastInfo.TargetPosition.Z);
                    var startPos = Owner.Position;
                    var to = trueCoords - startPos;
                    if (to.Length() > 200)
                    {
                        trueCoords = GetPointFromUnit(Owner, 200f);
                    }
                    var units = GetUnitsInRange(trueCoords, 200f, true);
                    for (int i = units.Count - 1; i >= 0; i--)
                    {
                        if (units[i].Team != Spelll.CastInfo.Owner.Team && !(units[i] is IObjBuilding || units[i] is IBaseTurret) && units[i] is IObjAiBase ai)
                        {
                            units[i].TakeDamage(Owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELLAOE, false);
                            units.RemoveAt(i);
                        }
                    }
                }
            }
        }
    }
}