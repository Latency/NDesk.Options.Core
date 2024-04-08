// 04****************************************************************************
// Project:  Unit Tests
// File:     OptionContextTest.cs
// Author:   Latency McLaughlin
// Date:     04/04/2024
// ****************************************************************************

using System;
using NDesk.Options;
using Xunit;

namespace Unit_Tests;

public class OptionContextTest
{
    [Fact]
    public void Exceptions()
    {
        var p = new OptionSet
        {
            {
                "a=", v => { /* ignore */ }
            }
        };
        var c = new OptionContext(p);
        Utils.AssertException(typeof(InvalidOperationException), "OptionContext.Option is null.", c, v =>
            {
                var ignore = v.OptionValues[0];
            }
        );
        c.Option = p[0];
        Utils.AssertException(typeof(ArgumentOutOfRangeException), "Specified argument was out of the range of valid values. (Parameter 'index')", c, v =>
            {
                var ignore = v.OptionValues[2];
            }
        );
        c.OptionName = "-a";
        Utils.AssertException(typeof(OptionException), "Missing required value for option '-a'.", c, v =>
            {
                var ignore = v.OptionValues[0];
            }
        );
    }
}