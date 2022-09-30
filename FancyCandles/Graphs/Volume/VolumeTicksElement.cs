/* 
    Copyright 2019 Dennis Geller.

    This file is part of FancyCandles.

    FancyCandles is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    FancyCandles is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with FancyCandles.  If not, see<https://www.gnu.org/licenses/>. */

using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Media;
using System.Globalization;

namespace FancyCandles.Graphs
{
    class VolumeTicksElement : SubgraphTickTemplate
    {


        public override string ToLabelString(double value)
        {
            string decimalSeparator = Culture.NumberFormat.NumberDecimalSeparator;
            char[] decimalSeparatorArray = decimalSeparator.ToCharArray();
            return MyNumberFormatting.VolumeToLimitedLengthString(value, Culture, decimalSeparator, decimalSeparatorArray);
        }


        public override double GetMostRoundValue(Dictionary<string, double> visibleCandlesExtremums)
        {
            return MyWpfMath.HighestDecimalPlace(VisibleCandlesExtremums[UpperTag], out _);
        }
    }
}
