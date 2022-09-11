using System;
using System.Windows;
using System.Windows.Media;
using System.Globalization;

namespace FancyCandles.Graphs
{
    class GraphTicksElement : FrameworkElement
    {
        public static double TICK_LINE_WIDTH = 3.0;
        public static double TICK_HORIZ_MARGIN = 2.0;

        private Typeface currentTypeFace = new Typeface(SystemFonts.MessageFontFamily.ToString());
        private Pen tickPen;

        //---------------------------------------------------------------------------------------------------------------------------------------
        public CultureInfo Culture
        {
            get { return (CultureInfo)GetValue(CultureProperty); }
            set { SetValue(CultureProperty, value); }
        }
        public static readonly DependencyProperty CultureProperty =
            DependencyProperty.Register("Culture", typeof(CultureInfo), typeof(GraphTicksElement), new FrameworkPropertyMetadata(CultureInfo.CurrentCulture) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        public GraphTicksElement()
        {
            if (tickPen == null)
            {
                tickPen = new Pen(CandleChart.DefaultAxisTickColor, 1.0);
                if (!tickPen.IsFrozen)
                    tickPen.Freeze();
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        static GraphTicksElement()
        {
            Pen defaultPen = new Pen(CandleChart.DefaultHorizontalGridlinesBrush, CandleChart.DefaultHorizontalGridlinesThickness);
            defaultPen.Freeze();
            GridlinesPenProperty = DependencyProperty.Register("GridlinesPen", typeof(Pen), typeof(GraphTicksElement), 
                new FrameworkPropertyMetadata(defaultPen, null, CoerceGridlinesPen) { AffectsRender = true });
        }
        //---------------------------------------------------------------------------------------------------------------------------------------

        #region Grid line
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
            = DependencyProperty.Register("IsGridlinesEnabled", typeof(bool), typeof(GraphTicksElement), new FrameworkPropertyMetadata(true) { AffectsRender = true });

        #endregion

        #region tick label setting
        public int MaxNumberOfFractionalDigits
        {
            get { return (int)GetValue(MaxNumberOfFractionalDigitsProperty); }
            set { SetValue(MaxNumberOfFractionalDigitsProperty, value); }
        }
        public static readonly DependencyProperty MaxNumberOfFractionalDigitsProperty =
            DependencyProperty.Register("MaxNumberOfFractionalDigits", typeof(int), typeof(GraphTicksElement), new FrameworkPropertyMetadata(0));
        //---------------------------------------------------------------------------------------------------------------------------------------
        public FontFamily TickLabelFontFamily
        {
            get { return (FontFamily)GetValue(TickLabelFontFamilyProperty); }
            set { SetValue(TickLabelFontFamilyProperty, value); }
        }
        public static readonly DependencyProperty TickLabelFontFamilyProperty =
            DependencyProperty.Register("TickLabelFontFamily", typeof(FontFamily), typeof(GraphTicksElement), new FrameworkPropertyMetadata(SystemFonts.MessageFontFamily, OnTickLabelFontFamilyChanged));

        static void OnTickLabelFontFamilyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            GraphTicksElement thisElement = obj as GraphTicksElement;
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
            = DependencyProperty.Register("GapBetweenTickLabels", typeof(double), typeof(GraphTicksElement), new FrameworkPropertyMetadata(0.0) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        public double TickLabelFontSize
        {
            get { return (double)GetValue(TickLabelFontSizeProperty); }
            set { SetValue(TickLabelFontSizeProperty, value); }
        }
        public static readonly DependencyProperty TickLabelFontSizeProperty
            = DependencyProperty.Register("TickLabelFontSize", typeof(double), typeof(GraphTicksElement), new FrameworkPropertyMetadata(9.0) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        public Brush TickColor
        {
            get { return (Brush)GetValue(TickColorProperty); }
            set { SetValue(TickColorProperty, value); }
        }
        public static readonly DependencyProperty TickColorProperty
            = DependencyProperty.Register("TickColor", typeof(Brush), typeof(GraphTicksElement),
                new FrameworkPropertyMetadata(CandleChart.DefaultAxisTickColor, null, CoerceTickColor) { AffectsRender = true });

        private static object CoerceTickColor(DependencyObject objWithOldDP, object newDPValue)
        {
            GraphTicksElement thisElement = (GraphTicksElement)objWithOldDP;
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

        #region chart margin
        public double ChartBottomMargin
        {
            get { return (double)GetValue(ChartBottomMarginProperty); }
            set { SetValue(ChartBottomMarginProperty, value); }
        }
        public static readonly DependencyProperty ChartBottomMarginProperty
            = DependencyProperty.Register("ChartBottomMargin", typeof(double), typeof(GraphTicksElement), new FrameworkPropertyMetadata(15.0) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        public double ChartTopMargin
        {
            get { return (double)GetValue(ChartTopMarginProperty); }
            set { SetValue(ChartTopMarginProperty, value); }
        }
        public static readonly DependencyProperty ChartTopMarginProperty
            = DependencyProperty.Register("ChartTopMargin", typeof(double), typeof(GraphTicksElement), new FrameworkPropertyMetadata(15.0) { AffectsRender = true });

        #endregion
        //---------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 放 Label 的地方要多寬 (所有圖都 bind PriceChart 的 AxisWidth, 讓右邊一樣寬)
        /// </summary>
        public double AxisWidth
        {
            get { return (double)GetValue(PricePanelWidthProperty); }
            set { SetValue(PricePanelWidthProperty, value); }
        }
        public static readonly DependencyProperty PricePanelWidthProperty
            = DependencyProperty.Register("AxisWidth", typeof(double), typeof(GraphTicksElement), new FrameworkPropertyMetadata(0.0) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------
        public CandleExtremums VisibleCandlesExtremums
        {
            get { return (CandleExtremums)GetValue(VisibleCandlesExtremumsProperty); }
            set { SetValue(VisibleCandlesExtremumsProperty, value); }
        }
        public static readonly DependencyProperty VisibleCandlesExtremumsProperty
            = DependencyProperty.Register("VisibleCandlesExtremums", typeof(CandleExtremums), typeof(GraphTicksElement), new FrameworkPropertyMetadata(new CandleExtremums(1.0, 1.0, 0L, 0L)) { AffectsRender = true });
        //---------------------------------------------------------------------------------------------------------------------------------------

        protected override void OnRender(DrawingContext drawingContext)
        {
            double textHeight = (new FormattedText("123", Culture, FlowDirection.LeftToRight, currentTypeFace, TickLabelFontSize, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip)).Height;
            double halfTextHeight = textHeight / 2.0;
            double chartPanelWidth = ActualWidth - AxisWidth;
            double tickLabelX = chartPanelWidth + TICK_LINE_WIDTH + TICK_HORIZ_MARGIN;
            double tickLineEndX = chartPanelWidth + TICK_LINE_WIDTH;
            double chartHeight = ActualHeight - ChartBottomMargin - ChartTopMargin;

            double stepInRubles = (VisibleCandlesExtremums.PriceHigh - VisibleCandlesExtremums.PriceLow) / chartHeight * (textHeight + GapBetweenTickLabels);
            double stepInRubles_HPlace = MyWpfMath.HighestDecimalPlace(stepInRubles, out int stepInRubles_HPow);
            stepInRubles = Math.Ceiling(stepInRubles / stepInRubles_HPlace) * stepInRubles_HPlace;
            MyWpfMath.HighestDecimalPlace(stepInRubles, out int stepInRublesHighestDecimalPow);
            string priceTickLabelNumberFormat = (stepInRubles_HPow >= 0) ? "N0" : $"N{-stepInRubles_HPow}";

            double chartHeight_candlesLHRange_Ratio = chartHeight / (VisibleCandlesExtremums.PriceHigh - VisibleCandlesExtremums.PriceLow);

            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            string decimalSeparator = Culture.NumberFormat.NumberDecimalSeparator;
            char[] decimalSeparatorArray = decimalSeparator.ToCharArray();

            void DrawPriceTickLabel(double price, int priceStepHighestDecimalPow)
            {
                string s = MyNumberFormatting.PriceToString(price, priceTickLabelNumberFormat, Culture, decimalSeparator, decimalSeparatorArray);
                FormattedText priceTickFormattedText = new FormattedText(s, Culture, FlowDirection.LeftToRight, currentTypeFace, TickLabelFontSize, TickColor, VisualTreeHelper.GetDpi(this).PixelsPerDip);
                double y = ChartTopMargin + (VisibleCandlesExtremums.PriceHigh - price) * chartHeight_candlesLHRange_Ratio;
                drawingContext.DrawText(priceTickFormattedText, new Point(tickLabelX, y - halfTextHeight));
                drawingContext.DrawLine(tickPen, new Point(chartPanelWidth, y), new Point(tickLineEndX, y));

                if (IsGridlinesEnabled && GridlinesPen != null)
                    drawingContext.DrawLine(GridlinesPen, new Point(0, y), new Point(chartPanelWidth, y));
            }
            
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            double theMostRoundPrice = MyWpfMath.TheMostRoundValueInsideRange(VisibleCandlesExtremums.PriceLow, VisibleCandlesExtremums.PriceHigh);
            DrawPriceTickLabel(theMostRoundPrice, stepInRublesHighestDecimalPow);

            double maxPriceThreshold = VisibleCandlesExtremums.PriceHigh + (ChartTopMargin - halfTextHeight) / chartHeight_candlesLHRange_Ratio;
            double minPriceThreshold = VisibleCandlesExtremums.PriceHigh + (ChartTopMargin - ActualHeight + halfTextHeight) / chartHeight_candlesLHRange_Ratio;

            int step_i = 1;
            double next_tick;
            while ((next_tick = theMostRoundPrice + step_i * stepInRubles) < maxPriceThreshold)
            {
                DrawPriceTickLabel(next_tick, stepInRublesHighestDecimalPow);
                step_i++;
            }

            step_i = 1;
            while ((next_tick = theMostRoundPrice - step_i * stepInRubles) > minPriceThreshold)
            {
                DrawPriceTickLabel(next_tick, stepInRublesHighestDecimalPow);
                step_i++;
            }
        }

    }
}
