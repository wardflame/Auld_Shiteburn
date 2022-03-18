﻿using AuldShiteburn.EntityData;
using System;

namespace AuldShiteburn.MapData.TileData.Tiles
{
    class PassageTile : Tile
    {
        public Direction Direction { get; set; }

        public PassageTile() : base("-", true, ConsoleColor.DarkYellow)
        {
        }

        public override void OnCollision(Entity entity, Area area)
        {
            if (entity is PlayerEntity)
            {
                Map.Instance.MoveArea(Direction);
            }
        }
    }
}