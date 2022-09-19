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
using System.ComponentModel;
using System.Runtime.CompilerServices; // [CallerMemberName]

namespace FancyCandles.Graphs
{
    /// <summary>
    /// Interaction logic for Volume.xaml
    /// </summary>
    public partial class Volume : UserControl
    {
        public static readonly string ExtremeUpper = "volumeUpper";
        public static readonly string ExtremeLower = "volumeLower";

        /// <summary>
        /// compose of:
        ///     VolumeChartElement
        ///     VolumeTickElement
        ///     Grid for showing Y label
        /// </summary>
        public Volume()
        {
            InitializeComponent();
        }
        public void UpdateVisibleCandlesExtremums(ICandlesSource candles, int start, int length, Dictionary<string,double> vcExetremums)
        {
            double upper = double.MinValue, lower = double.MaxValue;
            for (int i = start; i < start + length; i++)
            {
                ICandle candle = candles[i];
                upper = Math.Max(upper, candle.V);
                lower = Math.Min(lower, candle.V);
            }
            vcExetremums[ExtremeUpper] = upper;
            vcExetremums[ExtremeLower] = lower;
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!(this.DataContext is CandleChart chart)) return;
            chart.OnMouseWheel(sender, e);
        }

        private void OnMouseMoveInsideFrameworkElement(object sender, MouseEventArgs e)
        {
            if (!(this.DataContext is CandleChart chart)) return;
            chart.OnMouseMoveInsideFrameworkElement(sender, e);
        }
    }
}
