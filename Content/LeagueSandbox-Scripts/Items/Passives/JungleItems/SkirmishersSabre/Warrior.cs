﻿using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.Stats;
using LeagueSandbox.GameServer.API;
using GameServerCore.Domain;
//Warrior
namespace ItemPassives
{
    public class ItemID_3714 : IItemScript
    {
        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        public void OnActivate(IObjAiBase owner)
        {
            StatsModifier.ArmorPenetration.FlatBonus += 10;
            StatsModifier.CooldownReduction.FlatBonus = 0.1f;
            owner.AddStatModifier(StatsModifier);
        }

        public void OnDeactivate(IObjAiBase owner)
        {

        }

        public void OnUpdate(float diff)
        {
        }
    }
}
