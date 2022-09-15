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
    }
}
