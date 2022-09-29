﻿using System;
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



        public override string PropertiesEdtiorXAML
        {
            get
            {
                return $@"
                        <StackPanel VerticalAlignment=""Top"" Margin=""5 10 0 0"">
                            <StackPanel.Resources>
                                <Style x:Key=""horizontalCaption"" TargetType=""TextBlock"">
                                    <Setter Property=""Margin"" Value=""0 0 5 3""/>
                                    <Setter Property=""VerticalAlignment"" Value=""Bottom""/>
                                </Style>
                                <Style x:Key=""settingsItem"" TargetType=""StackPanel"">
                                    <Setter Property=""Orientation"" Value=""Horizontal""/>
                                    <Setter Property=""FrameworkElement.HorizontalAlignment"" Value=""Left""/>
                                    <Setter Property=""FrameworkElement.Margin"" Value=""0 8 0 0""/>
                                    <Setter Property=""ToolTipService.InitialShowDelay"" Value=""0""/>
                                    <Setter Property=""ToolTipService.ShowDuration"" Value=""7000""/>
                                </Style>
                            </StackPanel.Resources>

                            <StackPanel Style=""{{StaticResource settingsItem}}"">
                                <TextBlock Style=""{{StaticResource horizontalCaption}}"">True Range bar fill:</TextBlock>
                                <fp:StandardColorPicker SelectedColor=""{{Binding TrueRangeBarFill, Converter={{StaticResource symColorBrushStringConverter}}, Mode=TwoWay}}"" VerticalAlignment=""Bottom""/>
                            </StackPanel>

                            <StackPanel Style=""{{StaticResource settingsItem}}"">
                                <TextBlock Style=""{{StaticResource horizontalCaption}}"">ATR N=</TextBlock>
                                <fp:IntegerTextBox MinValue=""1"" Text=""{{Binding ATR.N, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}}"" VerticalAlignment=""Bottom""/>
                            </StackPanel>

                            <StackPanel Style=""{{StaticResource settingsItem}}"">
                                <TextBlock Style=""{{StaticResource horizontalCaption}}"">ATR Line:</TextBlock>
                                <fp:PenSelector SelectedPen=""{{Binding ATR.Pen, Mode = TwoWay}}"" VerticalAlignment=""Bottom""/>
                            </StackPanel>

                        </StackPanel>
                        ";
            }
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