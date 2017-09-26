//  *****************************************************************************
//  File:       OptionTest.cs
//  Solution:   NDesk.Options
//  Project:    Tests
//  Date:       09/26/2017
//  Author:     Latency McLaughlin
//  Copywrite:  Bio-Hazard Industries - 1998-2017
//  *****************************************************************************

using System;
using NDesk.Options;
using NUnit.Framework;

// ReSharper disable ObjectCreationAsStatement

namespace Tests.NDesk.Options {
  internal class DefaultOption : Option {
    public DefaultOption(string prototypes, string description) : base(prototypes, description) {
    }

    public DefaultOption(string prototypes, string description, int c) : base(prototypes, description, c) {
    }

    protected override void OnParseComplete(OptionContext c) {
      throw new NotImplementedException();
    }
  }

  [TestFixture]
  public class OptionTest {
    [Test]
    public void Exceptions() {
      Utils.AssertException(typeof(ArgumentNullException), "Argument cannot be null.\nParameter name: prototype", (object) null, v => {
        new DefaultOption(null, null);
      });
      Utils.AssertException(typeof(ArgumentException), "Cannot be the empty string.\nParameter name: prototype", (object) null, v => {
        new DefaultOption("", null);
      });
      Utils.AssertException(typeof(ArgumentException), "Empty option names are not supported.\nParameter name: prototype", (object) null, v => {
        new DefaultOption("a|b||c=", null);
      });
      Utils.AssertException(typeof(ArgumentException), "Conflicting option types: '=' vs. ':'.\nParameter name: prototype", (object) null, v => {
        new DefaultOption("a=|b:", null);
      });
      Utils.AssertException(typeof(ArgumentException), "The default option handler '<>' cannot require values.\nParameter name: prototype", (object) null, v => {
        new DefaultOption("<>=", null);
      });
      Utils.AssertException(typeof(ArgumentException), "The default option handler '<>' cannot require values.\nParameter name: prototype", (object) null, v => {
        new DefaultOption("<>:", null);
      });
      Utils.AssertException(null, null, (object) null, v => {
        new DefaultOption("t|<>=", null, 1);
      });
      Utils.AssertException(typeof(ArgumentException), "The default option handler '<>' cannot require values.\nParameter name: prototype", (object) null, v => {
        new DefaultOption("t|<>=", null, 2);
      });
      Utils.AssertException(null, null, (object) null, v => {
        new DefaultOption("a|b=", null, 2);
      });
      Utils.AssertException(typeof(ArgumentOutOfRangeException), "Argument is out of range.\nParameter name: maxValueCount", (object) null, v => {
        new DefaultOption("a", null, -1);
      });
      Utils.AssertException(typeof(ArgumentException), "Cannot provide maxValueCount of 0 for OptionValueType.Required or " + "OptionValueType.Optional.\nParameter name: maxValueCount", (object) null, v => {
        new DefaultOption("a=", null, 0);
      });
      Utils.AssertException(typeof(ArgumentException), "Ill-formed name/value separator found in \"a={\".\nParameter name: prototype", (object) null, v => {
        new DefaultOption("a={", null);
      });
      Utils.AssertException(typeof(ArgumentException), "Ill-formed name/value separator found in \"a=}\".\nParameter name: prototype", (object) null, v => {
        new DefaultOption("a=}", null);
      });
      Utils.AssertException(typeof(ArgumentException), "Ill-formed name/value separator found in \"a={{}}\".\nParameter name: prototype", (object) null, v => {
        new DefaultOption("a={{}}", null);
      });
      Utils.AssertException(typeof(ArgumentException), "Ill-formed name/value separator found in \"a={}}\".\nParameter name: prototype", (object) null, v => {
        new DefaultOption("a={}}", null);
      });
      Utils.AssertException(typeof(ArgumentException), "Ill-formed name/value separator found in \"a={}{\".\nParameter name: prototype", (object) null, v => {
        new DefaultOption("a={}{", null);
      });
      Utils.AssertException(typeof(ArgumentException), "Cannot provide key/value separators for Options taking 1 value(s).\nParameter name: prototype", (object) null, v => {
        new DefaultOption("a==", null);
      });
      Utils.AssertException(typeof(ArgumentException), "Cannot provide key/value separators for Options taking 1 value(s).\nParameter name: prototype", (object) null, v => {
        new DefaultOption("a={}", null);
      });
      Utils.AssertException(typeof(ArgumentException), "Cannot provide key/value separators for Options taking 1 value(s).\nParameter name: prototype", (object) null, v => {
        new DefaultOption("a=+-*/", null);
      });
      Utils.AssertException(null, null, (object) null, v => {
        new DefaultOption("a", null, 0);
      });
      Utils.AssertException(null, null, (object) null, v => {
        new DefaultOption("a", null, 0);
      });
      Utils.AssertException(null, null, (object) null, v => {
        var d = new DefaultOption("a", null);
        Assert.AreEqual(d.GetValueSeparators().Length, 0);
      });
      Utils.AssertException(null, null, (object) null, v => {
        var d = new DefaultOption("a=", null, 1);
        var s = d.GetValueSeparators();
        Assert.AreEqual(s.Length, 0);
      });
      Utils.AssertException(null, null, (object) null, v => {
        var d = new DefaultOption("a=", null, 2);
        var s = d.GetValueSeparators();
        Assert.AreEqual(s.Length, 2);
        Assert.AreEqual(s[0], ":");
        Assert.AreEqual(s[1], "=");
      });
      Utils.AssertException(null, null, (object) null, v => {
        var d = new DefaultOption("a={}", null, 2);
        var s = d.GetValueSeparators();
        Assert.AreEqual(s.Length, 0);
      });
      Utils.AssertException(null, null, (object) null, v => {
        var d = new DefaultOption("a={-->}{=>}", null, 2);
        var s = d.GetValueSeparators();
        Assert.AreEqual(s.Length, 2);
        Assert.AreEqual(s[0], "-->");
        Assert.AreEqual(s[1], "=>");
      });
      Utils.AssertException(null, null, (object) null, v => {
        var d = new DefaultOption("a=+-*/", null, 2);
        var s = d.GetValueSeparators();
        Assert.AreEqual(s.Length, 4);
        Assert.AreEqual(s[0], "+");
        Assert.AreEqual(s[1], "-");
        Assert.AreEqual(s[2], "*");
        Assert.AreEqual(s[3], "/");
      });
    }
  }
}