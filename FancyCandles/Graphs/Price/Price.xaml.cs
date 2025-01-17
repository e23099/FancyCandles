﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FancyCandles.Graphs
{
    /// <summary>
    /// Interaction logic for Price.xaml
    /// </summary>
    public partial class Price : Subgraph
    {
        private static int instance_count = 0;

        public Price()
        {
            InitializeComponent();
            instance_count++;
            UpperTag = $"Price{instance_count}H";
            LowerTag = $"Price{instance_count}L";
        }


        public override void UpdateVisibleCandlesExtremums(ICandlesSource candles, int start, int length, Dictionary<string,double> vcExetremums)
        {
            double upper = double.MinValue, lower = double.MaxValue;
            for (int i = start; i < start + length; i++)
            {
                ICandle candle = candles[i];
                upper = Math.Max(upper, candle.H);
                lower = Math.Min(lower, candle.L);
            }
            vcExetremums[UpperTag] = upper;
            vcExetremums[LowerTag] = lower;
        }

        public double PriceChartWidth
        {
            get { return priceChartContainer.ActualWidth; }
        }
        private void OnPanelCandlesContainerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            CandleChart chart = this.DataContext as CandleChart;
            if (chart == null) return;
            chart.OnPanelCandlesContainerSizeChanged(sender, e);
        }

    }
}
