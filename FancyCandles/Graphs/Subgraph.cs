using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyCandles.Graphs
{
    /// <summary>
    /// a Subgraph abstract class that targets the CandleChart instance as its DataContext to display what it needs to display.
    /// </summary>
    public abstract class Subgraph : UserControl, ICloneable
    {
        /// <summary>
        /// return Name of this subgraph
        /// </summary>
        public string GraphName
        {
            get { return this.GetType().Name; }
        }
        public CandleChart TargetChart { get; set; }

        public abstract void UpdateVisibleCandlesExtremums(ICandlesSource candles, int start, int length, Dictionary<string, double> vcExetremums);

        public abstract string PropertiesEdtiorXAML { get; }

        public void OnMouseMoveInsideChartContainer(object sender, MouseEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            var pos = Mouse.GetPosition(element);
            CandleChart chart = this.DataContext as CandleChart;
            if (chart != null)
                chart.CurrentMousePosition = pos;
        }

        public void OnMouseMoveInsideSubChartContainer(object sender, MouseEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            var pos = Mouse.GetPosition(element);
            Subgraph chart = this.DataContext as Subgraph;
            if (chart != null)
                chart.TargetChart.CurrentMousePosition = pos;
        }

        public object Clone()
        {
            var clone = (Subgraph)MemberwiseClone();
            return clone;
        }
    }
}
