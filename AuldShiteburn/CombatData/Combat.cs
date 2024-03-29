﻿using AuldShiteburn.ArtData;
using AuldShiteburn.CombatData.AbilityData;
using AuldShiteburn.CombatData.PayloadData;
using AuldShiteburn.CombatData.StatusEffectData.StatusEffects;
using AuldShiteburn.EntityData;
using AuldShiteburn.ItemData.WeaponData;
using AuldShiteburn.MapData;
using System;
using System.Collections.Generic;

namespace AuldShiteburn.CombatData
{
    [Serializable]
    internal class Combat
    {
        #region Modifier Constants
        #region Proficiency Modifiers
        public const int PROFICIENCY_DAMAGE_BONUS_MINOR = 2;
        public const int PROFICIENCY_DAMAGE_BONUS_MODERATE = 4;
        public const int PROFICIENCY_DAMAGE_BONUS_MAJOR = 6;
        public const int PROFICIENCY_ARMOUR_MITIGATION_MINOR = 2;
        public const int PROFICIENCY_ARMOUR_MITIGATION_MODERATE = 4;
        public const int PROFICIENCY_ARMOUR_MITIGATION_MAJOR = 6;
        #endregion Proficiency Modifiers
        #region Status Modifiers
        public const int STATUS_MITIGATION_MINOR = 2;
        public const int STATUS_MITIGATION_MODERATE = 4;
        public const int STATUS_MITIGATION_MAJOR = 6;
        #endregion Status Modifiers
        public const int ARMOUR_RESISTANCE_MITIGATION_MODIFIER = 2;
        public const int WEAKNESS_BONUS_MODIFIER = 2;
        #endregion Modifier Constants

        private static int RoundNumber { get; set; }

        /// <summary>
        /// Keep player and enemies fighting in a loop until all enemies are dead
        /// or the player has died.
        /// </summary>
        /// <param name="enemies">Enemies for the player to fight.</param>
        /// <returns>True if the player wins, false if the player dies.</returns>
        public static bool CombatEncounter(List<EnemyEntity> enemies)
        {
            RoundNumber = 1;
            while (enemies.Count > 0 && PlayerEntity.Instance.HP > 0)
            {
                PlayerCombatTurn(enemies);
                EnemyCombatTurn(enemies);
                AssessStatus(enemies);
                RoundNumber++;
            }
            if (PlayerEntity.Instance.HP > 0)
            {
                Utils.SetCursorInteract();
                if (Map.Instance.CurrentArea.BossArea)
                {
                    ASCIIArt.PrintASCII(ASCIIArt.VICTORY_BOSS, ConsoleColor.DarkYellow);
                }
                else
                {
                    ASCIIArt.PrintASCII(ASCIIArt.VICTORY_MESSAGE, ConsoleColor.Green);
                }
                Console.CursorTop += 2;
                Utils.WriteColour("Press any key to continue.");
                Console.ReadKey(true);
                Utils.ClearInteractInterface();
                PlayerEntity.Instance.HP = PlayerEntity.Instance.MaxHP;
                if (PlayerEntity.Instance.UsesMana)
                {
                    PlayerEntity.Instance.Mana = PlayerEntity.Instance.MaxMana;
                }
                else if (PlayerEntity.Instance.UsesStamina)
                {
                    PlayerEntity.Instance.Stamina = PlayerEntity.Instance.MaxStamina;
                }
                foreach (Ability ability in PlayerEntity.Instance.Class.Abilities)
                {
                    ability.ActiveCooldown = 0;
                }
                PlayerEntity.Instance.PrintStats();
                return true;
            }
            else
            {
                Utils.SetCursorInteract();
                ASCIIArt.PrintASCII(ASCIIArt.DEATH_MESSAGE, ConsoleColor.Red);
                Console.CursorTop += 2;
                Utils.WriteColour("Press any key to continue.");
                Console.ReadKey(true);
                Console.Clear();
                Game.playing = false;
                Game.running = false;
                return false;
            }
        }

