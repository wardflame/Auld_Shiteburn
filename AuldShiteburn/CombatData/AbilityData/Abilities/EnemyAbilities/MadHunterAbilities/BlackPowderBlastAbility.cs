﻿using AuldShiteburn.CombatData.PayloadData;
using AuldShiteburn.EntityData;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuldShiteburn.CombatData.AbilityData.Abilities.EnemyAbilities.MadHunterAbilities
{
    internal class BlackPowderBlastAbility : Ability
    {
        public override string Name => "Black Powder Blast";
        public override string Description => $"launches flame from their gunaxe!";
        public override int Cooldown => 3;
        public override int ResourceCost => 0;
        public override PropertyDamageType PropertyDamageType => PropertyDamageType.Fire;
        public override int PropertyMinDamage => 4;
        public override int PropertyMaxDamage => 6;

        public override CombatPayload UseAbility(List<EnemyEntity> enemies, EnemyEntity enemy = null)
        {
            Utils.SetCursorInteract(Console.CursorTop + 1);
            if (ActiveCooldown <= 0)
            {
                Random rand = new Random();
                int physDamage = rand.Next(PropertyMinDamage, PropertyMaxDamage + 1);
                int propDamage = rand.Next(PropertyMinDamage, PropertyMaxDamage + 1);
                ActiveCooldown = Cooldown;
                return new CombatPayload(
                    isAttack: true,
                    hasPhysical: true, hasProperty: true,
                    physicalAttackType: PhysicalDamageType, propertyAttackType: PropertyDamageType,
                    physicalDamage: physDamage, propertyDamage: propDamage);
            }
            return new CombatPayload(false);
        }
    }
}
