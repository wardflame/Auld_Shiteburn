﻿using AuldShiteburn.CombatData;
using AuldShiteburn.CombatData.AbilityData;
using AuldShiteburn.CombatData.AbilityData.Abilities.ClassAbilities.HeathenAbilities;
using AuldShiteburn.ItemData;
using AuldShiteburn.ItemData.ArmourData;
using AuldShiteburn.ItemData.WeaponData;
using System;
using System.Collections.Generic;

namespace AuldShiteburn.EntityData.PlayerData.Classes
{
    [Serializable]
    internal class HeathenClass : CharacterClass
    {
        public HeathenClass() : base
            ("Heathen", ClassType.Heathen,
            new TitleData(PlayerGenerationData.HeathenTitles),
            new ClassStatistics(16, 0, 30, 2),
            new ProficiencyData(ArmourFamily.Light, WeaponFamily.PrimitiveArms, PropertyDamageType.Occult, GeneralMaterials.Hardshite),
            new List<Ability>()
            {
                new HardshiteShardAbility(),
                new ShiteWardAbility(),
                new FaecalNourishmentAbility()
            }
            )
        { }
    }
}
