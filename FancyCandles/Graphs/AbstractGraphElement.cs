using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.CompilerServices; // [CallerMemberName]
using System.ComponentModel; // For PropertyChange


namespace FancyCandles.Graphs
{
    public abstract class AbstractGraphElement : UserControl
    {
        #region Properties
        /// <summary>
        /// 連結的 CandleSource
        /// </summary>
        public ICandlesSource CandlesSource
        {
            get { return (ICandlesSource)GetValue(CandlesSourceProperty); }
            set { SetValue(CandlesSourceProperty, value); }
        }
        /// <summary>Identifies the <see cref="CandlesSource"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty CandlesSourceProperty =
            DependencyProperty.Register("CandlesSource", typeof(ICandlesSource), typeof(AbstractGraphElement), new UIPropertyMetadata(null, CandleChart.OnCandlesSourceChanged, CandleChart.CoerceCandlesSource));


        /// <summary>
        /// 整體繪圖使用的 CultureInfo
        /// </summary>
        public CultureInfo Culture
        {
            get { return (CultureInfo)GetValue(CultureProperty); }
            set { SetValue(CultureProperty, value); }
        }
        /// <summary>Identifies the <see cref="Culture"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty CultureProperty =
            DependencyProperty.Register("Culture", typeof(CultureInfo), typeof(AbstractGraphElement), new PropertyMetadata(CultureInfo.CurrentCulture));

        /// <summary>
        /// CandleChart 可視範圍
        /// </summary>
        public IntRange VisibleCandlesRange
        {
            get { return (IntRange)GetValue(VisibleCandlesRangeProperty); }
            set { SetValue(VisibleCandlesRangeProperty, value); }
        }
        /// <summary>Identifies the <see cref="VisibleCandlesRange"/> dependency property.</summary>
        /// <value><see cref="DependencyProperty"/></value>
        public static readonly DependencyProperty VisibleCandlesRangeProperty =
            DependencyProperty.Register("VisibleCandlesRange", typeof(IntRange), typeof(AbstractGraphElement), new PropertyMetadata(IntRange.Undefined, CandleChart.OnVisibleCandlesRangeChanged, CandleChart.CoerceVisibleCandlesRange));

        /// <summary>
        /// 可是範圍內的價格、成交量極值
        /// </summary>
        public CandleExtremums VisibleCandlesExtremums
        {
            get { return visibleCandlesExtremums; }
            private set
            {
                visibleCandlesExtremums = value;
                OnPropertyChanged();
            }
        }
        private CandleExtremums visibleCandlesExtremums;
        #endregion

        #region INotifyPropertyChange
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
