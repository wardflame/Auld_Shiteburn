﻿using AuldShiteburn.CombatData.PayloadData;
using AuldShiteburn.EntityData;
using System;
using System.Collections.Generic;

namespace AuldShiteburn.CombatData.AbilityData.Abilities.EnemyAbilities.DungEaterAbilities
{
    [Serializable]
    internal class ClawedSwipeAbility : Ability
    {
        public override string Name => "Clawed Swipe";
        public override string Description => $"swipes {PlayerEntity.Instance.Name} with sharp claws!";
        public override int Cooldown => 0;
        public override int ResourceCost => 0;
        public override PhysicalDamageType PhysicalDamageType => PhysicalDamageType.Slash;
        public override int PhysicalMinDamage => 7;
        public override int PhysicalMaxDamage => 9;

        public override CombatPayload UseAbility(List<EnemyEntity> enemies, EnemyEntity enemy = null)
        {
            Random rand = new Random();
            int physDamage = rand.Next(PhysicalMinDamage, PhysicalMaxDamage + 1);
            return new CombatPayload(
                isAttack: true,
                hasPhysical: true,
                physicalAttackType: PhysicalDamageType,
                physicalDamage: physDamage);
        }
    }
}
