// 04****************************************************************************
// Project:  NDesk.Options.Core
// File:     Foo.cs
// Author:   Latency McLaughlin
// Date:     04/04/2024
// ****************************************************************************
// ReSharper disable CheckNamespace

using System.ComponentModel;

namespace NDesk.Options;

[TypeConverter(typeof(FooConverter))]
internal class Foo
{
    public static readonly Foo    A = new("A");
    public static readonly Foo    B = new("B");
    private readonly       string _s;

    private Foo(string s) => _s = s;

    public override string ToString() => _s;
}