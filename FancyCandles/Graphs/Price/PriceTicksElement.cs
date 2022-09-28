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
    class PriceTicksElement : TickElementTemplate
    {

        public int MaxNumberOfFractionalDigitsInPrice
        {
            get { return (int)GetValue(MaxNumberOfFractionalDigitsInPriceProperty); }
            set { SetValue(MaxNumberOfFractionalDigitsInPriceProperty, value); }
        }
        public static readonly DependencyProperty MaxNumberOfFractionalDigitsInPriceProperty =
            DependencyProperty.Register("MaxNumberOfFractionalDigitsInPrice", typeof(int), typeof(PriceTicksElement), new FrameworkPropertyMetadata(0));

        public override double GetMostRoundValue(Dictionary<string, double> visibleCandlesExtremums)
        {
            return MyWpfMath.TheMostRoundValueInsideRange(VisibleCandlesExtremums[LowerTag], VisibleCandlesExtremums[UpperTag]);
        }

        public override string ToLabelString(double value)
        {
            string decimalSeparator = Culture.NumberFormat.NumberDecimalSeparator;
            char[] decimalSeparatorArray = decimalSeparator.ToCharArray();
            string currentPriceLabelNumberFormat = $"N{MaxNumberOfFractionalDigitsInPrice}";
            string currentPriceString = MyNumberFormatting.PriceToString(value, currentPriceLabelNumberFormat, Culture, decimalSeparator, decimalSeparatorArray);
            return currentPriceString;
        }
    }
}
