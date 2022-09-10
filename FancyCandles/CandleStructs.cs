using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyCandles
{
    internal struct CandleDrawingParameters
    {
        public double Width;
        public double Gap;
        public CandleDrawingParameters(double width, double gapBetweenCandles)
        {
            Width = width;
            Gap = gapBetweenCandles;
        }
    }
#pragma warning restore CS1591

    /// <summary>Represents the extreme values of Price and Volume for a set of candlesticks.</summary>
    public struct CandleExtremums
    {
        /// <summary>The Price minimum.</summary>
        /// <value>The Price minimum.</value>
        public double PriceLow;

        /// <summary>The Price maximum.</summary>
        /// <value>The Price maximum.</value>
        public double PriceHigh;

        /// <summary>The Volume minimum.</summary>
        /// <value>The Volume minimum.</value>
        public double VolumeLow;

        /// <summary>The Volume maximum.</summary>
        /// <value>The Volume maximum.</value>
        public double VolumeHigh;

        /// <summary>Initializes a new instance of the CandleExtremums structure that has the specified PriceLow, PriceHigh, VolumeLow, and VolumeHigh.</summary>
        /// <param name="priceLow">The PriceLow of the CandleExtremums.</param>
        /// <param name="priceHigh">The PriceHigh of the CandleExtremums.</param>
        /// <param name="volumeLow">The VolumeLow of the CandleExtremums.</param>
        /// <param name="volumeHigh">The VolumeHigh of the CandleExtremums.</param>
        public CandleExtremums(double priceLow, double priceHigh, double volumeLow, double volumeHigh)
        {
            PriceLow = priceLow;
            PriceHigh = priceHigh;
            VolumeLow = volumeLow;
            VolumeHigh = volumeHigh;
        }
#pragma warning  disable CS1591
        public override bool Equals(object obj) { return false; }
#pragma warning restore CS1591
    }
}
