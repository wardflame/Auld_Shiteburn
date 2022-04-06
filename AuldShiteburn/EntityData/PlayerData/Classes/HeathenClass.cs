﻿using AuldShiteburn.CombatData;
using AuldShiteburn.CombatData.AbilityData;
using AuldShiteburn.CombatData.AbilityData.Abilities.ClassAbilities.HeathenAbilities;
using AuldShiteburn.ItemData;
using AuldShiteburn.ItemData.ArmourData;
using AuldShiteburn.ItemData.WeaponData;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuldShiteburn.EntityData.PlayerData.Classes
{
    [Serializable]
    internal class HeathenClass : CharacterClass
    {
        public HeathenClass() : base
            ("Heathen", ClassType.Heathen,
            new TitleData(PlayerGenerationData.HeathenTitles),
            new ClassStatistics(12, 0, 30, 3),
            new ProficiencyData(ArmourFamily.LightArmour, WeaponFamily.PrimitiveArms, PropertyDamageType.Occult, GeneralMaterials.Hardshite),
            new List<Ability>()
            {
                new HardshiteBoltAbility(),
                new ShiteWardAbility(),
                new FaecalNourishmentAbility()
            }
            )
        { }
    }
}
