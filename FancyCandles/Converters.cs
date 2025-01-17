﻿/* 
    Copyright 2021 Dennis Geller.

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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;
using FancyPrimitives;
using System.Linq;
using System.Reflection;
using FancyCandles.Graphs;

namespace FancyCandles
{
    //*******************************************************************************************************************************************************************
    class IsCandlesLoadingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int? candlesSourceCount = value as int?;
            return candlesSourceCount != null && candlesSourceCount == 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    //*******************************************************************************************************************************************************************
    class TypeToStaticNamePropertyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Type t = (Type)value;
            PropertyInfo propInfo = t.GetProperty("StaticName", BindingFlags.Public | BindingFlags.Static);
            return propInfo.GetValue(null, null);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    //*******************************************************************************************************************************************************************
    class HorizontalMarginToMarginsConverter : DependencyObject, IMultiValueConverter
    {
        // values[0] - Thickness LegendMargin
        // values[1] - HorizontalAlignment LegendHorizontalAlignment
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || values[0]==DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue) 
                return null;

            Thickness legendMargin = (Thickness)values[0];
            HorizontalAlignment legendHorizontalAlignment = (HorizontalAlignment)values[1];

            if (legendHorizontalAlignment == HorizontalAlignment.Right)
            {
                if (targetType == typeof(string))
                    return legendMargin.Right.ToString();
                else
                    return legendMargin.Right;
            }
            else
            {
                if (targetType == typeof(string))
                    return legendMargin.Left.ToString();
                else
                    return legendMargin.Left;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            CandleChartPropertiesWindow parentCandleChartPropertiesWindow = Application.Current.Windows.OfType<CandleChartPropertiesWindow>().FirstOrDefault();
            CandleChart parentCandleChart = (CandleChart)parentCandleChartPropertiesWindow.DataContext;
            Thickness oldLegendMargin = parentCandleChart.LegendMargin;

            FancyPrimitives.MyUtility.ParseStringAsDouble(value as string, out double newIndent);

            object[] arr = new object[2];
            arr[0] = new Thickness(newIndent, oldLegendMargin.Top, newIndent, oldLegendMargin.Bottom);
            return arr;
        }
    }
    //*******************************************************************************************************************************************************************
    class VerticalMarginToMarginsConverter : DependencyObject, IMultiValueConverter
    {
        // values[0] - Thickness LegendMargin
        // values[1] - VerticalAlignment LegendVerticalAlignment
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
                return null;

            Thickness legendMargin = (Thickness)values[0];
            VerticalAlignment legendVerticalAlignment = (VerticalAlignment)values[1];

            if (legendVerticalAlignment == VerticalAlignment.Top)
            {
                if (targetType == typeof(string))
                    return legendMargin.Top.ToString();
                else
                    return legendMargin.Top;
            }
            else
            {
                if (targetType == typeof(string))
                    return legendMargin.Bottom.ToString();
                else
                    return legendMargin.Bottom;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            CandleChartPropertiesWindow parentCandleChartPropertiesWindow = Application.Current.Windows.OfType<CandleChartPropertiesWindow>().FirstOrDefault();
            CandleChart parentCandleChart = (CandleChart)parentCandleChartPropertiesWindow.DataContext;
            Thickness oldLegendMargin = parentCandleChart.LegendMargin;

            FancyPrimitives.MyUtility.ParseStringAsDouble(value as string, out double newIndent);

            object[] arr = new object[2];
            arr[0] = new Thickness(oldLegendMargin.Left, newIndent, oldLegendMargin.Right, newIndent);
            return arr;
        }
    }
    //*******************************************************************************************************************************************************************
    class CrossPriceMarginConverter : IMultiValueConverter
    {
        // values[0] - Point CurrentMousePosition
        // values[1] - double PriceAxisTickLabelHeight
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2 || (values[0]).GetType() != typeof(Point) || (values[1]).GetType() != typeof(double))
                return new Thickness(0, 0, 0, 0);

            Point currentMousePosition = (Point)values[0];
            double priceAxisTickLabelHeight = (double)values[1];
            return new Thickness(0, currentMousePosition.Y - priceAxisTickLabelHeight / 2.0, 0, 0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }
    }
    //*******************************************************************************************************************************************************************
    class CrossTimeMarginConverter : IMultiValueConverter
    {
        // values[0] - Point CurrentMousePosition
        // values[1] - double PriceAxisTickLabelHeight
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2 || (values[0]).GetType() != typeof(Point) || (values[1]).GetType() != typeof(double))
                return new Thickness(0, 0, 0, 0);

            Point currentMousePosition = (Point)values[0];
            double timeAxisHeight = (double)values[1];
            return new Thickness(currentMousePosition.X, 0, 0, 0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }
    }
    //*******************************************************************************************************************************************************************
    class CrossPriceValueConverter : IMultiValueConverter
    {
        // values[0] - Point CurrentMousePosition
        // values[1] - double ChartAreaHeight
        // values[2] - CandleExtremums visibleCandlesExtremums
        // values[3] - double PriceChartTopMargin
        // values[4] - double PriceChartBottomMargin
        // values[5] - int MaxNumberOfFractionalDigitsInPrice
        // values[6] - CultureInfo candleChartCulture
        // values[7] - string UpperTag
        // values[8] - string LowerTag
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 6 || (values[0]).GetType() != typeof(Point)
                || (values[1]).GetType() != typeof(double) || (values[2]).GetType() != typeof(Dictionary<string,double>)
                || (values[3]).GetType() != typeof(double) || (values[4]).GetType() != typeof(double)
                || (values[5]).GetType() != typeof(int) || (values[6]).GetType() != typeof(CultureInfo)
                || (values[7]).GetType() != typeof(string) || (values[8]).GetType() != typeof(string)
                )
                return true;

            Point currentMousePosition = (Point)values[0];
            double ChartAreaHeight = (double)values[1];
            Dictionary<string, double> visibleCandlesExtremums = (Dictionary<string, double>)values[2];
            double priceLow = 0, priceHigh = 0;
            string upperTag = (string)values[7];
            string lowerTag = (string)values[8];
            if (visibleCandlesExtremums.ContainsKey(upperTag) && visibleCandlesExtremums.ContainsKey(lowerTag))
            {
                priceLow = ((Dictionary<string,double>)values[2])[lowerTag];
                priceHigh = ((Dictionary<string,double>)values[2])[upperTag];
            }
            double chartTopMargin = (double)values[3];
            double chartBottomMargin = (double)values[4];
            int maxNumberOfFractionalDigitsInPrice = (int)values[5];

            CultureInfo candleChartCulture = (CultureInfo)values[6];
            string decimalSeparator = candleChartCulture.NumberFormat.NumberDecimalSeparator;
            char[] decimalSeparatorArray = decimalSeparator.ToCharArray();

            // 用差值法算出游標位置的 price 是多少
            double price = Math.Round((priceHigh - (currentMousePosition.Y - chartTopMargin) / (ChartAreaHeight - chartTopMargin - chartBottomMargin) * (priceHigh - priceLow)), maxNumberOfFractionalDigitsInPrice);
            string priceNumberFormat = $"N{maxNumberOfFractionalDigitsInPrice}";
            return MyNumberFormatting.PriceToString(price, priceNumberFormat, candleChartCulture, decimalSeparator, decimalSeparatorArray);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }
    }
    //*******************************************************************************************************************************************************************
    class CrossVolumeConverter : IMultiValueConverter
    {
        // values[0] - Point CurrentMousePosition
        // values[1] - double VolumeHistogramHeight
        // values[2] - CandleExtremums visibleCandlesExtremums
        // values[3] - double VolumeHistogramTopMargin
        // values[4] - double VolumeHistogramBottomMargin
        // values[5] - CultureInfo candleChartCulture
        // values[6] - string UpperTag
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 5 || (values[0]).GetType() != typeof(Point) 
                 || (values[1]).GetType() != typeof(double) || (values[2]).GetType() != typeof(Dictionary<string,double>)
                 || (values[3]).GetType() != typeof(double) || (values[4]).GetType() != typeof(double)
                 || (values[6]).GetType() != typeof(string)
                 )
            {
                return "";
            }

            Point currentMousePosition = (Point)values[0];
            double volumeHistogramHeight = (double)values[1];
            Dictionary<string,double> visibleCandlesExtremums = (Dictionary<string,double>)values[2];
            double volumeHistogramTopMargin = (double)values[3];
            double volumeHistogramBottomMargin = (double)values[4];
            string upperTag = (string)values[6];

            CultureInfo candleChartCulture = (CultureInfo)values[5];
            string decimalSeparator = candleChartCulture.NumberFormat.NumberDecimalSeparator;
            char[] decimalSeparatorArray = decimalSeparator.ToCharArray();

            double volume = 0;
            if (visibleCandlesExtremums.ContainsKey(upperTag))
                volume = (((visibleCandlesExtremums[upperTag] - (currentMousePosition.Y - volumeHistogramTopMargin) / (volumeHistogramHeight - volumeHistogramTopMargin - volumeHistogramBottomMargin) * visibleCandlesExtremums[upperTag])));
            return MyNumberFormatting.VolumeToLimitedLengthString(volume, candleChartCulture, decimalSeparator, decimalSeparatorArray);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }
    }
    //*******************************************************************************************************************************************************************
    class CrossSubgraphValueConverter : IMultiValueConverter
    {
        // values[0] - Point CurrentMousePosition
        // values[1] - double VolumeHistogramHeight
        // values[2] - CandleExtremums visibleCandlesExtremums
        // values[3] - double VolumeHistogramTopMargin
        // values[4] - double VolumeHistogramBottomMargin
        // values[5] - CultureInfo candleChartCulture
        // values[6] - string UpperTag
        // values[7] - string LowerTag
        // values[8] - int MaxFractionalDigits
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 5 || (values[0]).GetType() != typeof(Point) 
                 || (values[1]).GetType() != typeof(double) || (values[2]).GetType() != typeof(Dictionary<string,double>)
                 || (values[3]).GetType() != typeof(double) || (values[4]).GetType() != typeof(double)
                 || (values[6]).GetType() != typeof(string) || (values[7]).GetType() != typeof(string)
                 || (values[8]).GetType() != typeof(int)
                 )
            {
                return "";
            }

            Point currentMousePosition = (Point)values[0];
            double renderHeight = (double)values[1];
            Dictionary<string,double> visibleCandlesExtremums = (Dictionary<string,double>)values[2];
            double topMargin = (double)values[3];
            double btmMargin = (double)values[4];
            string upperTag = (string)values[6];
            string lowerTag = (string)values[7];
            int maxDigits = (int)values[8];

            CultureInfo candleChartCulture = (CultureInfo)values[5];
            string decimalSeparator = candleChartCulture.NumberFormat.NumberDecimalSeparator;
            char[] decimalSeparatorArray = decimalSeparator.ToCharArray();

            double value = 0;
            if (visibleCandlesExtremums.ContainsKey(upperTag))
            {
                value = (((visibleCandlesExtremums[upperTag] - (currentMousePosition.Y - topMargin) / (renderHeight - topMargin - btmMargin) * visibleCandlesExtremums[upperTag])));
                string priceNumberFormat = $"N{maxDigits}";
                return MyNumberFormatting.PriceToString(value, priceNumberFormat, candleChartCulture, decimalSeparator, decimalSeparatorArray);
            }
            return "";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }
    }
    //*******************************************************************************************************************************************************************
    class VerticalCrossLineVisibilityConverter : IMultiValueConverter
    {
        // values[0] - bool IsCrossLinesVisible
        // values[1] - bool IsMouseOverPriceChart
        // values[2] - bool IsMouseOverVolumeHistogram
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 3 || (values[0]).GetType() != typeof(bool) || (values[1]).GetType() != typeof(bool) || (values[2]).GetType() != typeof(bool))
                return true;

            bool isCrossLinesVisible = (bool)values[0];
            bool isMouseOverPriceChart = (bool)values[1];
            bool isMouseOverVolumeHistogram = (bool)values[2];
            return (isCrossLinesVisible && (isMouseOverPriceChart || isMouseOverVolumeHistogram)) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }
    }
    //*******************************************************************************************************************************************************************
    class CandleDrawingParametersConverter : IMultiValueConverter
    {
        // values[0] - double CandleWidth
        // values[1] - double CandleGap
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double candleW = values[0] == DependencyProperty.UnsetValue ? 0.0 : (double)values[0];
            double candleG = values[1] == DependencyProperty.UnsetValue ? 0.0 : (double)values[1];
            return new CandleDrawingParameters(candleW, candleG);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }
    }
    //*******************************************************************************************************************************************************************
    class SquareBoolToVisibilityConverter : IMultiValueConverter
    {
        // values[0] - bool bool1
        // values[1] - bool bool2
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2 || (values[0]).GetType() != typeof(bool) || (values[1]).GetType() != typeof(bool))
            {
                return Visibility.Collapsed;
            }

            bool bool0 = (bool)values[0];
            bool bool1 = (bool)values[1];
            return (bool0 && bool1) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }
    }
    //*******************************************************************************************************************************************************************
    class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object bool_value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)bool_value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    //*******************************************************************************************************************************************************************
    // Opposite to BoolToVisibilityConverter
    class NotBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object bool_value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)bool_value) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    //*******************************************************************************************************************************************************************
    class IntRange_Start_i_Converter : IValueConverter
    {
        // value - IntRange visibleCandlesRange
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IntRange visibleCandlesRange = (IntRange)value;
            return visibleCandlesRange.Start_i;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int start_i = System.Convert.ToInt32((double)value);
            IntRange visibleCandlesRange = IntRange.CreateContainingOnlyStart_i(start_i);
            return visibleCandlesRange;
        }
    }
    //*******************************************************************************************************************************************************************
    class IntRangeCountToNoNegativeConverter : IValueConverter
    {
        // value - IntRange visibleCandlesRange
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IntRange visibleCandlesRange = (IntRange)value;
            int count = visibleCandlesRange.Count;
            if (count < 0)
                count = 0;
            return count;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int start_i = System.Convert.ToInt32((double)value);
            IntRange visibleCandlesRange = IntRange.CreateContainingOnlyStart_i(start_i);
            return visibleCandlesRange;
        }
    }
    //*******************************************************************************************************************************************************************
    // Возвращает максимально допустимое значение для свойства FirstCandle_csi.
    class FirstCandleMaxIndexConverter : IMultiValueConverter
    {
        // values[0] - ObservableCollection<Candle> candles
        // values[1] - IntRange VisibleCandlesRange
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2 || (values[1]).GetType() != typeof(IntRange))
                return 0.0;

            IList<ICandle> candles = (IList<ICandle>)values[0];
            if (candles == null)
                return (double)int.MaxValue;
            else
            {
                int candlesCount = ((IntRange)values[1]).Count;
                if (candlesCount == 0)
                    return (double)int.MaxValue;
                return (double)(candles.Count - candlesCount);
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }
    }
    //*******************************************************************************************************************************************************************
    // Возвращает Margin для candlesItemsControl.
    class TopBottomMarginConverter : IMultiValueConverter
    {
        // values[0] - double topMargin
        // values[1] - double bottomMargin
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2 || (values[0]).GetType() != typeof(double) || (values[1]).GetType() != typeof(double))
                return new Thickness(0, 0, 0, 0);

            double topMargin = (double)values[0];
            double bottomMargin = (double)values[1];
            return new Thickness(0, topMargin, 0, bottomMargin);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }
    }
    //*******************************************************************************************************************************************************************
    // overlay indicator get value
    class OverlayIndicatorValueConverter : IMultiValueConverter
    {
        // values[0] - int              selectedCandleId
        // values[1] - OverlayIndicator indicator
        // values[2] - int              maxNumberOfFractionalDigitsInPrice
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 3 || (values[0]).GetType() != typeof(int) ||
                !(values[1]).GetType().IsSubclassOf(typeof(Indicators.OverlayIndicator)) ||
                (values[2]).GetType() != typeof(int))
                return "--";

            int selectedCandleId = (int)values[0];
            Indicators.OverlayIndicator indicator = (Indicators.OverlayIndicator)values[1];
            int digits = (int)values[2];
            //double value = indicator.GetIndicatorValue(selectedCandleId);
            return indicator.GetIndicatorValue(selectedCandleId).ToString($"N{digits}");
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }
    }
    //*******************************************************************************************************************************************************************
    // subgraph info get value
    class SubgraphInfoValueConverter : IMultiValueConverter
    {
        // values[0] - int          selectedCandleId
        // values[1] - SubgraphInfo info
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2 || (values[0]).GetType() != typeof(int) ||
                (values[1]).GetType() !=typeof(Graphs.SubgraphInfo))
            {
                return "--";
            }

            int selectedCandleId = (int)values[0];
            Graphs.SubgraphInfo info = (Graphs.SubgraphInfo)values[1];
            return info.GetValue(selectedCandleId);
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }
    }
    //*******************************************************************************************************************************************************************
    // CandleInfo price converter
    class CandleInfoPriceConverter : IMultiValueConverter
    {
        // values[0] - double   price to be converted
        // values[1] - int      maxFractionalDigitsInPrice
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2 || (values[0]).GetType() != typeof(double) ||
                (values[1]).GetType() != typeof(int))
            {
                return "--";
            }

            double price = (double)values[0];
            int digits = (int)values[1];
            string fmt = $"N{digits}"; 
            return price.ToString(fmt, CultureInfo.InvariantCulture);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }
    }
    //*******************************************************************************************************************************************************************
    //*******************************************************************************************************************************************************************
    //*******************************************************************************************************************************************************************
}
