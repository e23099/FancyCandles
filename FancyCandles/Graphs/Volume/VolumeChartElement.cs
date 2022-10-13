/* 
    Copyright 2020 Dennis Geller.

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

using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.ComponentModel;
using System.Runtime.CompilerServices; // [CallerMemberName]
using System.Diagnostics;
using FancyCandles.Indicators;

namespace FancyCandles.Graphs
{
    class VolumeChartElement : SubgraphChartTemplate
    {
        public VolumeChartElement() : base()
        {
            if (bearishBarPen == null)
            {
                bearishBarPen = new Pen(Volume.DefaultBearishVolumeBarFill, 1);
                if (!bearishBarPen.IsFrozen)
                    bearishBarPen.Freeze();
            }
        }

        #region properties
        public Brush BullishBarFill
        {
            get { return (Brush)GetValue(BullishBarFillProperty); }
            set { SetValue(BullishBarFillProperty, value); }
        }
        public static readonly DependencyProperty BullishBarFillProperty
            = DependencyProperty.Register("BullishBarFill", typeof(Brush), typeof(VolumeChartElement), 
                new FrameworkPropertyMetadata(Volume.DefaultBullishVolumeBarFill, null, CoerceBullishCandleFill) { AffectsRender = true });

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
        private Pen bearishBarPen;

        public Brush BearishBarFill
        {
            get { return (Brush)GetValue(BearishBarFillProperty); }
            set { SetValue(BearishBarFillProperty, value); }
        }
        public static readonly DependencyProperty BearishBarFillProperty
            = DependencyProperty.Register("BearishBarFill", typeof(Brush), typeof(VolumeChartElement), 
                new FrameworkPropertyMetadata(Volume.DefaultBearishVolumeBarFill, null, CoerceBearishCandleFill) { AffectsRender = true });

        private static object CoerceBearishCandleFill(DependencyObject objWithOldDP, object newDPValue)
        {
            VolumeChartElement thisElement = (VolumeChartElement)objWithOldDP;
            Brush newBrushValue = (Brush)newDPValue;

            if (newBrushValue.IsFrozen)
            {
                Pen p = new Pen(newBrushValue, 1.0);
                p.Freeze();
                thisElement.bearishBarPen = p;
                return newDPValue;
            }
            else
            {
                Brush b = (Brush)newBrushValue.GetCurrentValueAsFrozen();
                Pen p = new Pen(b, 1.0);
                p.Freeze();
                thisElement.bearishBarPen = p;
                return b;
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public double VolumeBarWidthToCandleWidthRatio
        {
            get { return (double)GetValue(VolumeBarWidthToCandleWidthRatioProperty); }
            set { SetValue(VolumeBarWidthToCandleWidthRatioProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VolumeBarWidthToCandleWidthRatio.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VolumeBarWidthToCandleWidthRatioProperty =
            DependencyProperty.Register("VolumeBarWidthToCandleWidthRatio", typeof(double), typeof(VolumeChartElement), 
                new FrameworkPropertyMetadata(1.0) { AffectsRender = true });
        #endregion


        protected override void OnCandlesSourceChanged()
        {
            SetTargetSourceForAll_OverlayIndicators();
        }

        public override void SetTargetSourceForAll_OverlayIndicators()
        {
            foreach (var indicator in Indicators)
            {
                indicator.TargetSource = CandlesSource?.ToList<object>();
            }
        }


        protected override void OnRender(DrawingContext drawingContext)
        {
            double volumeBarWidth = VolumeBarWidthToCandleWidthRatio * CandleWidthAndGap.Width;
            double volumeBarWidthNotLessThan1 = Math.Max(1.0, volumeBarWidth);
            double halfDWidth = 0.5 * (CandleWidthAndGap.Width - volumeBarWidth);
            double volumeBarGap = (1.0 - VolumeBarWidthToCandleWidthRatio) * CandleWidthAndGap.Width + CandleWidthAndGap.Gap;
            Console.WriteLine($"volume onRender vcRange {VisibleCandlesRange.Start_i} ({VisibleCandlesRange.Count}) {CandlesSource?.Count ?? 0}");

            for (int i = 0; i < VisibleCandlesRange.Count; i++)
            {
                if (VisibleCandlesRange.Start_i + i >= CandlesSource.Count) continue;
                ICandle cndl = CandlesSource[VisibleCandlesRange.Start_i + i];
                Brush cndlBrush = (cndl.C > cndl.O) ? BullishBarFill : BearishBarFill;

                double barHeight = Math.Max(1.0, cndl.V / VisibleCandlesExtremums[UpperTag] * RenderSize.Height);
                double volumeBarLeftX = halfDWidth + i * (volumeBarWidth + volumeBarGap);

                drawingContext.DrawRectangle(cndlBrush, null, new Rect(new Point(volumeBarLeftX, RenderSize.Height), new Vector(volumeBarWidthNotLessThan1, -barHeight)));
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
            string tooltipText = $"{strT}\nV= {MyNumberFormatting.VolumeToString(cndl.V, Culture, decimalSeparator, decimalSeparatorArray)}";
            ((ToolTip)ToolTip).Content = tooltipText;
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        public string GetVolumeValue(int candle_id)
        {
            if (CandlesSource == null) return "--";
            string decimalSeparator = Culture.NumberFormat.NumberDecimalSeparator;
            char[] decimalSeparatorArray = decimalSeparator.ToCharArray();
            ICandle cndl = CandlesSource[candle_id];
            return MyNumberFormatting.VolumeToString(cndl.V, Culture, decimalSeparator, decimalSeparatorArray);
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------
    }
}
