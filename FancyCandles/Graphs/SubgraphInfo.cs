﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyCandles.Graphs
{
    public class SubgraphInfo
    {
        public string Name { get; internal set; }
        public Func<int,string> GetValue;
    }
}
