// 04****************************************************************************
// Project:  Unit Tests
// File:     OptionTest.cs
// Author:   Latency McLaughlin
// Date:     04/04/2024
// ****************************************************************************

using System;
using NDesk.Options;
using Xunit;

namespace Unit_Tests;

internal class DefaultOption : Option
{
    public DefaultOption(string prototypes, string description) : base(prototypes, description)
    { }

    public DefaultOption(string prototypes, string description, int c) : base(prototypes, description, c)
    { }

    protected override void OnParseComplete(OptionContext c) => throw new NotImplementedException();
}

public class OptionTest
{
    [Fact]
    public void Exceptions()
    {
        Utils.AssertException(typeof(ArgumentNullException), "Value cannot be null. (Parameter 'prototype')", (object)null, v =>
            {
                _ = new DefaultOption(null, null);
            }
        );
        Utils.AssertException(typeof(ArgumentException), "Cannot be the empty string. (Parameter 'prototype')", (object)null, v =>
            {
                _ = new DefaultOption("", null);
            }
        );
        Utils.AssertException(typeof(ArgumentException), "Empty option names are not supported. (Parameter 'prototype')", (object)null, v =>
            {
                _ = new DefaultOption("a|b||c=", null);
            }
        );
        Utils.AssertException(typeof(ArgumentException), "Conflicting option types: '=' vs. ':'. (Parameter 'prototype')", (object)null, v =>
            {
                _ = new DefaultOption("a=|b:", null);
            }
        );
        Utils.AssertException(typeof(ArgumentException), "The default option handler '<>' cannot require values. (Parameter 'prototype')", (object)null, v =>
            {
                _ = new DefaultOption("<>=", null);
            }
        );
        Utils.AssertException(typeof(ArgumentException), "The default option handler '<>' cannot require values. (Parameter 'prototype')", (object)null, v =>
            {
                _ = new DefaultOption("<>:", null);
            }
        );
        Utils.AssertException(null, null, (object)null, v =>
            {
                _ = new DefaultOption("t|<>=", null, 1);
            }
        );
        Utils.AssertException(typeof(ArgumentException), "The default option handler '<>' cannot require values. (Parameter 'prototype')", (object)null, v =>
            {
                _ = new DefaultOption("t|<>=", null, 2);
            }
        );
        Utils.AssertException(null, null, (object)null, v =>
            {
                _ = new DefaultOption("a|b=", null, 2);
            }
        );
        Utils.AssertException(typeof(ArgumentOutOfRangeException), "Specified argument was out of the range of valid values. (Parameter 'maxValueCount')", (object)null, v =>
            {
                _ = new DefaultOption("a", null, -1);
            }
        );
        Utils.AssertException(typeof(ArgumentException), "Cannot provide maxValueCount of 0 for OptionValueType.Required or OptionValueType.Optional. (Parameter 'maxValueCount')", (object)null, v =>
            {
                _ = new DefaultOption("a=", null, 0);
            }
        );
        Utils.AssertException(typeof(ArgumentException), "Ill-formed name/value separator found in \"a={\". (Parameter 'name')", (object)null, v =>
            {
                _ = new DefaultOption("a={", null);
            }
        );
        Utils.AssertException(typeof(ArgumentException), "Ill-formed name/value separator found in \"a=}\". (Parameter 'name')", (object)null, v =>
            {
                _ = new DefaultOption("a=}", null);
            }
        );
        Utils.AssertException(typeof(ArgumentException), "Ill-formed name/value separator found in \"a={{}}\". (Parameter 'name')", (object)null, v =>
            {
                _ = new DefaultOption("a={{}}", null);
            }
        );
        Utils.AssertException(typeof(ArgumentException), "Ill-formed name/value separator found in \"a={}}\". (Parameter 'name')", (object)null, v =>
            {
                _ = new DefaultOption("a={}}", null);
            }
        );
        Utils.AssertException(typeof(ArgumentException), "Ill-formed name/value separator found in \"a={}{\". (Parameter 'name')", (object)null, v =>
            {
                _ = new DefaultOption("a={}{", null);
            }
        );
        Utils.AssertException(typeof(ArgumentException), "Cannot provide key/value separators for Options taking 1 value(s). (Parameter 'prototype')", (object)null, v =>
            {
                _ = new DefaultOption("a==", null);
            }
        );
        Utils.AssertException(typeof(ArgumentException), "Cannot provide key/value separators for Options taking 1 value(s). (Parameter 'prototype')", (object)null, v =>
            {
                _ = new DefaultOption("a={}", null);
            }
        );
        Utils.AssertException(typeof(ArgumentException), "Cannot provide key/value separators for Options taking 1 value(s). (Parameter 'prototype')", (object)null, v =>
            {
                _ = new DefaultOption("a=+-*/", null);
            }
        );
        Utils.AssertException(null, null, (object)null, v =>
            {
                _ = new DefaultOption("a", null, 0);
            }
        );
        Utils.AssertException(null, null, (object)null, v =>
            {
                _ = new DefaultOption("a", null, 0);
            }
        );
        Utils.AssertException(null, null, (object)null, v =>
            {
                var d = new DefaultOption("a", null);
                Assert.Empty(d.GetValueSeparators());
            }
        );
        Utils.AssertException(null, null, (object)null, v =>
            {
                var d = new DefaultOption("a=", null, 1);
                var s = d.GetValueSeparators();
                Assert.Empty(s);
            }
        );
        Utils.AssertException(null, null, (object)null, v =>
            {
                var d = new DefaultOption("a=", null, 2);
                var s = d.GetValueSeparators();
                Assert.Equal(2,   s.Length);
                Assert.Equal(":", s[0]);
                Assert.Equal("=", s[1]);
            }
        );
        Utils.AssertException(null, null, (object)null, v =>
            {
                var d = new DefaultOption("a={}", null, 2);
                var s = d.GetValueSeparators();
                Assert.Empty(s);
            }
        );
        Utils.AssertException(null, null, (object)null, v =>
            {
                var d = new DefaultOption("a={-->}{=>}", null, 2);
                var s = d.GetValueSeparators();
                Assert.Equal(2,     s.Length);
                Assert.Equal("-->", s[0]);
                Assert.Equal("=>",  s[1]);
            }
        );
        Utils.AssertException(null, null, (object)null, v =>
            {
                var d = new DefaultOption("a=+-*/", null, 2);
                var s = d.GetValueSeparators();
                Assert.Equal(4,   s.Length);
                Assert.Equal("+", s[0]);
                Assert.Equal("-", s[1]);
                Assert.Equal("*", s[2]);
                Assert.Equal("/", s[3]);
            }
        );
    }
}