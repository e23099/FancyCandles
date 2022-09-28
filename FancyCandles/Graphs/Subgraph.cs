using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FancyCandles.Indicators;

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
        /// <summary>
        /// the targeting candle chart whose candlesSource and other basic info was used for display
        /// </summary>
        public CandleChart TargetChart { get; set; }

        /// <summary>
        /// tag for getting the upper extrema from VisibleCandlesExtremums
        /// </summary>
        public string UpperTag { get; protected set; }
        /// <summary>
        /// tag for getting the lower extrema from VisibleCandlesExtremums
        /// </summary>
        public string LowerTag { get; protected set; }
        /// <summary>
        /// calculate it's own upper and lower extremums.
        /// </summary>
        /// <param name="candles"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="vcExetremums"></param>
        public abstract void UpdateVisibleCandlesExtremums(ICandlesSource candles, int start, int length, Dictionary<string, double> vcExetremums);

        /// <summary>
        /// xaml string for controling UI.
        /// Plan to change it to return some UserControl.
        /// </summary>
        public abstract string PropertiesEdtiorXAML { get; }


        /// <summary>
        /// get list of infos for this subgraph. (for displaying in ChartInfo)
        /// </summary>
        public virtual ObservableCollection<SubgraphInfo> Infos { get; }

        /// <summary>
        /// get list of indicators for this subgraph. subgraphs may or may not have indicators.
        /// </summary>
        public virtual ObservableCollection<OverlayIndicator> Indicators { get; }


        /// <summary>
        /// support mouse position. 
        /// (Plan to remove this one after PriceChart turned to a Subgraph.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnMouseMoveInsideChartContainer(object sender, MouseEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            var pos = Mouse.GetPosition(element);
            if (this.DataContext is CandleChart chart)
                chart.CurrentMousePosition = pos;
        }

        /// <summary>
        /// support mouse position. Each subgraph should support it's own relative mouse position
        /// for cross hair to be drawn properly inside it's area.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnMouseMoveInsideSubChartContainer(object sender, MouseEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            var pos = Mouse.GetPosition(element);
            if (this.DataContext is Subgraph chart)
                chart.TargetChart.CurrentMousePosition = pos;
        }

        public object Clone()
        {
            var clone = (Subgraph)MemberwiseClone();
            return clone;
        }
    }
}
