﻿using AuldShiteburn.CombatData;
using AuldShiteburn.CombatData.AbilityData;
using AuldShiteburn.CombatData.AbilityData.Abilities.EnemyAbilities.MadHunterAbilities;
using AuldShiteburn.ItemData;
using System;
using System.Collections.Generic;

namespace AuldShiteburn.EntityData.Enemies
{
    [Serializable]
    internal class MadHunterEnemyEntity : EnemyEntity
    {
        public MadHunterEnemyEntity()
        {
            Random rand = new Random();
            int health = rand.Next(17, 21);
            Name = "Mad Hunter";
            MaxHP = health;
            HP = health;
            PhysicalWeakness = PhysicalDamageType.Slash;
            MaterialWeakness = GeneralMaterials.Moonstone;
            PropertyWeakness = PropertyDamageType.Fire;
            StunCap = 6;
            Abilities = new List<Ability>()
            {
                new MadSlashAbility()
            };
        }
    }
}
