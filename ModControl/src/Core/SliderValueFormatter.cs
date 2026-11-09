using System;
using System.Globalization;

namespace PreLuna.ModControl.Core
{
    internal enum SliderValueStyle
    {
        Percent,
        NoDecimal,
        OneDecimal,
        TwoDecimal,
        Integer
    }

    internal static class SliderValueFormatter
    {
        internal static string Format(double value, SliderValueStyle style)
        {
            switch (style)
            {
                case SliderValueStyle.Percent:
                    return (value * 100d).ToString("0", CultureInfo.InvariantCulture) + "%";
                case SliderValueStyle.NoDecimal:
                    return value.ToString("0", CultureInfo.InvariantCulture);
                case SliderValueStyle.OneDecimal:
                    return value.ToString("0.0", CultureInfo.InvariantCulture);
                case SliderValueStyle.TwoDecimal:
                    return value.ToString("0.00", CultureInfo.InvariantCulture);
                case SliderValueStyle.Integer:
                    return Math.Round(value, MidpointRounding.AwayFromZero).ToString("0", CultureInfo.InvariantCulture);
                default:
                    return value.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}
