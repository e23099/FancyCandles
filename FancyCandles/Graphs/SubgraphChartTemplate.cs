using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using FancyCandles.Indicators;

namespace FancyCandles.Graphs
{
    /// <summary>
    /// A template class for creating subgraph's chart element.
    /// </summary>
    public abstract class SubgraphChartTemplate : FrameworkElement
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public SubgraphChartTemplate()
        {
            ToolTip tt = new ToolTip() { FontSize = CandleChart.ToolTipFontSize, BorderBrush = Brushes.Beige };
            tt.Content = "";
            ToolTip = tt;

            // We set the delay time for the appearance of hints here, and the location of the hints (if it needs to be changed) is set in XAML:
            ToolTipService.SetShowDuration(this, int.MaxValue);
            ToolTipService.SetInitialShowDelay(this, 0);
        }


        /// <summary>
        /// The string used for finding the upper bound of current VisibleCandles 
        /// by querying VisibleCandlesExtremums.
        /// </summary>
        public string UpperTag
        {
            get { return ((string)GetValue(UpperTagProperty)).ToString(); }
            set { SetValue(UpperTagProperty, value); }
        }
        public static readonly DependencyProperty UpperTagProperty
            = DependencyProperty.Register("UpperTag", typeof(string), typeof(SubgraphChartTemplate), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// The string used for finding the lower bound of current VisibleCandles 
        /// by querying VisibleCandlesExtremums.
        /// </summary>
        public string LowerTag
        {
            get { return ((string)GetValue(LowerTagProperty)).ToString(); }
            set { SetValue(LowerTagProperty, value); }
        }
        public static readonly DependencyProperty LowerTagProperty
            = DependencyProperty.Register("LowerTag", typeof(string), typeof(SubgraphChartTemplate), new FrameworkPropertyMetadata(null));


        #region Indicators

        public static readonly DependencyProperty IndicatorsProperty
            = DependencyProperty.Register("Indicators", typeof(ObservableCollection<OverlayIndicator>), typeof(SubgraphChartTemplate), 
                new FrameworkPropertyMetadata(null, OnIndicatorsChanged) { AffectsRender = true });
        public ObservableCollection<OverlayIndicator> Indicators
        {
            get { return (ObservableCollection<OverlayIndicator>)GetValue(IndicatorsProperty); }
            set { SetValue(IndicatorsProperty, value); }
        }

        static void OnIndicatorsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            SubgraphChartTemplate thisSubgraphElementTemplate = obj as SubgraphChartTemplate;
            if (thisSubgraphElementTemplate == null) return;

            ObservableCollection<OverlayIndicator> old_obsCollection = e.OldValue as ObservableCollection<OverlayIndicator>;
            if (old_obsCollection != null)
            {
                old_obsCollection.CollectionChanged -= thisSubgraphElementTemplate.OnIndicatorsCollectionChanged;

                foreach (OverlayIndicator indicator in old_obsCollection)
                    indicator.PropertyChanged -= thisSubgraphElementTemplate.OnIndicatorsCollectionItemChanged;
            }

            ObservableCollection<OverlayIndicator> new_obsCollection = e.NewValue as ObservableCollection<OverlayIndicator>;
            if (new_obsCollection != null)
            {
                new_obsCollection.CollectionChanged += thisSubgraphElementTemplate.OnIndicatorsCollectionChanged;

                foreach (OverlayIndicator indicator in new_obsCollection)
                    indicator.PropertyChanged += thisSubgraphElementTemplate.OnIndicatorsCollectionItemChanged;
            }
        }

        private void OnIndicatorsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (OverlayIndicator indicator in e.NewItems)
                    indicator.PropertyChanged += OnIndicatorsCollectionItemChanged;
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (OverlayIndicator indicator in e.NewItems)
                    indicator.PropertyChanged += OnIndicatorsCollectionItemChanged;

