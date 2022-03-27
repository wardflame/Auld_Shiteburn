﻿using AuldShiteburn.CombatData;
using AuldShiteburn.EntityData;
using System;
using System.Collections.Generic;

namespace AuldShiteburn.ItemData.WeaponData
{
    [Serializable]
    class WeaponType
    {
        #region DextrousWeapons
        public static WeaponType Dagger
        {
            get { return new WeaponType("Dagger", WeaponFamily.DextrousSmallArms, PhysicalDamageType.Pierce, 3, 5); }
        }
        public static WeaponType Rapier
        {
            get { return new WeaponType("Rapier", WeaponFamily.DextrousSmallArms, PhysicalDamageType.Pierce, 3, 5); }
        }
        public static WeaponType Shortsword
        {
            get { return new WeaponType("Shortsword", WeaponFamily.DextrousSmallArms, PhysicalDamageType.Slash, 3, 5); }
        }
        #endregion DextrousWeapons
        #region PrimitiveWeapons
        public static WeaponType HandAxe
        {
            get { return new WeaponType("Hand Axe", WeaponFamily.PrimitiveArms, PhysicalDamageType.Slash, 4, 6); }
        }
        public static WeaponType Cudgel
        {
            get { return new WeaponType("Cudgel", WeaponFamily.PrimitiveArms, PhysicalDamageType.Strike, 4, 6); }
        }
        public static WeaponType Spear
        {
            get { return new WeaponType("Spear", WeaponFamily.PrimitiveArms, PhysicalDamageType.Pierce, 4, 6); }
        }
        #endregion PrimitiveWeapons
        #region MartialWeapons
        public static WeaponType Longsword
        {
            get { return new WeaponType("Longsword", WeaponFamily.MartialArms, PhysicalDamageType.Slash, 5, 7); }
        }
        public static WeaponType BattleAxe
        {
            get { return new WeaponType("Battle Axe", WeaponFamily.MartialArms, PhysicalDamageType.Slash, 5, 7); }
        }
        public static WeaponType Mace
        {
            get { return new WeaponType("Mace", WeaponFamily.MartialArms, PhysicalDamageType.Strike, 5, 7); }
        }
        public static WeaponType Warhammer
        {
            get { return new WeaponType("Warhammer", WeaponFamily.MartialArms, PhysicalDamageType.Strike, 5, 7); }
        }
        #endregion MartialWeapons
        #region StrengthWeapons
        public static WeaponType Greatsword
        {
            get { return new WeaponType("Greatsword", WeaponFamily.StrengthLargeArms, PhysicalDamageType.Slash, 7, 9); }
        }
        public static WeaponType Greataxe
        {
            get { return new WeaponType("Greataxe", WeaponFamily.StrengthLargeArms, PhysicalDamageType.Slash, 7, 9); }
        }
        public static WeaponType Greathammer
        {
            get { return new WeaponType("Greathammer", WeaponFamily.StrengthLargeArms, PhysicalDamageType.Strike, 7, 9); }
        }
        #endregion StrengthWeapons

        public static List<WeaponType> AllWeaponTypes { get; } = new List<WeaponType>()
        {
            Dagger,
            Rapier,
            Shortsword,
            HandAxe,
            Cudgel,
            Spear,
            Longsword,
            BattleAxe,
            Mace,
            Warhammer,
            Greatsword,
            Greataxe,
            Greathammer
        };
        public static List<WeaponType> DextrousWeaponTypes { get; } = new List<WeaponType>()
        {
            Dagger,
            Rapier,
            Shortsword
        };
        public static List<WeaponType> PrimitiveWeaponTypes { get; } = new List<WeaponType>()
        {
            HandAxe,
            Cudgel,
            Spear
        };
        public static List<WeaponType> MartialWeaponTypes { get; } = new List<WeaponType>()
        {
            Longsword,
            BattleAxe,
            Mace,
            Warhammer
        };
        public static List<WeaponType> StrengthWeaponTypes { get; } = new List<WeaponType>()
        {
            Greatsword,
            Greataxe,
            Greathammer
        };
        public string Name { get; }
        public WeaponFamily Family { get; }
        public PhysicalDamageType PrimaryAttack { get; }
        public PhysicalDamageType SecondaryAttack { get; }
        private int minDamage, maxDamage;
        public int MinDamage
        {
            get
            {
                if (Proficient)
                {
                    return minDamage += Combat.PROFICIENCY_DAMAGE_MODIFIER;
                }
                return minDamage;
            }
            private set
            {
                minDamage = value;
            }
        }
        public int MaxDamage
        {
            get
            {
                if (Proficient)
                {
                    return maxDamage += Combat.PROFICIENCY_DAMAGE_MODIFIER;
                }
                return maxDamage;
            }
            private set
            {
                maxDamage = value;
            }
        }
        public bool Proficient
        {
            get
            {
                if (PlayerEntity.Instance.Class.WeaponProficiency == Family)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public WeaponType(string name, WeaponFamily family, PhysicalDamageType primaryAttack, int minDamage, int maxDamage, PhysicalDamageType secondaryAttack = PhysicalDamageType.None)
        {
            Name = name;
            Family = family;
            PrimaryAttack = primaryAttack;
            SecondaryAttack = secondaryAttack;
            MinDamage = minDamage;
            MaxDamage = maxDamage;
        }
    }
}