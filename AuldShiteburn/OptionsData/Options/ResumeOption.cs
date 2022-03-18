﻿using AuldShiteburn.ArtData;
using AuldShiteburn.MapData;
using AuldShiteburn.OptionData;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuldShiteburn.OptionsData.Options
{
    internal class ResumeOption : Option
    {
        public override string DisplayString => ASCIIArt.menuResume;

        public override void OnUse()
        {
            Console.Clear();
            Map.Instance.PrintMap();
        }
    }
}
