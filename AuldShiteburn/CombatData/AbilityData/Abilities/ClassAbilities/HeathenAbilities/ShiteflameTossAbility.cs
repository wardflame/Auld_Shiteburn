﻿using AuldShiteburn.CombatData.PayloadData;
using AuldShiteburn.EntityData;
using System;

namespace AuldShiteburn.CombatData.AbilityData.Abilities.ClassAbilities.HeathenAbilities
{
    [Serializable]
    internal class ShiteflameTossAbility : Ability
    {
        public override string Name => "Shiteflame Toss";
        public override string Description => $"Hurls a pungent ball of burning shite at the target for {PhysicalMinDamage} to {PhysicalMaxDamage} Occult damage.";
        public override int Cooldown => 4;
        public override int ResourceCost => 6;
        public override int PhysicalMinDamage => 4;
        public override int PhysicalMaxDamage => 8;

        public override CombatPayload UseAbility()
        {
            Utils.SetCursorInteract(Console.CursorTop - 1);
            if (ActiveCooldown > 0)
            {
                ActiveCooldown--;
                Utils.WriteColour($"{Name} is on cooldown {ActiveCooldown}/{Cooldown}.", ConsoleColor.DarkRed);
                return new CombatPayload(false);
            }
            else if (!PlayerEntity.Instance.CheckResourceLevel(ResourceCost))
            {
                Utils.WriteColour($"You lack the resources to use this ability.", ConsoleColor.DarkRed);
                return new CombatPayload(false);
            }
            else if (ActiveCooldown <= 0)
            {
                Random rand = new Random();
                int damage = rand.Next(PhysicalMinDamage, PhysicalMaxDamage + 1);
                ActiveCooldown = Cooldown;
                if (PlayerEntity.Instance.UsesStamina)
                {
                    PlayerEntity.Instance.Stamina -= ResourceCost;
                }
                else if (PlayerEntity.Instance.UsesMana)
                {
                    PlayerEntity.Instance.Mana -= ResourceCost;
                }
                return new CombatPayload(true, isStun: true, hasProperty: true, propertyAttackType: PropertyDamageType.Occult, stunCount: 3, propertyDamage: damage);
            }
            return new CombatPayload(false);
        }
    }
}
