using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;

namespace FancyCandles.Graphs
{
    public abstract class TickElementTemplate : FrameworkElement
    {
        /// <summary>
        /// Y axis scale mark's length
        /// </summary>
        public static double TICK_LINE_WIDTH = 3.0;

        /// <summary>
        /// length of the gap between scale mark and it's label
        /// </summary>
        public static double TICK_LEFT_MARGIN = 2.0;

        private Typeface currentTypeFace = new Typeface(SystemFonts.MessageFontFamily.ToString());
        private Pen tickPen;
        //---------------------------------------------------------------------------------------------------------------------------------------
        static TickElementTemplate()
        {
            Pen defaultPen = new Pen(CandleChart.DefaultHorizontalGridlinesBrush, CandleChart.DefaultHorizontalGridlinesThickness);
            defaultPen.Freeze();
            GridlinesPenProperty = DependencyProperty.Register("GridlinesPen", typeof(Pen), typeof(TickElementTemplate),
                new FrameworkPropertyMetadata(defaultPen, null, CoerceGridlinesPen) { AffectsRender = true });
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public TickElementTemplate()
        {
            if (tickPen == null)
            {
                tickPen = new Pen(CandleChart.DefaultAxisTickColor, 1.0);
                if (!tickPen.IsFrozen)
                    tickPen.Freeze();
            }
        }
        #region common properties
        public double PriceAxisWidth
        {
            get { return (double)GetValue(PricePanelWidthProperty); }
            set { SetValue(PricePanelWidthProperty, value); }
        }
        public static readonly DependencyProperty PricePanelWidthProperty 
            = DependencyProperty.Register("PriceAxisWidth", typeof(double), typeof(TickElementTemplate), new FrameworkPropertyMetadata(0.0) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        public string UpperTag
        {
            get { return ((string)GetValue(UpperTagProperty)).ToString(); }
            set { SetValue(UpperTagProperty, value); }
        }
        public static readonly DependencyProperty UpperTagProperty
            = DependencyProperty.Register("UpperTag", typeof(string), typeof(TickElementTemplate), new FrameworkPropertyMetadata(""));

        protected double Upper
        {
            get
            {
                if (string.IsNullOrEmpty(UpperTag)) return 0;
                if (!VisibleCandlesExtremums.ContainsKey(UpperTag))
                {
                    VisibleCandlesExtremums[UpperTag] = 0;
                    return 0;
                }
                return VisibleCandlesExtremums[UpperTag];
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public string LowerTag
        {
            get { return ((string)GetValue(LowerTagProperty)).ToString(); }
            set { SetValue(LowerTagProperty, value); }
        }
        public static readonly DependencyProperty LowerTagProperty
            = DependencyProperty.Register("LowerTag", typeof(string), typeof(TickElementTemplate), new FrameworkPropertyMetadata(""));
        protected double Lower
        {
            get
            {
                if (string.IsNullOrEmpty(LowerTag)) return 0;
                if (!VisibleCandlesExtremums.ContainsKey(LowerTag))
                {
                    VisibleCandlesExtremums[LowerTag] = 0;
                    return 0;
                }
                return VisibleCandlesExtremums[LowerTag];
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public Dictionary<string,double> VisibleCandlesExtremums
        {
            get { return (Dictionary<string,double>)GetValue(VisibleCandlesExtremumsProperty); }
            set { SetValue(VisibleCandlesExtremumsProperty, value); }
        }
        public static readonly DependencyProperty VisibleCandlesExtremumsProperty
            = DependencyProperty.Register("VisibleCandlesExtremums", typeof(Dictionary<string, double>),
                typeof(TickElementTemplate), new FrameworkPropertyMetadata(null) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        public CultureInfo Culture
        {
            get { return (CultureInfo)GetValue(CultureProperty); }
            set { SetValue(CultureProperty, value); }
        }
        public static readonly DependencyProperty CultureProperty =
            DependencyProperty.Register("Culture", typeof(CultureInfo), typeof(TickElementTemplate), new FrameworkPropertyMetadata(CultureInfo.CurrentCulture) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        public Pen GridlinesPen
        {
            get { return (Pen)GetValue(GridlinesPenProperty); }
            set { SetValue(GridlinesPenProperty, value); }
        }
        public static readonly DependencyProperty GridlinesPenProperty;

        private static object CoerceGridlinesPen(DependencyObject objWithOldDP, object newDPValue)
        {
            Pen newPenValue = (Pen)newDPValue;
            return newPenValue.IsFrozen ? newDPValue : newPenValue.GetCurrentValueAsFrozen();
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public bool IsGridlinesEnabled
        {
            get { return (bool)GetValue(IsGridlinesEnabledProperty); }
            set { SetValue(IsGridlinesEnabledProperty, value); }
        }
        public static readonly DependencyProperty IsGridlinesEnabledProperty
            = DependencyProperty.Register("IsGridlinesEnabled", typeof(bool), typeof(TickElementTemplate), new FrameworkPropertyMetadata(true) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------

        public FontFamily TickLabelFontFamily
        {
            get { return (FontFamily)GetValue(TickLabelFontFamilyProperty); }
            set { SetValue(TickLabelFontFamilyProperty, value); }
        }
        public static readonly DependencyProperty TickLabelFontFamilyProperty =
            DependencyProperty.Register("TickLabelFontFamily", typeof(FontFamily), typeof(TickElementTemplate), new FrameworkPropertyMetadata(SystemFonts.MessageFontFamily, OnTickLabelFontFamilyChanged));

        static void OnTickLabelFontFamilyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            TickElementTemplate thisElement = obj as TickElementTemplate;
            if (thisElement == null) return;
            thisElement.currentTypeFace = new Typeface(thisElement.TickLabelFontFamily.ToString());
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public double GapBetweenTickLabels
        {
            get { return (double)GetValue(GapBetweenTickLabelsProperty); }
            set { SetValue(GapBetweenTickLabelsProperty, value); }
        }
        public static readonly DependencyProperty GapBetweenTickLabelsProperty
            = DependencyProperty.Register("GapBetweenTickLabels", typeof(double), typeof(TickElementTemplate), new FrameworkPropertyMetadata(0.0) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        public double ChartBottomMargin
        {
            get { return (double)GetValue(ChartBottomMarginProperty); }
            set { SetValue(ChartBottomMarginProperty, value); }
        }
        public static readonly DependencyProperty ChartBottomMarginProperty
             = DependencyProperty.Register("ChartBottomMargin", typeof(double), typeof(TickElementTemplate), new FrameworkPropertyMetadata(15.0) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        public double ChartTopMargin
        {
            get { return (double)GetValue(ChartTopMarginProperty); }
            set { SetValue(ChartTopMarginProperty, value); }
        }
        public static readonly DependencyProperty ChartTopMarginProperty
            = DependencyProperty.Register("ChartTopMargin", typeof(double), typeof(TickElementTemplate), new FrameworkPropertyMetadata(15.0) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        public double TickLabelFontSize
        {
            get { return (double)GetValue(TickLabelFontSizeProperty); }
            set { SetValue(TickLabelFontSizeProperty, value); }
        }
        public static readonly DependencyProperty TickLabelFontSizeProperty
            = DependencyProperty.Register("TickLabelFontSize", typeof(double), typeof(TickElementTemplate), new FrameworkPropertyMetadata(9.0) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------

        public Brush TickColor
        {
            get { return (Brush)GetValue(TickColorProperty); }
            set { SetValue(TickColorProperty, value); }
        }
        public static readonly DependencyProperty TickColorProperty
            = DependencyProperty.Register("TickColor", typeof(Brush), typeof(TickElementTemplate),
                new FrameworkPropertyMetadata(CandleChart.DefaultAxisTickColor, null, CoerceTickColor) { AffectsRender = true });

        private static object CoerceTickColor(DependencyObject objWithOldDP, object newDPValue)
        {
            TickElementTemplate thisElement = (TickElementTemplate)objWithOldDP;
            Brush newBrushValue = (Brush)newDPValue;

            if (newBrushValue.IsFrozen)
            {
                Pen p = new Pen(newBrushValue, 1.0);
                p.Freeze();
                thisElement.tickPen = p;
                return newDPValue;
            }
            else
            {
                Brush b = (Brush)newBrushValue.GetCurrentValueAsFrozen();
                Pen p = new Pen(b, 1.0);
                p.Freeze();
                thisElement.tickPen = p;
                return b;
            }
        }
        #endregion

        #region Current Value properties
        public double CurrentValue
        {
            get { return (double)GetValue(CurrentValueProperty); }
            set { SetValue(CurrentValueProperty, value); }
        }
        public static readonly DependencyProperty CurrentValueProperty =
            DependencyProperty.Register("CurrentValue", typeof(double), typeof(TickElementTemplate), new FrameworkPropertyMetadata(0.0) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        public bool IsCurrentValueLabelVisible
        {
            get { return (bool)GetValue(IsCurrentValueLabelVisibleProperty); }
            set { SetValue(IsCurrentValueLabelVisibleProperty, value); }
        }
        public static readonly DependencyProperty IsCurrentValueLabelVisibleProperty =
            DependencyProperty.Register("IsCurrentValueLabelVisible", typeof(bool), typeof(TickElementTemplate), new FrameworkPropertyMetadata(false) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        private Pen currentValueLabelForegroundPen;

        public Brush CurrentValueLabelForeground
        {
            get { return (Brush)GetValue(CurrentValueLabelForegroundProperty); }
            set { SetValue(CurrentValueLabelForegroundProperty, value); }
        }
        public static readonly DependencyProperty CurrentValueLabelForegroundProperty =
            DependencyProperty.Register("CurrentValueLabelForeground", typeof(Brush), typeof(TickElementTemplate), 
                new FrameworkPropertyMetadata(CandleChart.DefaultCurrentPriceLabelForeground, null, CoerceCurrentValueLabelForeground) { AffectsRender = true });

        private static object CoerceCurrentValueLabelForeground(DependencyObject objWithOldDP, object newDPValue)
        {
            TickElementTemplate thisElement = (TickElementTemplate)objWithOldDP;
            Brush newBrushValue = (Brush)newDPValue;

            if (newBrushValue.IsFrozen)
            {
                Pen p = new Pen(newBrushValue, 1.0);
                p.Freeze();
                thisElement.currentValueLabelForegroundPen = p;
                return newDPValue;
            }
            else
            {
                Brush b = (Brush)newBrushValue.GetCurrentValueAsFrozen();
                Pen p = new Pen(b, 1.0);
                p.Freeze();
                thisElement.currentValueLabelForegroundPen = p;
                return b;
            }
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        public Brush CurrentValueLabelBackground
        {
            get { return (Brush)GetValue(CurrentValueLabelBackgroundProperty); }
            set { SetValue(CurrentValueLabelBackgroundProperty, value); }
        }
        public static readonly DependencyProperty CurrentValueLabelBackgroundProperty =
            DependencyProperty.Register("CurrentValueLabelBackground", typeof(Brush), typeof(TickElementTemplate), 
                new FrameworkPropertyMetadata(CandleChart.DefaultCurrentPriceLabelBackground, null, CoerceCurrentValueLabelBackground) { AffectsRender = true });

        private static object CoerceCurrentValueLabelBackground(DependencyObject objWithOldDP, object newDPValue)
        {
            TickElementTemplate thisElement = (TickElementTemplate)objWithOldDP;
            Brush newBrushValue = (Brush)newDPValue;

            if (newBrushValue.IsFrozen)
                return newDPValue;
            else
                return (Brush)newBrushValue.GetCurrentValueAsFrozen();
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        #endregion


        protected override void OnRender(DrawingContext drawingContext)
        {
            if (VisibleCandlesExtremums == null) return;
            
            double textHeight = (new FormattedText("123", Culture, FlowDirection.LeftToRight, currentTypeFace, TickLabelFontSize, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip)).Height;
            double halfTextHeight = textHeight / 2.0;
            double chartPanelWidth = ActualWidth - PriceAxisWidth;
            double tickLabelX = chartPanelWidth + TICK_LINE_WIDTH + TICK_LEFT_MARGIN; // Y 座標文字起始位置
            double tickLineEndX = chartPanelWidth + TICK_LINE_WIDTH; // Y 座標格線終點位置
            double chartHeight = ActualHeight - ChartBottomMargin - ChartTopMargin;
            if (chartHeight <= 0) return;

            // 子圖 Y 座標每一格相當於子圖數值的多少
            double stepInVolumeUnits = (Upper - Lower) * ((textHeight + GapBetweenTickLabels) / chartHeight);
            // stepInVolumeUnits 的最大位數 (ex: 34 = 10 位數; 123 = 100 位數)
            double stepInVolumeUnits_HPlace = MyWpfMath.HighestDecimalPlace(stepInVolumeUnits, out _);
            // 把每一格化整成「整數」格，比如 HPlace 是 10，一格 34，那就把一格改成 40
            stepInVolumeUnits = Math.Ceiling(stepInVolumeUnits / stepInVolumeUnits_HPlace) * stepInVolumeUnits_HPlace;
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            // 一單位子圖數值，實際代表 Y 座標多長的距離
            double chartHeight_candlesLHRange_Ratio = chartHeight / (Upper - Lower);


            void DrawTickLabel(double volume)
            {
                string s = ToLabelString(volume);
                FormattedText priceTickFormattedText = new FormattedText(s, Culture, FlowDirection.LeftToRight, currentTypeFace, TickLabelFontSize, TickColor, VisualTreeHelper.GetDpi(this).PixelsPerDip);
                double y = ChartTopMargin + (Upper - volume) * chartHeight_candlesLHRange_Ratio;
                drawingContext.DrawText(priceTickFormattedText, new Point(tickLabelX, y - halfTextHeight)); // label 文字
                drawingContext.DrawLine(tickPen, new Point(chartPanelWidth, y), new Point(tickLineEndX, y)); // label 前面的橫線刻度

                if (IsGridlinesEnabled && GridlinesPen != null)
                    drawingContext.DrawLine(GridlinesPen, new Point(0, y), new Point(chartPanelWidth, y)); // 格線
            }

            void DrawCurrentTickLabel()
            {
                string s = ToLabelString(CurrentValue);
                FormattedText formattedText = new FormattedText(s, Culture, FlowDirection.LeftToRight, currentTypeFace, TickLabelFontSize, CurrentValueLabelForeground, VisualTreeHelper.GetDpi(this).PixelsPerDip);
                double formattedTextWidth = formattedText.Width;
                double y = ChartTopMargin + (VisibleCandlesExtremums[UpperTag] - CurrentValue) * chartHeight_candlesLHRange_Ratio;
                drawingContext.DrawRectangle(CurrentValueLabelBackground, currentValueLabelForegroundPen, 
                                             new Rect(chartPanelWidth, y - halfTextHeight, formattedTextWidth + TICK_LINE_WIDTH + 2 * TICK_LEFT_MARGIN, textHeight + 1.0));
                drawingContext.DrawLine(currentValueLabelForegroundPen, new Point(chartPanelWidth, y), new Point(tickLineEndX, y));
                drawingContext.DrawText(formattedText, new Point(tickLabelX, y - halfTextHeight));
            }
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            double theMostRoundVolume = GetMostRoundValue(VisibleCandlesExtremums);
            DrawTickLabel(theMostRoundVolume);

            double maxVolumeThreshold = (Upper + (ChartTopMargin - halfTextHeight) / chartHeight_candlesLHRange_Ratio);
            double minVolumeThreshold = (Upper + (ChartTopMargin - ActualHeight + halfTextHeight) / chartHeight_candlesLHRange_Ratio);

            int step_i = 1;
            double next_tick;
            while ((next_tick = theMostRoundVolume + step_i * stepInVolumeUnits) < maxVolumeThreshold)
            {
                DrawTickLabel(next_tick);
                step_i++;
            }

            step_i = 1;
            while ((next_tick = theMostRoundVolume - step_i * stepInVolumeUnits) > minVolumeThreshold)
            {
                DrawTickLabel(next_tick);
                step_i++;
            }

            if (IsCurrentValueLabelVisible)
                DrawCurrentTickLabel();
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Turn 'value' into a string label shown in the Y axis.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract string ToLabelString(double value);


        /// <summary>
        /// Get the anchor value from VisibleCandlesExtremums so that ticks can be drawn accordingly.
        /// </summary>
        /// <param name="visibleCandlesExtremums"></param>
        /// <returns></returns>
        public abstract double GetMostRoundValue(Dictionary<string, double> visibleCandlesExtremums);
    }

}