                foreach (OverlayIndicator indicator in e.OldItems)
                    indicator.PropertyChanged -= OnIndicatorsCollectionItemChanged;
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (OverlayIndicator indicator in e.OldItems)
                    indicator.PropertyChanged -= OnIndicatorsCollectionItemChanged;
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (OverlayIndicator indicator in (sender as IEnumerable<OverlayIndicator>))
                    indicator.PropertyChanged += OnIndicatorsCollectionItemChanged;
            }
            else if (e.Action == NotifyCollectionChangedAction.Move) {}

            InvalidateVisual();
        }

        private void OnIndicatorsCollectionItemChanged(object source, PropertyChangedEventArgs args)
        {
            InvalidateVisual();
        }

        public abstract void SetTargetSourceForAll_OverlayIndicators();

        #endregion

        #region Target Chart
        public CultureInfo Culture
        {
            get { return (CultureInfo)GetValue(CultureProperty); }
            set { SetValue(CultureProperty, value); }
        }
        public static readonly DependencyProperty CultureProperty =
            DependencyProperty.Register("Culture", typeof(CultureInfo), typeof(SubgraphChartTemplate), new FrameworkPropertyMetadata(CultureInfo.CurrentCulture) { AffectsRender = true });

        public ICandlesSource CandlesSource
        {
            get { return (ICandlesSource)GetValue(CandlesSourceProperty); }
            set { SetValue(CandlesSourceProperty, value); }
        }
        public static readonly DependencyProperty CandlesSourceProperty
             = DependencyProperty.Register("CandlesSource", typeof(ICandlesSource), typeof(SubgraphChartTemplate), 
                 new UIPropertyMetadata(null, OnCandlesSourceChanged));

        internal static void OnCandlesSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            SubgraphChartTemplate thisChart = obj as SubgraphChartTemplate;
            if (thisChart == null) return;
            if (thisChart.IsLoaded)
            {
                thisChart.OnCandlesSourceChanged();
            }
        }

        protected abstract void OnCandlesSourceChanged();


        public Dictionary<string,double> VisibleCandlesExtremums
        {
            get { return (Dictionary<string,double>)GetValue(VisibleCandlesExtremumsProperty); }
            set { SetValue(VisibleCandlesExtremumsProperty, value); }
        }
        public static readonly DependencyProperty VisibleCandlesExtremumsProperty
            = DependencyProperty.Register("VisibleCandlesExtremums", typeof(Dictionary<string,double>),
                typeof(SubgraphChartTemplate), new FrameworkPropertyMetadata(null) { AffectsRender = true });
        

        public IntRange VisibleCandlesRange
        {
            get { return (IntRange)GetValue(VisibleCandlesRangeProperty); }
            set { SetValue(VisibleCandlesRangeProperty, value); }
        }
        public static readonly DependencyProperty VisibleCandlesRangeProperty
             = DependencyProperty.Register("VisibleCandlesRange", typeof(IntRange), typeof(SubgraphChartTemplate), new FrameworkPropertyMetadata(IntRange.Undefined));


        public CandleDrawingParameters CandleWidthAndGap
        {
            get { return (CandleDrawingParameters)GetValue(CandleWidthAndGapProperty); }
            set { SetValue(CandleWidthAndGapProperty, value); }
        }
        public static readonly DependencyProperty CandleWidthAndGapProperty
             = DependencyProperty.Register("CandleWidthAndGap", typeof(CandleDrawingParameters), typeof(SubgraphChartTemplate),
                 new FrameworkPropertyMetadata(new CandleDrawingParameters()));


        public int MaxFractionalDigits
        {
            get { return (int)GetValue(MaxFractionalDigitsProperty); }
            set { SetValue(MaxFractionalDigitsProperty, value); }
        }
        public static readonly DependencyProperty MaxFractionalDigitsProperty =
            DependencyProperty.Register("MaxFractionalDigits", typeof(int), typeof(SubgraphChartTemplate), new FrameworkPropertyMetadata(0));
        #endregion

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