        /// <summary>
        /// Present the player with combat targets and have them iterate through
        /// the list and choose a target. Then, present them with their combat options
        /// and deliver a combat payload to the chosen enemy.
        /// </summary>
        /// <param name="enemies">List of enemies in the area.</param>
        private static void PlayerCombatTurn(List<EnemyEntity> enemies)
        {
            bool playerTurn = true;
            bool abilityCooldowns = false;
            while (playerTurn)
            {
                #region Ability Cooldowns
                if (RoundNumber > 1)
                {

                    if (!abilityCooldowns)
                    {
                        // Iterate through player abilities. If they have a cooldown active, decrement it.
                        int i = 1;
                        int abilitiesCoolingDown = 0;
                        Utils.SetCursorInteract();
                        Utils.WriteColour("Cooldowns", ConsoleColor.DarkYellow);
                        foreach (Ability ability in PlayerEntity.Instance.Class.Abilities)
                        {
                            Utils.SetCursorInteract(i++);
                            if (ability.ActiveCooldown > 0)
                            {
                                Utils.WriteColour($"{ability.Name} cooling down: {ability.ActiveCooldown}/{ability.Cooldown}", ConsoleColor.Magenta);
                                ability.ActiveCooldown--;
                                abilitiesCoolingDown++;
                            }
                            else
                            {
                                Utils.WriteColour($"{ability.Name}: ", ConsoleColor.Magenta);
                                Utils.WriteColour($"Ready", ConsoleColor.DarkGreen);
                            }
                        }
                        Utils.SetCursorInteract(Console.CursorTop);
                        Utils.WriteColour("Press any key to continue.");
                        Console.ReadKey(true);
                        Utils.ClearInteractInterface();
                        abilityCooldowns = true;
                    }
                }
                #endregion Ability Cooldowns
                PlayerEntity.Instance.PrintStats();
                if (!PlayerEntity.Instance.Stunned)
                {
                    EnemyEntity enemy = ChooseEnemy(enemies);
                    int activity = ChooseActivity();
                    if (activity >= 0)
                    {
                        #region Activity 1: Melee Combat
                        if (activity == 0)
                        {
                            CombatPayload playerMeleePayload = ChooseMeleeAttack();
                            if (playerMeleePayload.IsAttack)
                            {
                                if (enemy.ReceiveAttack(playerMeleePayload, enemies.Count + 12))
                                {
                                    enemies.Remove(enemy);
                                }
                                playerTurn = false;
                            }
                            else
                            {
                                Utils.SetCursorInteract(enemies.Count + 2);
                                Utils.ClearInteractArea(length: 30);
                            }
                        }
                        #endregion Activity 1: Melee Combat
                        #region Activity 2: Ability Combat
                        else if (activity == 1)
                        {
                            Utils.ClearInteractArea(enemies.Count + 5, 20);
                            Utils.SetCursorInteract(enemies.Count + 4);
                            CombatPayload playerAbilityPayload = ChooseAbility(enemies);
                            if (playerAbilityPayload.IsAttack)
                            {
                                if (enemy.ReceiveAttack(playerAbilityPayload, Console.CursorTop - 1))
                                {
                                    enemies.Remove(enemy);
                                }
                                PlayerEntity.Instance.PrintStats();
                                playerTurn = false;
                            }
                            else if (playerAbilityPayload.IsUtility)
                            {
                                PlayerEntity.Instance.PrintStats();
                                playerTurn = false;
                            }
                            else
                            {
                                Utils.SetCursorInteract(enemies.Count + 2);
                                Utils.ClearInteractArea(length: 30);
                            }
                        }
                        #endregion Activity 2: Ability Combat
                    }
                    else
                    {
                        Utils.ClearInteractInterface(30);
                    }
                }
                else
                {
                    Utils.SetCursorInteract();
                    Utils.WriteColour("You are stunned, recovering in ");
                    Utils.WriteColour($"{PlayerEntity.Instance.StunTimer} ", ConsoleColor.Blue);
                    Utils.WriteColour($"rounds!");
                    playerTurn = false;
                }
            }
            #region Status Effect Actives
            if (PlayerEntity.Instance.AbilityStatusEffect != null && PlayerEntity.Instance.AbilityStatusEffect.GetType() == typeof(ReplenishStatusEffect) && RoundNumber > 1)
            {
                PlayerEntity.Instance.AbilityStatusEffect.EffectActive(new CombatPayload(false));
            }
            if (PlayerEntity.Instance.PotionStatusEffect != null && PlayerEntity.Instance.PotionStatusEffect.GetType() == typeof(ReplenishStatusEffect) && RoundNumber > 1)
            {
                PlayerEntity.Instance.PotionStatusEffect.EffectActive(new CombatPayload(false));
            }
            #endregion Status Effect Actives
            // Readkey to ensure player has a chance to read the round's report.
            Utils.SetCursorInteract(Console.CursorTop);
            Utils.WriteColour("Press any key to continue.");
            Console.ReadKey(true);
            Utils.ClearInteractInterface(30);
        }

