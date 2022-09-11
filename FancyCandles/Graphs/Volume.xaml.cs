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

namespace FancyCandles.Graphs
{
    /// <summary>
    /// Interaction logic for Volume.xaml
    /// </summary>
    public partial class Volume : UserControl
    {

        private CandleChart parentChart;
        public Volume(CandleChart parentChart)
        {
            this.parentChart = parentChart;
            this.DataContext = this.parentChart;
            InitializeComponent();
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            parentChart.OnMouseWheel(sender, e);
        }

        private void OnMouseMoveInsideFrameworkElement(object sender, MouseEventArgs e)
        {
            parentChart.OnMouseMoveInsideFrameworkElement(sender, e);
        }

    }
}
