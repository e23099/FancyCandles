using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;

using FancyCandles.Indicators;

namespace FancyCandles.Graphs
{
    public class TrueRangeChartElement : SubgraphChartTemplate
    {
        public TrueRangeChartElement() : base()
        {
            candlesTrueRange = new List<double>();
        }

        public List<double> CandlesTrueRange
        {
            get
            {
                if (candlesTrueRange.Count == 0 && CandlesSource != null)
                {
                    ReCalc_CandlesTrueRange();
                    SetTargetSourceForAll_OverlayIndicators();
                }
                return candlesTrueRange;
            }
        }
        private List<double> candlesTrueRange;


        //---------------------------------------------------------------------------------------------------------------------------------------
        public static readonly DependencyProperty BarFillProperty
            = DependencyProperty.Register("BarFill", typeof(Brush), typeof(TrueRangeChartElement), 
                new FrameworkPropertyMetadata(TrueRange.DefaultTrueRangeBarFill, null, CoerceBullishCandleFill) { AffectsRender = true });
        public Brush BarFill
        {
            get { return (Brush)GetValue(BarFillProperty); }
            set { SetValue(BarFillProperty, value); }
        }

        private static object CoerceBullishCandleFill(DependencyObject objWithOldDP, object newDPValue)
        {
            Brush newBrushValue = (Brush)newDPValue;

            if (newBrushValue.IsFrozen)
                return newDPValue;
            else
            {
                Brush b = (Brush)newBrushValue.GetCurrentValueAsFrozen();
                return b;
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        protected override void OnCandlesSourceChanged()
        {
            ReCalc_CandlesTrueRange();
            ReCalc_VisibleCandlesExtremums();
            SetTargetSourceForAll_OverlayIndicators();
        }

        private void ReCalc_CandlesTrueRange()
        {
            candlesTrueRange.Clear();
            if (CandlesSource == null) return;
            candlesTrueRange.Add(0); // set TR = 0 for first candle
            for (int i=1; i<CandlesSource.Count; i++)
            {
                ICandle cndl = CandlesSource[i];
                ICandle cndl_old = CandlesSource[i - 1];
                double TR = Math.Max(cndl.H, cndl_old.C) - Math.Min(cndl.L, cndl_old.C); // see https://en.wikipedia.org/wiki/Average_true_range
                candlesTrueRange.Add(TR);
            }
        }

        private void ReCalc_VisibleCandlesExtremums()
        {
            double high = double.MinValue;
            if (CandlesTrueRange.Count == 0 || CandlesSource == null) return;
            for (int i = VisibleCandlesRange.Start_i; i < VisibleCandlesRange.Start_i + VisibleCandlesRange.Count; i++)
            {
                double tr = CandlesTrueRange[i];
                high = Math.Max(high, tr);
            }
            VisibleCandlesExtremums[UpperTag] = high;
            VisibleCandlesExtremums[LowerTag] = 0;
        }

        public override void SetTargetSourceForAll_OverlayIndicators()
        {
            foreach (var indicator in Indicators)
            {
                indicator.TargetSource = CandlesTrueRange.Cast<object>().ToList();
            }
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        protected override void OnRender(DrawingContext drawingContext)
        {
            double VolumeBarWidthToCandleWidthRatio = 0.8;
            double volumeBarWidth = VolumeBarWidthToCandleWidthRatio * CandleWidthAndGap.Width;
            double volumeBarWidthNotLessThan1 = Math.Max(1.0, volumeBarWidth);
            double halfDWidth = 0.5 * (CandleWidthAndGap.Width - volumeBarWidth);
            double volumeBarGap = (1.0 - VolumeBarWidthToCandleWidthRatio) * CandleWidthAndGap.Width + CandleWidthAndGap.Gap;

            for (int i = 0; i < VisibleCandlesRange.Count; i++)
            {
                double TR = CandlesTrueRange[VisibleCandlesRange.Start_i + i];
                double barHeight = Math.Max(1.0, TR / VisibleCandlesExtremums[UpperTag] * RenderSize.Height);
                double volumeBarLeftX = halfDWidth + i * (volumeBarWidth + volumeBarGap);

                drawingContext.DrawRectangle(BarFill, null, new Rect(new Point(volumeBarLeftX, RenderSize.Height), new Vector(volumeBarWidthNotLessThan1, -barHeight)));
            }
            RenderIndicators(drawingContext);
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        protected override void OnMouseMove(MouseEventArgs e)
        {
            string decimalSeparator = Culture.NumberFormat.NumberDecimalSeparator;
            char[] decimalSeparatorArray = decimalSeparator.ToCharArray();

            Point mousePos = e.GetPosition(this);
            //Vector uv = new Vector(mousePos.X/ RenderSize.Width, mousePos.Y / RenderSize.Height);
            int cndl_i = VisibleCandlesRange.Start_i + (int)(mousePos.X / (CandleWidthAndGap.Width + CandleWidthAndGap.Gap));
            ICandle cndl = CandlesSource[cndl_i];
            string strT = cndl.t.ToString((CandlesSource.TimeFrame < 0) ? "G" : "g", Culture);
            double TR = CandlesTrueRange[cndl_i];
            string currentPriceLabelNumberFormat = $"N{MaxFractionalDigits}";
            string currentPriceString = MyNumberFormatting.PriceToString(TR, currentPriceLabelNumberFormat, Culture, decimalSeparator, decimalSeparatorArray);

            string tooltipText = $"{strT}\nTR= {currentPriceString}";
            ((ToolTip)ToolTip).Content = tooltipText;
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public string GetTrueRangeValue(int candle_id)
        {
            if (CandlesSource == null || candle_id < 1 || CandlesTrueRange.Count == 0) return "--";
            string decimalSeparator = Culture.NumberFormat.NumberDecimalSeparator;
            char[] decimalSeparatorArray = decimalSeparator.ToCharArray();
            double TR = CandlesTrueRange[candle_id];
            string currentPriceLabelNumberFormat = $"N{MaxFractionalDigits}";
            string currentPriceString = MyNumberFormatting.PriceToString(TR, currentPriceLabelNumberFormat, Culture, decimalSeparator, decimalSeparatorArray);
            return currentPriceString;
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
    }
}
