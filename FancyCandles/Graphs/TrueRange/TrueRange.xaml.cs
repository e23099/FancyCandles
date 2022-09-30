using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

using FancyCandles.Indicators;

namespace FancyCandles.Graphs
{
    /// <summary>
    /// Interaction logic for TrueRange.xaml
    /// </summary>
    public partial class TrueRange : Subgraph
    {
        private static int instance_count = 0;

        #region Properties
        public Brush TrueRangeBarFill;
        public static readonly DependencyProperty TrueRangeBarFillProperty = 
            DependencyProperty.Register("TrueRangeBarFill", typeof(Brush), typeof(TrueRange),
                new PropertyMetadata(DefaultTrueRangeBarFill));
        public static Brush DefaultTrueRangeBarFill { get { return (Brush)(new SolidColorBrush(Colors.Teal)); } }


        public SimpleMovingAverage ATR
        {
            get { return (SimpleMovingAverage)GetValue(ATRProperty); }
            set { SetValue(ATRProperty, value); }
        }

        public static readonly DependencyProperty ATRProperty =
            DependencyProperty.Register("ATR", typeof(SimpleMovingAverage), typeof(TrueRange),
                new PropertyMetadata(null));


        /// <summary>
        /// chart element top margin
        /// </summary>
        public double TopMargin
        {
            get { return topMargin; }
            set { topMargin = value; }
        }
        private double topMargin = 10;

        /// <summary>
        /// chart element bottom margin
        /// </summary>
        public double BottomMargin
        {
            get { return bottomMargin; }
            set { bottomMargin = value; }
        }
        private double bottomMargin = 5;
        #endregion

        public TrueRange()
        {
            InitializeComponent();
            infos = new ObservableCollection<SubgraphInfo>
            {
                new SubgraphInfo
                {
                    Name = "TR",
                    GetValue = (candle_i) => thisSubgraphElement.GetTrueRangeValue(candle_i)
                }
            };
            instance_count++;
            UpperTag = $"TrueRange{instance_count}H";
            LowerTag = $"TrueRange{instance_count}L";
            InitAllIndicators();
        }

        public void InitAllIndicators()
        {
            ATR = new SimpleMovingAverage
            {
                TargetSubgraph = this,
                ValueMapper = (tr) => (double)tr,
                N = 3,
            };
            Indicators = new ObservableCollection<OverlayIndicator>
            {
                ATR
            };
        }


        public override ObservableCollection<SubgraphInfo> Infos
        {
            get { return infos; }
        }
        private ObservableCollection<SubgraphInfo> infos;

        public override void UpdateVisibleCandlesExtremums(ICandlesSource candles, int start, int length, Dictionary<string, double> vcExetremums)
        {
            double high = double.MinValue;
            if (thisSubgraphElement.CandlesTrueRange.Count == 0) return;
            for (int i = start; i < start + length; i++)
            {
                double tr = thisSubgraphElement.CandlesTrueRange[i];
                high = Math.Max(high, tr);
            }
            vcExetremums[UpperTag] = high;
            vcExetremums[LowerTag] = 0;
        }
    }
}
