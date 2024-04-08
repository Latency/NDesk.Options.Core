// 04****************************************************************************
// Project:  NDesk.Options.Core
// File:     FooConverter.cs
// Author:   Latency McLaughlin
// Date:     04/04/2024
// ****************************************************************************

using System;
using System.ComponentModel;
using System.Globalization;

namespace NDesk.Options;

internal class FooConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) => value is not string v
                                                                                                                 ? base.ConvertFrom(context, culture, value)
                                                                                                                 : v switch
                                                                                                                 {
                                                                                                                     "A" => Foo.A,
                                                                                                                     "B" => Foo.B,
                                                                                                                     _   => base.ConvertFrom(context, culture, value)
                                                                                                                 };
}