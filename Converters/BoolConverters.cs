using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace PosSale.Converters
{
    public static class BoolConverters
    {
        public static readonly IValueConverter Not =
            new FuncValueConverter<bool, bool>(v => !v);
            
        public static readonly IValueConverter TrueToVisible =
            new FuncValueConverter<bool, bool>(v => v);
    }
}