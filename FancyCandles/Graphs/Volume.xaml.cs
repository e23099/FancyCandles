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
    public partial class Volume : UserControl, INotifyPropertyChanged
    {
        public CandleChart ParentChart
        {
            get { return (CandleChart)GetValue(ParentChartProperty); }
            set { SetValue(ParentChartProperty, value); _chart = value; }
        }
        /// <summary>Identifies the <see cref="CandleChart"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty ParentChartProperty =
            DependencyProperty.Register("ParentChart", typeof(CandleChart), typeof(CandleChart), new FrameworkPropertyMetadata(new CandleChart()));

        private CandleChart _chart;

        public Volume()
        {
            InitializeComponent();
        }


        #region context menu

        private void OpenCandleChartPropertiesWindow(object sender, RoutedEventArgs e)
        {
            ParentChart.OpenCandleChartPropertiesWindow(sender, e);
        }

        private void OpenLoadSettingsDialog(object sender, RoutedEventArgs e)
        {
            ParentChart.OpenLoadSettingsDialog(sender, e);
        }

        internal void OpenSaveSettingsAsDialog(object sender, RoutedEventArgs e)
        {
            ParentChart.OpenSaveSettingsAsDialog(sender, e);
        }

        #endregion


        #region mouse 
        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ParentChart.OnMouseWheel(sender, e);
        }

        private void OnMouseMoveInsideVolumeHistogramContainer(object sender, MouseEventArgs e)
        {
            CurrentMousePosition = Mouse.GetPosition(volumeHistogramContainer);
        }

        Point currentMousePosition;
        /// <summary>This is a property for internal use only. You should not use it.</summary>
        public Point CurrentMousePosition
        {
            get { return currentMousePosition; }
            private set
            {
                if (currentMousePosition == value) return;
                currentMousePosition = value;
                OnPropertyChanged();
            }
        }
        #endregion

        //---------------- INotifyPropertyChanged ----------------------------------------------------------
        /// <summary>INotifyPropertyChanged interface realization.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>INotifyPropertyChanged interface realization.</summary>
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