        /// <summary>
        /// Present the player with combat targets and have them iterate through
        /// the list and choose a target. Then, present them with their combat options
        /// and deliver a combat payload to the chosen enemy.
        /// </summary>
        /// <param name="enemies">List of enemies in the area.</param>
        private static void EnemyCombatTurn(List<EnemyEntity> enemies)
        {
            bool enemyTurn = true;
            while (enemyTurn)
            {
                foreach (EnemyEntity enemy in enemies)
                {
                    if (!enemy.Stunned)
                    {
                        CombatPayload enemyAttack = enemy.PerformAttack(enemies);
                        PlayerEntity.Instance.ReceiveAttack(enemyAttack, aggressor: enemy);
                        // Readkey to ensure player has a chance to read the round's report.
                        Utils.SetCursorInteract(Console.CursorTop - 1);
                        Utils.WriteColour("Press any key to continue.");
                        Console.ReadKey(true);
                        Utils.ClearInteractInterface(30);
                    }
                }
                enemyTurn = false;
            }
        }

        /// <summary>
        /// Run through player entity and enemies and check for stuns and
        /// status effects. If any exist, decrement/remove them.
        /// </summary>
        /// <param name="enemies">List of enemies in current combat.</param>
        private static void AssessStatus(List<EnemyEntity> enemies)
        {
            #region Status Effects Duration
            if (PlayerEntity.Instance.AbilityStatusEffect != null)
            {
                PlayerEntity.Instance.AbilityStatusEffect.Duration--;
                if (PlayerEntity.Instance.AbilityStatusEffect.Duration == 0)
                {
                    PlayerEntity.Instance.AbilityStatusEffect = null;
                }
            }
            if (PlayerEntity.Instance.PotionStatusEffect != null)
            {
                PlayerEntity.Instance.PotionStatusEffect.Duration--;
                if (PlayerEntity.Instance.PotionStatusEffect.Duration == 0)
                {
                    PlayerEntity.Instance.PotionStatusEffect = null;
                }
            }
            foreach (EnemyEntity enemy in enemies)
            {
                if (enemy.StatusEffect != null)
                {
                    if (enemy.JustAfflicted)
                    {
                        enemy.JustAfflicted = false;
                    }
                    else
                    {
                        enemy.StatusEffect.Duration--;
                        if (enemy.StatusEffect.Duration == 0)
                        {
                            enemy.StatusEffect = null;
                        }
                    }
                }
            }
            #endregion Status Effects Duration
            #region Stun Duration
            if (PlayerEntity.Instance.Stunned)
            {
                if (PlayerEntity.Instance.JustStunned)
                {
                    PlayerEntity.Instance.JustStunned = false;
                }
                else
                {
                    PlayerEntity.Instance.StunTimer--;
                }

            }
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].Stunned)
                {
                    if (enemies[i].JustStunned)
                    {
                        enemies[i].JustStunned = false;
                    }
                    else
                    {
                        enemies[i].StunTimer--;
                    }
                }
            }
            #endregion Stun Duration
        }

        /// <summary>
        /// Generate a combat payload from the player's equipped weapon,
        /// randomising between the min and max physical and property
        /// damages.
        /// </summary>
        /// <returns>Combat payload for enemy to process.</returns>
        private static CombatPayload CalculateWeaponDamage(bool primaryElseSecondary)
        {
            CombatPayload attackPayload = new CombatPayload(true);
            Random rand = new Random();
            WeaponItem playerWeapon = PlayerEntity.Instance.EquippedWeapon;
            attackPayload.PhysicalDamage = rand.Next(playerWeapon.MinPhysDamage, playerWeapon.MaxPhysDamage + 1);
            if (attackPayload.PhysicalDamage > 0)
            {
                attackPayload.PhysicalAttackType = playerWeapon.Type.PrimaryAttack;
                if (!primaryElseSecondary && playerWeapon.Type.SecondaryAttack != PhysicalDamageType.None)
                {
                    attackPayload.PhysicalAttackType = playerWeapon.Type.SecondaryAttack;
                }
                attackPayload.HasPhysical = true;
            }
            attackPayload.PropertyDamage = rand.Next(playerWeapon.MinPropDamage, playerWeapon.MaxPropDamage + 1);
            if (attackPayload.PropertyDamage > 0 || playerWeapon.Property.Type == PropertyDamageType.Damaged)
            {
                attackPayload.PropertyAttackType = playerWeapon.Property.Type;
                attackPayload.HasProperty = true;
            }
            return attackPayload;
        }

        /// <summary>
        /// Iterate through a list of enemies and highlight one at an int index.
        /// The player moves the index with the up/down arrow keys. Pressing enter
        /// returns the enemy at that index.
        /// </summary>
        /// <param name="enemies">List of enemies to navigate.</param>
        /// <returns>Returns the chosen enemy.</returns>
        private static EnemyEntity ChooseEnemy(List<EnemyEntity> enemies)
        {
            Utils.SetCursorInteract();
            Utils.WriteColour("Choose an enemy to attack.", ConsoleColor.DarkYellow);
            int index = 0;
            do
            {
                Utils.ClearInteractArea(1, enemies.Count);
                for (int i = 0; i < enemies.Count; i++)
                {
                    Utils.SetCursorInteract(i + 1);
                    if (enemies[i] == enemies[index])
                    {
                        Utils.WriteColour(">>", ConsoleColor.Yellow);
                    }
                    Utils.WriteColour($"{enemies[i].Name} ", ConsoleColor.Cyan);
                    Utils.WriteColour($"{enemies[i].HP}/{enemies[i].MaxHP} ", ConsoleColor.Red);
                    Utils.WriteColour($"HP ");
                    bool status = enemies[i].StatusEffect != null;
                    bool stunned = enemies[i].Stunned;
                    if (status && stunned)
                    {
                        Utils.WriteColour($"{enemies[i].StatusEffect.Name}: {enemies[i].StatusEffect.Duration}", enemies[i].StatusEffect.DisplayColor);
                        Utils.WriteColour(", ");
                        Utils.WriteColour($"Stunned: {enemies[i].StunTimer}", ConsoleColor.Blue);
                    }
                    else if (status)
                    {
                        Utils.WriteColour($"{enemies[i].StatusEffect.Name}: {enemies[i].StatusEffect.Duration}", enemies[i].StatusEffect.DisplayColor);
                    }
                    else if (stunned)
                    {
                        Utils.WriteColour($"Stunned: {enemies[i].StunTimer}", ConsoleColor.Blue);
                    }
                }
                InputSystem.GetInput();
                switch (InputSystem.InputKey)
                {
                    case ConsoleKey.UpArrow:
                        {
                            if (index <= enemies.Count && index > 0)
                            {
                                index--;
                            }
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        {
                            if (index >= 0 && index < enemies.Count - 1)
                            {
                                index++;
                            }
                        }
                        break;
                }
            } while (InputSystem.InputKey != ConsoleKey.Enter);
            return enemies[index];
        }

        /// <summary>
        /// Iterate through a list of string options and highlight one at an int index.
        /// The player moves the index with the up/down arrow keys. Pressing enter
        /// returns the index for an activity.
        /// </summary>
        /// <param name="offsetY">Offset for SetCursorInteract() first parameter.</param>
        /// <returns>Return the activity number.</returns>
        private static int ChooseActivity()
        {
            List<string> activities = new List<string>() { "Use Equipped Weapon", "Use Ability" };
            int offsetY = Console.CursorTop;
            Utils.SetCursorInteract(offsetY);
            Utils.WriteColour("Choose an activity.", ConsoleColor.DarkYellow);
            int index = 0;
            do
            {
                Utils.ClearInteractArea(offsetY + 1, 2);
                for (int i = 0; i < activities.Count; i++)
                {
                    Utils.SetCursorInteract(offsetY + 1 + i);
                    if (i == index)
                    {
                        Utils.WriteColour(">>", ConsoleColor.Yellow);
                        Utils.WriteColour(activities[i], ConsoleColor.Cyan);
                    }
                    else
                    {
                        Utils.WriteColour(activities[i]);
                    }
                }
                Utils.SetCursorInteract(offsetY + activities.Count + 1);
                Utils.WriteColour("[");
                Utils.WriteColour("BACKSPACE", ConsoleColor.DarkGray);
                Utils.WriteColour("] Return");
                InputSystem.GetInput();
                switch (InputSystem.InputKey)
                {
                    case ConsoleKey.UpArrow:
                        {
                            if (index > 0 && index < activities.Count)
                            {
                                index--;
                            }
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        {
                            if (index >= 0 && index < activities.Count - 1)
                            {
                                index++;
                            }
                        }
                        break;
                    case ConsoleKey.Backspace:
                        {
                            return -1;
                        }
                }
            } while (InputSystem.InputKey != ConsoleKey.Enter);
            return index;
        }

        /// <summary>
        /// Iterate through a list of weapon attack types the weapon can perform and have the player choose one.
        /// If, by chance, a weapon's secondary attack is None, the weapon will default to the primary attack type
        /// in the CalculateWeaponDamage() method.
        /// </summary>
        /// <returns>CombatPayload based on the attack type the player chooses.</returns>
        private static CombatPayload ChooseMeleeAttack()
        {
            WeaponItem weapon = PlayerEntity.Instance.EquippedWeapon;
            List<string> meleeAttacks = new List<string>() { weapon.Type.PrimaryAttack.ToString(), weapon.Type.SecondaryAttack.ToString() };
            int offsetY = Console.CursorTop;
            Utils.ClearInteractArea(offsetY, 2);
            Utils.SetCursorInteract(offsetY);
            Utils.WriteColour("Choose an attack technique.", ConsoleColor.DarkYellow);
            int index = 0;
            do
            {
                Utils.ClearInteractArea(offsetY + 1, 2);
                for (int i = 0; i < meleeAttacks.Count; i++)
                {
                    Utils.SetCursorInteract(offsetY + 1 + i);
                    if (i == index)
                    {
                        Utils.WriteColour(">>", ConsoleColor.Yellow);
                        Utils.WriteColour(meleeAttacks[i], ConsoleColor.Cyan);
                    }
                    else
                    {
                        Utils.WriteColour(meleeAttacks[i]);
                    }
                }
                Utils.SetCursorInteract(offsetY + meleeAttacks.Count + 1);
                Utils.WriteColour("[");
                Utils.WriteColour("BACKSPACE", ConsoleColor.DarkGray);
                Utils.WriteColour("] Return");
                InputSystem.GetInput();
                switch (InputSystem.InputKey)
                {
                    case ConsoleKey.UpArrow:
                        {
                            if (index > 0 && index < meleeAttacks.Count)
                            {
                                index--;
                            }
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        {
                            if (index >= 0 && index < meleeAttacks.Count - 1)
                            {
                                index++;
                            }
                        }
                        break;
                    case ConsoleKey.Backspace:
                        {
                            return new CombatPayload(false);
                        }
                }
            } while (InputSystem.InputKey != ConsoleKey.Enter);
            if (index == 0)
            {
                return CalculateWeaponDamage(true);
            }
            else
            {
                return CalculateWeaponDamage(false);
            }
        }

        /// <summary>
        /// Iterate through a list of the player's class abilities. Provide details to the side
        /// of them. On pressing enter, activate the ability and return the combat payload.
        /// </summary>
        /// <returns>Returns the chosen ability's combat payload.</returns>
        private static CombatPayload ChooseAbility(List<EnemyEntity> enemies)
        {
            List<Ability> abilities = PlayerEntity.Instance.Class.Abilities;
            int offsetY = Console.CursorTop;
            Utils.ClearInteractArea(offsetY, 1);
            Utils.SetCursorInteract(offsetY);
            Utils.WriteColour("Choose an ability.", ConsoleColor.DarkYellow);
            int index = 0;
            do
            {
                Utils.ClearInteractArea(offsetY + 1, abilities.Count);
                for (int i = 0; i < abilities.Count; i++)
                {
                    Utils.SetCursorInteract(offsetY + 1 + i);
                    if (abilities[i] == abilities[index])
                    {
                        Utils.WriteColour(">>", ConsoleColor.Yellow);
                        Utils.WriteColour($"{abilities[i].Name}", ConsoleColor.Cyan);
                        Utils.SetCursorInteract(offsetY + 1, 25);
                        Utils.WriteColour(abilities[i].Description);
                        Utils.SetCursorInteract(offsetY + 2, 25);
                        Utils.WriteColour($"Cooldown: ");
                        if (abilities[i].ActiveCooldown > 0) Utils.WriteColour($"{abilities[i].ActiveCooldown}/{abilities[i].Cooldown}", ConsoleColor.Red);
                        else Utils.WriteColour($"{abilities[i].ActiveCooldown}/{abilities[i].Cooldown}", ConsoleColor.Green);
                        Utils.SetCursorInteract(offsetY + 3, 25);
                        Utils.WriteColour($"Resource Cost: {abilities[i].ResourceCost}");
                    }
                    else
                    {
                        Utils.SetCursorInteract(offsetY + 1 + i);
                        Utils.WriteColour(abilities[i].Name);
                    }
                }
                Utils.SetCursorInteract(offsetY + abilities.Count + 1);
                Utils.WriteColour("[");
                Utils.WriteColour("BACKSPACE", ConsoleColor.DarkGray);
                Utils.WriteColour("] Return");
                InputSystem.GetInput();
                switch (InputSystem.InputKey)
                {
                    case ConsoleKey.UpArrow:
                        {
                            if (index > 0 && index < abilities.Count)
                            {
                                index--;
                            }
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        {
                            if (index >= 0 && index < abilities.Count - 1)
                            {
                                index++;
                            }
                        }
                        break;
                    case ConsoleKey.Backspace:
                        {
                            return new CombatPayload(false);
                        }
                }
            } while (InputSystem.InputKey != ConsoleKey.Enter);
            Ability chosenAbility = PlayerEntity.Instance.Class.Abilities[index];
            if (chosenAbility.ActiveCooldown > 0)
            {
                Utils.SetCursorInteract(Console.CursorTop);
                Utils.WriteColour($"{chosenAbility.Name} is on cooldown {chosenAbility.ActiveCooldown}/{chosenAbility.Cooldown}.", ConsoleColor.Red);
                Console.ReadKey(true);
                return new CombatPayload(false);
            }
            else if (!PlayerEntity.Instance.CheckResourceLevel(chosenAbility.ResourceCost))
            {
                Utils.SetCursorInteract(Console.CursorTop);
                Utils.WriteColour($"You lack the resources to use this ability.", ConsoleColor.Red);
                Console.ReadKey(true);
                return new CombatPayload(false);
            }
            return PlayerEntity.Instance.Class.Abilities[index].UseAbility(enemies);
        }
    }

    [Serializable]
    public enum EffectLevel
    {
        None,
        Minor,
        Moderate,
        Major
    }
}
