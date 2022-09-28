using System.Collections.Generic;
using System.Windows;


namespace FancyCandles.Graphs
{
    public class TrueRangeTicksElement : TickElementTemplate
    {
        public int MaxFractionalDigits
        {
            get { return (int)GetValue(MaxFractionalDigitsProperty); }
            set { SetValue(MaxFractionalDigitsProperty, value); }
        }
        public static readonly DependencyProperty MaxFractionalDigitsProperty =
            DependencyProperty.Register("MaxFractionalDigits", typeof(int), typeof(TrueRangeTicksElement), new FrameworkPropertyMetadata(0));


        public override double GetMostRoundValue(Dictionary<string, double> visibleCandlesExtremums)
        {
            return MyWpfMath.HighestDecimalPlace(visibleCandlesExtremums[UpperTag], out _);
        }


        public override string ToLabelString(double value)
        {
            string decimalSeparator = Culture.NumberFormat.NumberDecimalSeparator;
            char[] decimalSeparatorArray = decimalSeparator.ToCharArray();
            string currentPriceLabelNumberFormat = $"N{MaxFractionalDigits}";
            string currentPriceString = MyNumberFormatting.PriceToString(value, currentPriceLabelNumberFormat, Culture, decimalSeparator, decimalSeparatorArray);
            return currentPriceString;
        }
    }
}
