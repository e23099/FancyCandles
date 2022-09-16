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
        public Price()
        {
            InitializeComponent();
        }

        public double PriceChartWidth
        {
            get { return priceChartContainer.ActualWidth; }
        }
        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            CandleChart chart = this.DataContext as CandleChart;
            if (chart == null) return;
            chart.OnMouseWheel(sender, e);
        }

        private void OnMouseMoveInsideFrameworkElement(object sender, MouseEventArgs e)
        {
            CandleChart chart = this.DataContext as CandleChart;
            if (chart == null) return;
            chart.OnMouseMoveInsideFrameworkElement(sender, e);
        }
        private void OnPanelCandlesContainerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            CandleChart chart = this.DataContext as CandleChart;
            if (chart == null) return;
            chart.OnPanelCandlesContainerSizeChanged(sender, e);
        }
    }
}
