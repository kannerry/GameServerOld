﻿using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using GameServerCore.Domain.GameObjects.Spell.Sector;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System;
using System.Numerics;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace Spells
{
    public class YasuoQW : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true,
            NotSingleTargetSpell = true
            // TODO
        };

        ISpell _spell;

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            _spell = spell;
            ApiEventManager.OnSpellHit.AddListener(this, spell, TargetExecute, false);
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            //ner.StopMovement();
        }

        public void OnSpellCast(ISpell spell)
        {
        }

        public void EQ1(IAttackableUnit unit)
        {
            var owner = unit;
            owner.PlayAnimation("Spell1E", 0.5f, 0, 1);
            AddParticleTarget(owner, owner, "Yasuo_Base_EQ_cas.troy", owner);
            var sector = _spell.CreateSpellSector(new SectorParameters
            {
                BindObject = _spell.CastInfo.Owner,
                Length = 215f,
                SingleTick = true,
                CanHitSameTargetConsecutively = true,
                OverrideFlags = SpellDataFlags.AffectEnemies | SpellDataFlags.AffectNeutral | SpellDataFlags.AffectMinions | SpellDataFlags.AffectHeroes,
                Type = SectorType.Area
            });
        }

        public void OnSpellPostCast(ISpell spell)
        {
            var atkspeed = spell.CastInfo.Owner.Stats.AttackSpeedFlat * spell.CastInfo.Owner.Stats.AttackSpeedMultiplier.Total;
            var bonus = atkspeed - spell.CastInfo.Owner.Stats.AttackSpeedFlat;
            var cd = 4 * (1 - bonus);
            float cd1 = (float)Math.Max(cd, 1.33);
            LogDebug(bonus.ToString());
            //var cdr = 
            CreateTimer(0.01f, () => { ((IObjAiBase)spell.CastInfo.Owner).GetSpell(0).SetCooldown(cd1, true); });
            var owner = spell.CastInfo.Owner;

            var spellPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);

            if (owner.HasBuff("YasuoEFIX"))
            {
                ApiEventManager.OnFinishDash.AddListener(this, owner, EQ1, true);
            }
            else
            {
                owner.PlayAnimation("Spell1A", 0.5f, 0, 1);
                CreateTimer(0.15F, () => { AddParticleTarget(owner, owner, "Yasuo_Q_WindStrike.troy", owner); });
                owner.SetStatus(StatusFlags.CanMove, false);
                FaceDirection(spellPos, owner, true);
                owner.StopMovement();
                CreateTimer(0.1f, () => { FaceDirection(spellPos, owner, true); });
                CreateTimer(0.25f, () => { owner.SetStatus(StatusFlags.CanMove, true); });
                CreateTimer(0.25F, () =>
                {
                    var sector = spell.CreateSpellSector(new SectorParameters
                    {
                        BindObject = owner,
                        Length = 450f,
                        Width = 100f,
                        PolygonVertices = new Vector2[]
    {
                    // Basic square, however the values will be scaled by Length/Width, which will make it a rectangle
                    new Vector2(-1, 0),
                    new Vector2(-1, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 0)
    },
                        SingleTick = true,
                        Type = SectorType.Polygon
                    });
                });
            }
        }

        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {
            var owner = spell.CastInfo.Owner;

            var APratio = owner.Stats.AttackDamage.Total;
            var spelllvl = (spell.CastInfo.SpellLevel * 20);
            target.TakeDamage(owner, APratio / 2 + spelllvl / 2 + 1, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
            AddParticleTarget(owner, target, "Yasuo_Base_Q_hit_tar.troy", target);
            AddBuff("YasuoQ01", 10.0f, 1, spell, owner, owner);
            int i = 0;
            if (HasBuff(spell.CastInfo.Owner, "StaticField"))
            {
                var ItemOwner = owner;
                ItemOwner.RemoveBuffsWithName("StaticField");
                AddBuff("StaticFieldCooldown", 7.0f, 1, ItemOwner.GetSpell(0), ItemOwner, ItemOwner);
                var x = GetUnitsInRange(target.Position, 600, true);
                foreach (var unit in x)
                {
                    if (unit.Team != ItemOwner.Team)
                    {
                        if (i < 4)
                        {
                            i++;
                            AddParticle(ItemOwner, unit, "volibear_R_chain_lighting_01.troy", target.Position);
                            unit.TakeDamage(ItemOwner, 100, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELLAOE, false);
                        }
                        CreateTimer(0.01f, () => { i = 0; });
                    }
                }
            }

        }

        public void OnSpellChannel(ISpell spell)
        {
        }

        public void OnSpellChannelCancel(ISpell spell, ChannelingStopSource source)
        {
        }

        public void OnSpellPostChannel(ISpell spell)
        {
        }

        public void OnUpdate(float diff)
        {
        }
    }
}