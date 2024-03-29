﻿using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.Stats;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace Buffs
{
    internal class KarthusSlow : IBuffGameScript
    {
        public BuffType BuffType => BuffType.SLOW;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            var spellLevel = ownerSpell.CastInfo.Owner.GetSpell("KarthusWallOfPain").CastInfo.SpellLevel * 0.12;
            StatsModifier.MoveSpeed.PercentBonus = (float)(StatsModifier.MoveSpeed.PercentBonus - 0.40f - spellLevel);
            unit.AddStatModifier(StatsModifier);
            AddParticleTarget(ownerSpell.CastInfo.Owner, unit, "Ashe_Base_W_tar.troy", unit, 1f);
            // ApplyAssistMarker
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
        }

        public void OnUpdate(float diff)
        {
        }
    }
}