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
using Newtonsoft.Json;

namespace FancyCandles.Graphs
{
    /// <summary>
    /// Interaction logic for Volume.xaml
    /// </summary>
    public partial class Volume : Subgraph
    {
        public static readonly string ExtremeUpper = "volumeUpper";
        public static readonly string ExtremeLower = "volumeLower";

        
        #region VOLUME PROPERTIES
        [UndoableProperty]
        [JsonProperty]
        public double VolumeBarWidthToCandleWidthRatio
        {
            get { return (double)GetValue(VolumeBarWidthToCandleWidthRatioProperty); }
            set { SetValue(VolumeBarWidthToCandleWidthRatioProperty, value); }
        }

        public static readonly DependencyProperty VolumeBarWidthToCandleWidthRatioProperty =
            DependencyProperty.Register("VolumeBarWidthToCandleWidthRatio", typeof(double), typeof(CandleChart), new PropertyMetadata(DefaultVolumeBarWidthToCandleWidthRatio, null, CoerceVolumeBarWidthToCandleWidthRatio));

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
            DependencyProperty.Register("VolumeHistogramTopMargin", typeof(double), typeof(CandleChart), new PropertyMetadata(DefaultVolumeHistogramTopMargin));
        public static double DefaultVolumeHistogramTopMargin { get { return 10.0; } }

        [UndoableProperty]
        [JsonProperty]
        public double VolumeHistogramBottomMargin
        {
            get { return (double)GetValue(VolumeHistogramBottomMarginProperty); }
            set { SetValue(VolumeHistogramBottomMarginProperty, value); }
        }
        public static readonly DependencyProperty VolumeHistogramBottomMarginProperty =
            DependencyProperty.Register("VolumeHistogramBottomMargin", typeof(double), typeof(CandleChart), new PropertyMetadata(DefaultVolumeHistogramBottomMargin));
        public static double DefaultVolumeHistogramBottomMargin { get { return 5.0; } }
        [UndoableProperty]
        [JsonProperty]
        public Brush BullishVolumeBarFill
        {
            get { return (Brush)GetValue(BullishVolumeBarFillProperty); }
            set { SetValue(BullishVolumeBarFillProperty, value); }
        }
        public static readonly DependencyProperty BullishVolumeBarFillProperty =
            DependencyProperty.Register("BullishVolumeBarFill", typeof(Brush), typeof(CandleChart), new PropertyMetadata(DefaultBullishVolumeBarFill));

        public static Brush DefaultBullishVolumeBarFill { get { return (Brush)(new SolidColorBrush(Colors.Green)).GetCurrentValueAsFrozen(); } }
        [UndoableProperty]
        [JsonProperty]
        public Brush BearishVolumeBarFill
        {
            get { return (Brush)GetValue(BearishVolumeBarFillProperty); }
            set { SetValue(BearishVolumeBarFillProperty, value); }
        }
        public static readonly DependencyProperty BearishVolumeBarFillProperty =
            DependencyProperty.Register("BearishVolumeBarFill", typeof(Brush), typeof(CandleChart), new PropertyMetadata(DefaultBearishVolumeBarFill));

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
        }
        public override void UpdateVisibleCandlesExtremums(ICandlesSource candles, int start, int length, Dictionary<string,double> vcExetremums)
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

                            <StackPanel Style=""{{StaticResource settingsItem}}"" ToolTip=""The gap between the highest point of the visible bars and the top border of the volume histogram panel."">
                                <TextBlock Style=""{{StaticResource horizontalCaption}}"">Indent from top:</TextBlock>
                                <fp:IntegerTextBox x:Name=""txtBoxVolumeHistogramTopMargin"" 
                                                   MinValue=""0"" Width=""30""
                                                   Text=""{{Binding VolumeHistogramTopMargin, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}}""/>
                                <fp:UpDownButtonBlock Height=""{{Binding ElementName=txtBoxVolumeHistogramTopMargin, Path=ActualHeight}}"" Width=""20""
                                                      UpButtonPressedCommand=""{{Binding ElementName=txtBoxVolumeHistogramTopMargin, Path=IncrementValueCommand}}""
                                                      DownButtonPressedCommand=""{{Binding ElementName=txtBoxVolumeHistogramTopMargin, Path=DecrementValueCommand}}""/>
                            </StackPanel>

                            <StackPanel Style=""{{StaticResource settingsItem}}"" ToolTip=""The gap between the lowest point of the visible bars and the bottom border of the volume histogram panel."">
                                <TextBlock Style=""{{StaticResource horizontalCaption}}"">Indent from bottom:</TextBlock>
                                <fp:IntegerTextBox x:Name=""txtBoxVolumeHistogramBottomMargin"" 
                                                   MinValue=""0"" Width=""30""
                                                   Text=""{{Binding VolumeHistogramBottomMargin, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}}""/>
                                <fp:UpDownButtonBlock Height=""{{Binding ElementName=txtBoxVolumeHistogramBottomMargin, Path=ActualHeight}}"" Width=""20""
                                                      UpButtonPressedCommand=""{{Binding ElementName=txtBoxVolumeHistogramBottomMargin, Path=IncrementValueCommand}}""
                                                      DownButtonPressedCommand=""{{Binding ElementName=txtBoxVolumeHistogramBottomMargin, Path=DecrementValueCommand}}""/>

                            </StackPanel>
                            
                            <StackPanel Style=""{{StaticResource settingsItem}}"">
                                <TextBlock Style=""{{StaticResource horizontalCaption}}"">Bullish volume bar fill:</TextBlock>
                                <fp:StandardColorPicker SelectedColor=""{{Binding BullishVolumeBarFill, Converter={{StaticResource symColorBrushStringConverter}}, Mode=TwoWay}}"" VerticalAlignment=""Bottom""/>
                            </StackPanel>
                            
                            <StackPanel Style=""{{StaticResource settingsItem}}"">
                                <TextBlock Style=""{{StaticResource horizontalCaption}}"">Bearish volume bar fill:</TextBlock>
                                <fp:StandardColorPicker SelectedColor=""{{Binding BearishVolumeBarFill, Converter={{StaticResource symColorBrushStringConverter}}, Mode=TwoWay}}"" VerticalAlignment=""Bottom""/>
                            </StackPanel>

                            <StackPanel Style=""{{StaticResource settingsItem}}"">
                                <TextBlock Style=""{{StaticResource horizontalCaption}}"">Volume bar width:</TextBlock>
                                <Slider Value=""{{Binding VolumeBarWidthToCandleWidthRatio, Mode=TwoWay}}"" 
                                        VerticalAlignment=""Bottom"" Width=""70"" Minimum=""0"" Maximum=""1.0""/>
                            </StackPanel>
                        </StackPanel>
                ";
            }
        }
    }
}
