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
using System.ComponentModel;
using System.Runtime.CompilerServices; // [CallerMemberName]
using Newtonsoft.Json;
using FancyCandles.Indicators;

namespace FancyCandles.Graphs
{
    /// <summary>
    /// Interaction logic for Volume.xaml
    /// </summary>
    public partial class Volume : Subgraph
    {
        private static int instance_count = 0;


        #region VOLUME PROPERTIES
        public SimpleMovingAverage VolumeMA
        {
            get { return (SimpleMovingAverage)GetValue(VolumeMAProperty); }
            set { SetValue(VolumeMAProperty, value); }
        }

        public static readonly DependencyProperty VolumeMAProperty =
            DependencyProperty.Register("VolumeMA", typeof(SimpleMovingAverage), typeof(Volume),
                new PropertyMetadata(null));

        public bool VolumeMaIsVisible;
        public static readonly DependencyProperty VolumeMaIsVisiblePoperty =
            DependencyProperty.Register("VolumeMaIsVisible", typeof(bool), typeof(Volume),
                new FrameworkPropertyMetadata(true) { AffectsRender = true });


        [UndoableProperty]
        [JsonProperty]
        public double VolumeBarWidthToCandleWidthRatio
        {
            get { return (double)GetValue(VolumeBarWidthToCandleWidthRatioProperty); }
            set { SetValue(VolumeBarWidthToCandleWidthRatioProperty, value); }
        }

        public static readonly DependencyProperty VolumeBarWidthToCandleWidthRatioProperty =
            DependencyProperty.Register("VolumeBarWidthToCandleWidthRatio", typeof(double), typeof(Volume), new PropertyMetadata(DefaultVolumeBarWidthToCandleWidthRatio, null, CoerceVolumeBarWidthToCandleWidthRatio));

        private static object CoerceVolumeBarWidthToCandleWidthRatio(DependencyObject objWithOldDP, object newDPValue)
        {
            //CandleChart thisCandleChart = (CandleChart)objWithOldDP; // Содержит старое значение для изменяемого свойства.
            double newValue = (double)newDPValue;
            return Math.Min(1.0, Math.Max(0.0, newValue));
        }

        public static double DefaultVolumeBarWidthToCandleWidthRatio { get { return 0.8; } }

        [UndoableProperty]
        [JsonProperty]
        public double VolumeHistogramTopMargin
        {
            get { return (double)GetValue(VolumeHistogramTopMarginProperty); }
            set { SetValue(VolumeHistogramTopMarginProperty, value); }
        }
        public static readonly DependencyProperty VolumeHistogramTopMarginProperty =
            DependencyProperty.Register("VolumeHistogramTopMargin", typeof(double), typeof(Volume), new PropertyMetadata(DefaultVolumeHistogramTopMargin));
        public static double DefaultVolumeHistogramTopMargin { get { return 10.0; } }

        [UndoableProperty]
        [JsonProperty]
        public double VolumeHistogramBottomMargin
        {
            get { return (double)GetValue(VolumeHistogramBottomMarginProperty); }
            set { SetValue(VolumeHistogramBottomMarginProperty, value); }
        }
        public static readonly DependencyProperty VolumeHistogramBottomMarginProperty =
            DependencyProperty.Register("VolumeHistogramBottomMargin", typeof(double), typeof(Volume), new PropertyMetadata(DefaultVolumeHistogramBottomMargin));
        public static double DefaultVolumeHistogramBottomMargin { get { return 5.0; } }
        [UndoableProperty]
        [JsonProperty]
        public Brush BullishVolumeBarFill
        {
            get { return (Brush)GetValue(BullishVolumeBarFillProperty); }
            set { SetValue(BullishVolumeBarFillProperty, value); }
        }
        public static readonly DependencyProperty BullishVolumeBarFillProperty =
            DependencyProperty.Register("BullishVolumeBarFill", typeof(Brush), typeof(Volume), new PropertyMetadata(DefaultBullishVolumeBarFill));

        public static Brush DefaultBullishVolumeBarFill { get { return (Brush)(new SolidColorBrush(Colors.Green)).GetCurrentValueAsFrozen(); } }
        [UndoableProperty]
        [JsonProperty]
        public Brush BearishVolumeBarFill
        {
            get { return (Brush)GetValue(BearishVolumeBarFillProperty); }
            set { SetValue(BearishVolumeBarFillProperty, value); }
        }
        public static readonly DependencyProperty BearishVolumeBarFillProperty =
            DependencyProperty.Register("BearishVolumeBarFill", typeof(Brush), typeof(Volume), new PropertyMetadata(DefaultBearishVolumeBarFill));

        public static Brush DefaultBearishVolumeBarFill { get { return (Brush)(new SolidColorBrush(Colors.Red)).GetCurrentValueAsFrozen(); } }
        #endregion

        /// <summary>
        /// compose of:
        ///     VolumeChartElement
        ///     VolumeTickElement
        ///     Grid for showing Y label
        /// </summary>
        public Volume()
        {
            InitializeComponent();
            info = new ObservableCollection<SubgraphInfo>
            {
                new SubgraphInfo
                {
                    Name = "成交量",
                    GetValue = (candle_id) => thisChartElement.GetVolumeValue(candle_id)
                }
            };
            instance_count++; 
            UpperTag = $"Volume{instance_count}H";
            LowerTag = $"Volume{instance_count}L";
            InitAllIndicators();
        }
        public void InitAllIndicators()
        {
            VolumeMA = new SimpleMovingAverage
            {
                TargetSubgraph = this,
                ValueMapper = (cndl) => ((ICandle)cndl).V,
                N = 5,
            };
            Indicators = new ObservableCollection<OverlayIndicator>
            {
                VolumeMA
            };
        }

        public override void UpdateVisibleCandlesExtremums(ICandlesSource candles, int start, int length, Dictionary<string,double> vcExetremums)
        {
            double upper = double.MinValue;
            if (VolumeMA.TargetSource == null && TargetChart.CandlesSource != null)
                thisChartElement.SetTargetSourceForAll_OverlayIndicators();
            for (int i = start; i < start + length; i++)
            {
                ICandle candle = candles[i];
                upper = Math.Max(upper, candle.V);
            }
            vcExetremums[UpperTag] = upper;
            vcExetremums[LowerTag] = 0;
        }

        public ObservableCollection<SubgraphInfo> Infos
        {
            get { return info; }
        }
        private ObservableCollection<SubgraphInfo> info;
    }
}
