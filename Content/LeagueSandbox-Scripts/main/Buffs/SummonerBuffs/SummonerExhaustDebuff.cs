﻿using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.Stats;

namespace Buffs
{
    internal class SummonerExhaustDebuff : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_DEHANCER;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            StatsModifier.MoveSpeed.PercentBonus = -0.3f;
            StatsModifier.AttackSpeed.PercentBonus = -0.3f;
            StatsModifier.Armor.BaseBonus = -10;
            StatsModifier.MagicResist.BaseBonus = -10;
            unit.AddStatModifier(StatsModifier);
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
        }

        public void OnUpdate(float diff)
        {
        }
    }
}