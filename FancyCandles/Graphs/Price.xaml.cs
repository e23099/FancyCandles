using System;
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
    public partial class Price : UserControl
    {
        public static readonly string ExtremeUpper = "priceHigh";
        public static readonly string ExtremeLower = "priceLow";

        public Price()
        {
            InitializeComponent();
        }


        public void UpdateVisibleCandlesExtremums(ICandlesSource candles, int start, int length, Dictionary<string,double> vcExetremums)
        {
            double upper = double.MinValue, lower = double.MaxValue;
            for (int i = start; i < start + length; i++)
            {
                ICandle candle = candles[i];
                upper = Math.Max(upper, candle.H);
                lower = Math.Min(lower, candle.L);
            }
            vcExetremums[ExtremeUpper] = upper;
            vcExetremums[ExtremeLower] = lower;
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

        private void priceChartContainer_MouseMove(object sender, MouseEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            var pos = Mouse.GetPosition(element);
            CandleChart chart = this.DataContext as CandleChart;
            if (chart != null)
                chart.CurrentMousePosition = pos;
        }
    }
}
