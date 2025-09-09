using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace PosSale.Converters
{
    public static class StringConverters
    {
        public static readonly IValueConverter IsNotNullOrEmpty =
            new FuncValueConverter<string, bool>(v => !string.IsNullOrEmpty(v));
    }
}
