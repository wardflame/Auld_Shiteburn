﻿using AuldShiteburn.EntityData;
using AuldShiteburn.ItemData;
using AuldShiteburn.ItemData.ArmourData;
using AuldShiteburn.ItemData.ConsumableData;
using AuldShiteburn.ItemData.KeyData;
using AuldShiteburn.ItemData.WeaponData;
using System;
using System.Collections.Generic;

namespace AuldShiteburn.MapData.TileData.Tiles
{
    [Serializable]
    internal class LootTile : Tile
    {
        private const float CHANCE_KEY = 0.95f;
        private const float CHANCE_WEAPON = 0.6f;
        private const float CHANCE_ARMOUR = 0.4f;
        private const float CHANCE_CONSUMABLE = 0.0f;

        public override bool Collidable => Looted ? false : true;
        public override ConsoleColor Foreground => Looted ? ConsoleColor.DarkGray : ConsoleColor.Magenta;
        string Message { get; }
        public List<Item> Items { get; } = new List<Item>();
        public bool Temporary { get; set; }
        public bool Looted { get; set; } = false;

        public LootTile(string message, List<Item> itemList, bool randomised, bool temporary = false) : base("?", false)
        {
            Message = message;
            Temporary = temporary;
            if (!randomised)
            {
                Items = itemList;
            }
            /// If the items in the loot tile aren't fixed, generate them by chance.
            else
            {
                Random rand = new Random();
                double chance = rand.NextDouble();
                if (chance >= CHANCE_KEY)
                {
                    KeyItem lootKey = KeyItem.AllKeys[rand.Next(KeyItem.AllKeys.Count)];
                    Items.Add(lootKey);
                }
                chance = rand.NextDouble();
                if (chance >= CHANCE_WEAPON)
                {
                    Items.Add(WeaponItem.GenerateWeapon());
                }
                chance = rand.NextDouble();
                if (chance >= CHANCE_ARMOUR)
                {
                    ArmourItem armour = ArmourItem.AllArmours[rand.Next(ArmourItem.AllArmours.Count)];
                    Items.Add(armour);
                }
                chance = rand.NextDouble();
                if (chance >= CHANCE_CONSUMABLE)
                {
                    ConsumableItem consumable = ConsumableItem.AllConsumables[rand.Next(ConsumableItem.AllConsumables.Count)];
                    Items.Add(consumable);
                }
            }
        }

        public static bool GenerateLootTile()
        {
            int spawnX = PlayerEntity.Instance.PosX;
            int spawnY = PlayerEntity.Instance.PosY;
            bool tileFound = false;
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }
                    int checkX = PlayerEntity.Instance.PosX + x;
                    int checkY = PlayerEntity.Instance.PosY + y;
                    Tile emptyTile = Map.Instance.CurrentArea.GetTile(checkX, checkY);
                    if (emptyTile != null && emptyTile.DisplayChar == Tile.AirTile.DisplayChar)
                    {
                        spawnX = checkX;
                        spawnY = checkY;
                        tileFound = true;
                        break;
                    }
                }
                if (tileFound)
                {
                    break;
                }
            }
            if (!tileFound)
            {
                return false;
            }
            Map.Instance.CurrentArea.SetTile(spawnX, spawnY,
                new LootTile("Loot Pile", new List<Item>(), true, true));
            Map.Instance.PrintTile(spawnX, spawnY);
            return true;
        }

        public override void OnCollision(Entity entity)
        {
            foreach (Item item in Items.ToArray())
            {
                LootStock();
                if (PlayerEntity.Instance.Inventory.AddItem(item, true))
                {
                    Items.Remove(item);
                }
            }
            if (LootStock() == 0)
            {
                if (Temporary)
                {
                    Map.Instance.CurrentArea.SetTile(entity.PosX, entity.PosY, AirTile);
                }
                else
                {
                    Items.Clear();
                    Looted = true;
                }
            }
            Utils.ClearInteractInterface();
            Utils.ClearPlayerInventoryInterface();
            PlayerEntity.Instance.PrintInventory();
        }

        private int LootStock()
        {
            Utils.ClearInteractInterface();
            Utils.SetCursorInteract();
            Utils.WriteColour($"{Message}: ", ConsoleColor.DarkYellow);
            int lootStock = 0;
            foreach (Item item in Items)
            {
                lootStock++;
                Utils.SetCursorInteract(lootStock);
                Utils.WriteColour($"{lootStock}. {item.Name}");
            }
            return lootStock;
        }
    }
}
