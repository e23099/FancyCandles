﻿using System;
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
    public class TrueRangeChartElement : FrameworkElement
    {
        public TrueRangeChartElement()
        {
            ToolTip tt = new ToolTip() { FontSize = CandleChart.ToolTipFontSize, BorderBrush = Brushes.Beige };
            tt.Content = "";
            ToolTip = tt;

            // We set the delay time for the appearance of hints here, and the location of the hints (if it needs to be changed) is set in XAML:
            ToolTipService.SetShowDuration(this, int.MaxValue);
            ToolTipService.SetInitialShowDelay(this, 0);

            candlesTrueRange = new List<double>();

        }

        public List<double> CandlesTrueRange
        {
            get
            {
                if (candlesTrueRange.Count == 0 && CandlesSource != null)
                {
                    ReCalc_CandlesTrueRange();
                    SetAll_OverlayIndicators();
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
        
        public ICandlesSource CandlesSource
        {
            get { return (ICandlesSource)GetValue(CandlesSourceProperty); }
            set { SetValue(CandlesSourceProperty, value); }
        }
        public static readonly DependencyProperty CandlesSourceProperty
             = DependencyProperty.Register("CandlesSource", typeof(ICandlesSource), typeof(TrueRangeChartElement), 
                 new UIPropertyMetadata(null, OnCandlesSourceChanged));
        internal static void OnCandlesSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            TrueRangeChartElement thisChart = obj as TrueRangeChartElement;
            if (thisChart == null) return;
            if (thisChart.IsLoaded)
            {
                thisChart.ReCalc_CandlesTrueRange();
                thisChart.ReCalc_VisibleCandlesExtremums();
                thisChart.SetAll_OverlayIndicators();
            }
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
            if (CandlesTrueRange.Count == 0) return;
            for (int i = VisibleCandlesRange.Start_i; i < VisibleCandlesRange.Start_i + VisibleCandlesRange.Count; i++)
            {
                double tr = CandlesTrueRange[i];
                high = Math.Max(high, tr);
            }
            VisibleCandlesExtremums[UpperTag] = high;
            VisibleCandlesExtremums[LowerTag] = 0;
        }

        private void SetAll_OverlayIndicators()
        {
            foreach (var indicator in Indicators)
            {
                indicator.TargetSource = CandlesTrueRange.Cast<object>().ToList();
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public string UpperTag
        {
            get { return ((string)GetValue(UpperTagProperty)).ToString(); }
            set { SetValue(UpperTagProperty, value); }
        }
        public static readonly DependencyProperty UpperTagProperty
            = DependencyProperty.Register("UpperTag", typeof(string), typeof(TrueRangeChartElement), new FrameworkPropertyMetadata(null));
        //---------------------------------------------------------------------------------------------------------------------------------------
        public string LowerTag
        {
            get { return ((string)GetValue(LowerTagProperty)).ToString(); }
            set { SetValue(LowerTagProperty, value); }
        }
        public static readonly DependencyProperty LowerTagProperty
            = DependencyProperty.Register("LowerTag", typeof(string), typeof(TrueRangeChartElement), new FrameworkPropertyMetadata(null));

        //---------------------------------------------------------------------------------------------------------------------------------------
        #region Indicators
        public static readonly DependencyProperty IndicatorsProperty
            = DependencyProperty.Register("Indicators", typeof(ObservableCollection<OverlayIndicator>), typeof(TrueRangeChartElement), 
                new FrameworkPropertyMetadata(null, OnIndicatorsChanged) { AffectsRender = true });
        public ObservableCollection<OverlayIndicator> Indicators
        {
            get { return (ObservableCollection<OverlayIndicator>)GetValue(IndicatorsProperty); }
            set { SetValue(IndicatorsProperty, value); }
        }

        static void OnIndicatorsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            TrueRangeChartElement thisTrueRangeChartElement = obj as TrueRangeChartElement;
            if (thisTrueRangeChartElement == null) return;

            ObservableCollection<OverlayIndicator> old_obsCollection = e.OldValue as ObservableCollection<OverlayIndicator>;
            if (old_obsCollection != null)
            {
                old_obsCollection.CollectionChanged -= thisTrueRangeChartElement.OnIndicatorsCollectionChanged;

                foreach (OverlayIndicator indicator in old_obsCollection)
                    indicator.PropertyChanged -= thisTrueRangeChartElement.OnIndicatorsCollectionItemChanged;
            }

            ObservableCollection<OverlayIndicator> new_obsCollection = e.NewValue as ObservableCollection<OverlayIndicator>;
            if (new_obsCollection != null)
            {
                new_obsCollection.CollectionChanged += thisTrueRangeChartElement.OnIndicatorsCollectionChanged;

                foreach (OverlayIndicator indicator in new_obsCollection)
                    indicator.PropertyChanged += thisTrueRangeChartElement.OnIndicatorsCollectionItemChanged;
            }
        }

        private void OnIndicatorsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (OverlayIndicator indicator in e.NewItems)
                    indicator.PropertyChanged += OnIndicatorsCollectionItemChanged;
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (OverlayIndicator indicator in e.NewItems)
                    indicator.PropertyChanged += OnIndicatorsCollectionItemChanged;

                foreach (OverlayIndicator indicator in e.OldItems)
                    indicator.PropertyChanged -= OnIndicatorsCollectionItemChanged;
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (OverlayIndicator indicator in e.OldItems)
                    indicator.PropertyChanged -= OnIndicatorsCollectionItemChanged;
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (OverlayIndicator indicator in (sender as IEnumerable<OverlayIndicator>))
                    indicator.PropertyChanged += OnIndicatorsCollectionItemChanged;
            }
            else if (e.Action == NotifyCollectionChangedAction.Move) {}

            InvalidateVisual();
        }

        private void OnIndicatorsCollectionItemChanged(object source, PropertyChangedEventArgs args)
        {
            InvalidateVisual();
        }
        #endregion
        //---------------------------------------------------------------------------------------------------------------------------------------
        #region TargetChart Properties
        public CultureInfo Culture
        {
            get { return (CultureInfo)GetValue(CultureProperty); }
            set { SetValue(CultureProperty, value); }
        }
        public static readonly DependencyProperty CultureProperty =
            DependencyProperty.Register("Culture", typeof(CultureInfo), typeof(TrueRangeChartElement), new FrameworkPropertyMetadata(CultureInfo.CurrentCulture) { AffectsRender = true });

        //---------------------------------------------------------------------------------------------------------------------------------------
        public static readonly DependencyProperty VisibleCandlesExtremumsProperty
            = DependencyProperty.Register("VisibleCandlesExtremums", typeof(Dictionary<string,double>),
                typeof(TrueRangeChartElement), new FrameworkPropertyMetadata(null) { AffectsRender = true });
        public Dictionary<string,double> VisibleCandlesExtremums
        {
            get { return (Dictionary<string,double>)GetValue(VisibleCandlesExtremumsProperty); }
            set { SetValue(VisibleCandlesExtremumsProperty, value); }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public static readonly DependencyProperty VisibleCandlesRangeProperty
             = DependencyProperty.Register("VisibleCandlesRange", typeof(IntRange), typeof(TrueRangeChartElement), new FrameworkPropertyMetadata(IntRange.Undefined));
        public IntRange VisibleCandlesRange
        {
            get { return (IntRange)GetValue(VisibleCandlesRangeProperty); }
            set { SetValue(VisibleCandlesRangeProperty, value); }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public static readonly DependencyProperty CandleWidthAndGapProperty
             = DependencyProperty.Register("CandleWidthAndGap", typeof(CandleDrawingParameters), typeof(TrueRangeChartElement),
                 new FrameworkPropertyMetadata(new CandleDrawingParameters()));
        public CandleDrawingParameters CandleWidthAndGap
        {
            get { return (CandleDrawingParameters)GetValue(CandleWidthAndGapProperty); }
            set { SetValue(CandleWidthAndGapProperty, value); }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public int MaxNumberOfFractionalDigitsInPrice
        {
            get { return (int)GetValue(MaxNumberOfFractionalDigitsInPriceProperty); }
            set { SetValue(MaxNumberOfFractionalDigitsInPriceProperty, value); }
        }
        public static readonly DependencyProperty MaxNumberOfFractionalDigitsInPriceProperty =
            DependencyProperty.Register("MaxNumberOfFractionalDigitsInPrice", typeof(int), typeof(TrueRangeChartElement), new FrameworkPropertyMetadata(0));
        #endregion
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
            if (Indicators == null) return; 
            for (int i = 0; i < Indicators.Count ; i++)
                Indicators[i].OnRender(drawingContext, VisibleCandlesRange, VisibleCandlesExtremums, CandleWidthAndGap.Width, CandleWidthAndGap.Gap, RenderSize.Height);
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
            string currentPriceLabelNumberFormat = $"N{MaxNumberOfFractionalDigitsInPrice}";
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
            string currentPriceLabelNumberFormat = $"N{MaxNumberOfFractionalDigitsInPrice}";
            string currentPriceString = MyNumberFormatting.PriceToString(TR, currentPriceLabelNumberFormat, Culture, decimalSeparator, decimalSeparatorArray);
            return currentPriceString;
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
    }
}
