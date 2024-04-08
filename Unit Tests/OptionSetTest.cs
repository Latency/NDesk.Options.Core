// 04****************************************************************************
// Project:  Unit Tests
// File:     OptionSetTest.cs
// Author:   Latency McLaughlin
// Date:     04/04/2024
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using NDesk.Options;
using Xunit;

namespace Unit_Tests;

public class OptionSetTest
{
    private static IEnumerable<string> _(params string[] a) => a;

    private static void AssertDictionary<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> dict, params string[] set)
    {
        var k = TypeDescriptor.GetConverter(typeof(TKey));
        var v = TypeDescriptor.GetConverter(typeof(TValue));

        Assert.Equal(dict.Count, set.Length / 2);
        for (var i = 0; i < set.Length; i += 2)
        {
            var key = (TKey)k.ConvertFromString(set[i]);
            Assert.True(dict.ContainsKey(key));
            if (set[i + 1] == null)
                Assert.Equal(dict[key], default);
            else
                Assert.Equal(dict[key], (TValue)v.ConvertFromString(set[i + 1]));
        }
    }

    [Fact]
    public void BooleanValues()
    {
        var a = false;
        var p = new OptionSet
        {
            { "a", v => a = v != null }
        };
        p.Parse(_("-a"));
        Assert.True(a);
        p.Parse(_("-a+"));
        Assert.True(a);
        p.Parse(_("-a-"));
        Assert.False(a);
    }

    [Fact]
    public void BundledValues()
    {
        var defines = new List<string>();
        var libs    = new List<string>();
        var debug   = false;
        var p = new OptionSet
        {
            {
                "D|define=", v => defines.Add(v)
            },
            {
                "L|library:", v => libs.Add(v)
            },
            {
                "Debug", v => debug = v != null
            },
            {
                "E", v => { /* ignore */ }
            }
        };
        p.Parse(_("-DNAME", "-D", "NAME2", "-Debug", "-L/foo", "-L", "/bar", "-EDNAME3"));
        Assert.Equal(3,       defines.Count);
        Assert.Equal("NAME",  defines[0]);
        Assert.Equal("NAME2", defines[1]);
        Assert.Equal("NAME3", defines[2]);
        Assert.True(debug);
        Assert.Equal(2,      libs.Count);
        Assert.Equal("/foo", libs[0]);
        Assert.Null(libs[1]);

        Utils.AssertException(typeof(OptionException), "Cannot bundle unregistered option '-V'.", p, v =>
            {
                v.Parse(_("-EVALUENOTSUP"));
            }
        );
    }

    [Fact]
    public void CombinationPlatter()
    {
        int a = -1,
            b = -1;
        string av = null,
               bv = null;
        Foo f       = null;
        var help    = 0;
        var verbose = 0;
        var p = new OptionSet
        {
            {
                "a=", v =>
                {
                    a  = 1;
                    av = v;
                }
            },
            {
                "b", "desc", v =>
                {
                    b  = 2;
                    bv = v;
                }
            },
            {
                "f=", (Foo v) => f = v
            },
            {
                "v", v =>
                {
                    ++verbose;
                }
            },
            {
                "h|?|help", v =>
                {
                    switch (v)
                    {
                        case "h":
                            help |= 0x1;
                            break;
                        case "?":
                            help |= 0x2;
                            break;
                        case "help":
                            help |= 0x4;
                            break;
                    }
                }
            }
        };
        var e = p.Parse(
            [
                "foo",
                "-v",
                "-a=42",
                "/b-",
                "-a",
                "64",
                "bar",
                "--f",
                "B",
                "/h",
                "-?",
                "--help",
                "-v"
            ]
        );

        Assert.Equal(2,     e.Count);
        Assert.Equal("foo", e[0]);
        Assert.Equal("bar", e[1]);
        Assert.Equal(1,     a);
        Assert.Equal("64",  av);
        Assert.Equal(2,     b);
        Assert.Null(bv);
        Assert.Equal(2,   verbose);
        Assert.Equal(0x7, help);
        Assert.Equal(f,   Foo.B);
    }

    [Fact]
    public void CustomKeyValue()
    {
        var a = new Dictionary<string, string>();
        var b = new Dictionary<string, string[]>();
        var p = new OptionSet
        {
            new CustomOption("a==:", null, 2, v => a.Add(v[0], v[1])),
            new CustomOption("b==:", null, 3, v => b.Add(v[0],
                                 [
                                     v[1],
                                     v[2]
                                 ]
                             )
            )
        };
        p.Parse(_("-a=b=c", "-a=d", "e", "-a:f=g", "-a:h:i", "-a", "j=k", "-a", "l:m"));
        Assert.Equal(6,   a.Count);
        Assert.Equal("c", a["b"]);
        Assert.Equal("e", a["d"]);
        Assert.Equal("g", a["f"]);
        Assert.Equal("i", a["h"]);
        Assert.Equal("k", a["j"]);
        Assert.Equal("m", a["l"]);

        Utils.AssertException(typeof(OptionException), "Missing required value for option '-a'.", p, v =>
            {
                v.Parse(_("-a=b"));
            }
        );

        p.Parse(_("-b", "a", "b", "c", "-b:d:e:f", "-b=g=h:i", "-b:j=k:l"));
        Assert.Equal(4,   b.Count);
        Assert.Equal("b", b["a"][0]);
        Assert.Equal("c", b["a"][1]);
        Assert.Equal("e", b["d"][0]);
        Assert.Equal("f", b["d"][1]);
        Assert.Equal("h", b["g"][0]);
        Assert.Equal("i", b["g"][1]);
        Assert.Equal("k", b["j"][0]);
        Assert.Equal("l", b["j"][1]);
    }

    [Fact]
    public void DefaultHandler()
    {
        var extra = new List<string>();
        var p = new OptionSet
        {
            {
                "<>", v => extra.Add(v)
            }
        };
        var e = p.Parse(_("-a", "b", "--c=D", "E"));
        Assert.Empty(e);
        Assert.Equal(4,       extra.Count);
        Assert.Equal("-a",    extra[0]);
        Assert.Equal("b",     extra[1]);
        Assert.Equal("--c=D", extra[2]);
        Assert.Equal("E",     extra[3]);
    }

    [Fact]
    public void DefaultHandlerRuns()
    {
        var formats = new Dictionary<string, List<string>>();
        var format  = "foo";
        var p = new OptionSet
        {
            {
                "f|format=", v => format = v
            },
            {
                "<>", v =>
                {
                    if (!formats.TryGetValue(format, out var f))
                    {
                        f = [];
                        formats.Add(format, f);
                    }

                    f.Add(v);
                }
            }
        };
        var e = p.Parse(_("a", "b", "-fbar", "c", "d", "--format=baz", "e", "f"));
        Assert.Empty(e);
        Assert.Equal(3,   formats.Count);
        Assert.Equal(2,   formats["foo"].Count);
        Assert.Equal("a", formats["foo"][0]);
        Assert.Equal("b", formats["foo"][1]);
        Assert.Equal(2,   formats["bar"].Count);
        Assert.Equal("c", formats["bar"][0]);
        Assert.Equal("d", formats["bar"][1]);
        Assert.Equal(2,   formats["baz"].Count);
        Assert.Equal("e", formats["baz"][0]);
        Assert.Equal("f", formats["baz"][1]);
    }

    [Fact]
    public void DerivedType()
    {
        var help = false;
        var p = new CiOptionSet
        {
            {
                "h|help", v => help = v != null
            }
        };
        p.Parse(_("-H"));
        Assert.True(help);
        help = false;
        p.Parse(_("-HELP"));
        Assert.True(help);

        Assert.Equal(p.GetOptionForName("h"),    p[0]);
        Assert.Equal(p.GetOptionForName("help"), p[0]);

        Utils.AssertException(typeof(KeyNotFoundException), "The given key 'invalid' was not present in the dictionary.", p, v =>
            {
                p.GetOptionForName("invalid");
            }
        );

        Utils.AssertException(typeof(ArgumentException), "prototypes must be null!", p, v =>
            {
                v.Add("N|NUM=", (int n) =>
                      { }
                );
            }
        );

        Utils.AssertException(typeof(ArgumentNullException), "Value cannot be null. (Parameter 'key')", p, v =>
            {
                v.GetOptionForName(null);
            }
        );
    }

    [Fact]
    public void Exceptions()
    {
        string a = null;
        var p = new OptionSet
        {
            { "a=", v => a = v },
            { "b", v => { } },
            { "c", v => { } },
            { "n=", (int v) => { } },
            { "f=", (Foo v) => { } }
        };

        // missing argument
        Utils.AssertException(typeof(OptionException), "Missing required value for option '-a'.", p, v => { v.Parse(_("-a")); });

        // another named option while expecting one -- follow Getopt::Long
        Utils.AssertException(null, null, p, v => { v.Parse(_("-a", "-a")); });
        Assert.Equal("-a", a);

        // no exception when an unregistered named option follows.
        Utils.AssertException(null, null, p, v => { v.Parse(_("-a", "-b")); });
        Assert.Equal("-b", a);
        Utils.AssertException(typeof(ArgumentNullException), "Value cannot be null. (Parameter 'item')", p, v => { v.Add(null); });

        // bad type
        Utils.AssertException(typeof(OptionException), "Could not convert string `value' to type Int32 for option `-n'.", p, v => { v.Parse(_("-n", "value")); });
        Utils.AssertException(typeof(OptionException), "Could not convert string `invalid' to type Foo for option `--f'.", p, v => { v.Parse(_("--f", "invalid")); });

        // try to bundle with an option requiring a value
        Utils.AssertException(typeof(OptionException),       "Cannot bundle unregistered option '-z'.",                                                 p, v => { v.Parse(_("-cz", "extra")); });
        Utils.AssertException(typeof(ArgumentNullException), "Value cannot be null. (Parameter 'action')",                                              p, v => { v.Add("foo", (Action<string>)null); });
        Utils.AssertException(typeof(ArgumentException),     "Cannot provide maxValueCount of 2 for OptionValueType.None. (Parameter 'maxValueCount')", p, v => { v.Add("foo", (k, val) => { /* ignore */ }); });
    }

    [Fact]
    public void HaltProcessing()
    {
        var p = new OptionSet
        {
            {
                "a", v =>
                { }
            },
            {
                "b", v =>
                { }
            }
        };
        var e = p.Parse(_("-a", "-b", "--", "-a", "-b"));
        Assert.Equal(2,    e.Count);
        Assert.Equal("-a", e[0]);
        Assert.Equal("-b", e[1]);
    }

    [Fact]
    public void KeyValueOptions()
    {
        var a = new Dictionary<string, string>();
        var b = new Dictionary<int, char>();
        var p = new OptionSet
        {
            {
                "a=", (k, v) => a.Add(k, v)
            },
            {
                "b=", (int k, char v) => b.Add(k, v)
            },
            {
                "c:", (k, v) =>
                {
                    if (k != null)
                        a.Add(k, v);
                }
            },
            {
                "d={=>}{-->}", (k, v) => a.Add(k, v)
            },
            {
                "e={}", (k, v) => a.Add(k, v)
            },
            {
                "f=+/", (k, v) => a.Add(k, v)
            }
        };
        p.Parse(_("-a", "A", "B", "-a", "C", "D", "-a=E=F", "-a:G:H", "-aI=J", "-b", "1", "a", "-b", "2", "b"));
        AssertDictionary(a, "A", "B", "C", "D", "E", "F", "G", "H", "I", "J");
        AssertDictionary(b, "1", "a", "2", "b");

        a.Clear();
        p.Parse(_("-c"));
        Assert.Empty(a);
        p.Parse(_("-c", "a"));
        Assert.Empty(a);
        p.Parse(_("-ca"));
        AssertDictionary(a, "a", null);
        a.Clear();
        p.Parse(_("-ca=b"));
        AssertDictionary(a, "a", "b");

        a.Clear();
        p.Parse(_("-dA=>B", "-d", "C-->D", "-d:E", "F", "-d", "G", "H", "-dJ-->K"));
        AssertDictionary(a, "A", "B", "C", "D", "E", "F", "G", "H", "J", "K");

        a.Clear();
        p.Parse(_("-eA=B", "-eC=D", "-eE", "F", "-e:G", "H"));
        AssertDictionary(a, "A=B", "-eC=D", "E", "F", "G", "H");

        a.Clear();
        p.Parse(_("-f1/2", "-f=3/4", "-f:5+6", "-f7", "8", "-f9=10", "-f11=12"));
        AssertDictionary(a, "1", "2", "3", "4", "5", "6", "7", "8", "9=10", "-f11=12");
    }

    [Fact]
    public void Localization()
    {
        var p = new OptionSet(f => "hello!")
        {
            {
                "n=", (int v) =>
                { }
            }
        };
        Utils.AssertException(typeof(OptionException), "hello!", p, v =>
            {
                v.Parse(_("-n=value"));
            }
        );

        var expected = new StringWriter();
        expected.WriteLine("  -nhello!                   hello!");

        var actual = new StringWriter();
        p.WriteOptionDescriptions(actual);

        Assert.Equal(actual.ToString(), expected.ToString());
    }

    [Fact]
    public void MixedDefaultHandler()
    {
        var tests = new List<string>();
        var p = new OptionSet
        {
            {
                "t|<>=", v => tests.Add(v)
            }
        };
        var e = p.Parse(_("-tA", "-t:B", "-t=C", "D", "--E=F"));
        Assert.Empty(e);
        Assert.Equal(5,       tests.Count);
        Assert.Equal("A",     tests[0]);
        Assert.Equal("B",     tests[1]);
        Assert.Equal("C",     tests[2]);
        Assert.Equal("D",     tests[3]);
        Assert.Equal("--E=F", tests[4]);
    }

    [Fact]
    public void OptionalValues()
    {
        string a = null;
        var    n = -1;
        Foo    f = null;
        var p = new OptionSet
        {
            {
                "a:", v => a = v
            },
            {
                "n:", (int v) => n = v
            },
            {
                "f:", (Foo v) => f = v
            }
        };
        p.Parse(_("-a=s"));
        Assert.Equal("s", a);
        p.Parse(_("-a"));
        Assert.Null(a);
        p.Parse(_("-a="));
        Assert.Equal("", a);

        p.Parse(_("-f", "A"));
        Assert.Null(f);
        p.Parse(_("-f"));
        Assert.Null(f);
        p.Parse(_("-f=A"));
        Assert.Equal(f, Foo.A);
        f = null;
        p.Parse(_("-fA"));
        Assert.Equal(f, Foo.A);

        p.Parse(_("-n42"));
        Assert.Equal(42, n);
        p.Parse(_("-n", "42"));
        Assert.Equal(0, n);
        p.Parse(_("-n=42"));
        Assert.Equal(42, n);
        p.Parse(_("-n"));
        Assert.Equal(0, n);
    }

    [Fact]
    public void OptionBundling()
    {
        string b,
               c,
               f;
        var a = b = c = f = null;
        var p = new OptionSet
        {
            {
                "a", v => a = "a"
            },
            {
                "b", v => b = "b"
            },
            {
                "c", v => c = "c"
            },
            {
                "f=", v => f = v
            }
        };
        var extra = p.Parse(_("-abcf", "foo", "bar"));
        Assert.Single(extra);
        Assert.Equal("bar", extra[0]);
        Assert.Equal("a",   a);
        Assert.Equal("b",   b);
        Assert.Equal("c",   c);
        Assert.Equal("foo", f);
    }

    [Fact]
    public void OptionContext()
    {
        var p = new OptionSet
        {
            new ContextCheckerOption("a=", "a desc", "/a",   "a-val", 1),
            new ContextCheckerOption("b",  "b desc", "--b+", "--b+",  2),
            new ContextCheckerOption("c=", "c desc", "--c",  "C",     3),
            new ContextCheckerOption("d",  "d desc", "/d-",  null,    4)
        };
        Assert.Equal(4, p.Count);
        p.Parse(_("/a", "a-val", "--b+", "--c=C", "/d-"));
    }

    [Fact]
    public void OptionParts()
    {
        var p = new CiOptionSet();
        p.CheckOptionParts("A",       false, null, null, null, null);
        p.CheckOptionParts("A=B",     false, null, null, null, null);
        p.CheckOptionParts("-A=B",    true,  "-",  "A",  "=",  "B");
        p.CheckOptionParts("-A:B",    true,  "-",  "A",  ":",  "B");
        p.CheckOptionParts("--A=B",   true,  "--", "A",  "=",  "B");
        p.CheckOptionParts("--A:B",   true,  "--", "A",  ":",  "B");
        p.CheckOptionParts("/A=B",    true,  "/",  "A",  "=",  "B");
        p.CheckOptionParts("/A:B",    true,  "/",  "A",  ":",  "B");
        p.CheckOptionParts("-A=B=C",  true,  "-",  "A",  "=",  "B=C");
        p.CheckOptionParts("-A:B=C",  true,  "-",  "A",  ":",  "B=C");
        p.CheckOptionParts("-A:B:C",  true,  "-",  "A",  ":",  "B:C");
        p.CheckOptionParts("--A=B=C", true,  "--", "A",  "=",  "B=C");
        p.CheckOptionParts("--A:B=C", true,  "--", "A",  ":",  "B=C");
        p.CheckOptionParts("--A:B:C", true,  "--", "A",  ":",  "B:C");
        p.CheckOptionParts("/A=B=C",  true,  "/",  "A",  "=",  "B=C");
        p.CheckOptionParts("/A:B=C",  true,  "/",  "A",  ":",  "B=C");
        p.CheckOptionParts("/A:B:C",  true,  "/",  "A",  ":",  "B:C");
        p.CheckOptionParts("-AB=C",   true,  "-",  "AB", "=",  "C");
        p.CheckOptionParts("-AB:C",   true,  "-",  "AB", ":",  "C");
    }

    [Fact]
    public void RequiredValues()
    {
        string a = null;
        var    n = 0;
        var p = new OptionSet
        {
            {
                "a=", v => a = v
            },
            {
                "n=", (int v) => n = v
            }
        };
        var extra = p.Parse(_("a", "-a", "s", "-n=42", "n"));
        Assert.Equal(2,   extra.Count);
        Assert.Equal("a", extra[0]);
        Assert.Equal("n", extra[1]);
        Assert.Equal("s", a);
        Assert.Equal(42,  n);

        extra = p.Parse(_("-a="));
        Assert.Empty(extra);
        Assert.Equal("", a);
    }

    [Fact]
    public void WriteOptionDescriptions()
    {
        var p = new OptionSet
        {
            {
                "p|indicator-style=", "append / indicator to directories", v => { }
            },
            {
                "color:", "controls color info", v => { }
            },
            {
                "color2:", "set {color}", v => { }
            },
            {
                "rk=", "required key/value option", (k, v) => { }
            },
            {
                "rk2=", "required {{foo}} {0:key}/{1:value} option", (k, v) => { }
            },
            {
                "ok:", "optional key/value option", (k, v) => { }
            },
            {
                "long-desc",

                                "This has a really\nlong, multi-line description that also\ntests\n" +
                                "the-builtin-supercalifragilisticexpialidicious-break-on-hyphen.  "  + 
                                "Also, a list:\n"                                                    +
                                "  item 1\n"                                                         +
                                "  item 2",
                                v => { }
            },
            {
                "long-desc2", "IWantThisDescriptionToBreakInsideAWordGeneratingAutoWordHyphenation.", v => { }
            },
            {
                "h|?|help", "show help text", v => { }
            },
            {
                "version", "output version information and exit", v => { }
            },
            {
                "<>", v => { }
            }
        };

        var expected = new StringWriter();
        expected.WriteLine("  -p, --indicator-style=VALUE");
        expected.WriteLine("                             append / indicator to directories");
        expected.WriteLine("      --color[=VALUE]        controls color info");
        expected.WriteLine("      --color2[=color]       set color");
        expected.WriteLine("      --rk=VALUE1:VALUE2     required key/value option");
        expected.WriteLine("      --rk2=key:value        required {foo} key/value option");
        expected.WriteLine("      --ok[=VALUE1:VALUE2]   optional key/value option");
        expected.WriteLine("      --long-desc            This has a really");
        expected.WriteLine("                               long, multi-line description that also");
        expected.WriteLine("                               tests");
        expected.WriteLine("                               the-builtin-supercalifragilisticexpialidicious-");
        expected.WriteLine("                               break-on-hyphen.  Also, a list:");
        expected.WriteLine("                                 item 1");
        expected.WriteLine("                                 item 2");
        expected.WriteLine("      --long-desc2           IWantThisDescriptionToBreakInsideAWordGenerating-");
        expected.WriteLine("                               AutoWordHyphenation.");
        expected.WriteLine("  -h, -?, --help             show help text");
        expected.WriteLine("      --version              output version information and exit");

        var actual = new StringWriter();
        p.WriteOptionDescriptions(actual);

        Assert.Equal(actual.ToString(), expected.ToString());
    }

    private class CustomOption(string p, string d, int c, Action<OptionValueCollection> a) : Option(p, d, c)
    {
        protected override void OnParseComplete(OptionContext c) => a(c.OptionValues);
    }

    private class CiOptionSet : OptionSet
    {
        protected override void InsertItem(int index, Option item)
        {
            if (item.Prototype.ToLower() != item.Prototype)
                throw new ArgumentException("prototypes must be null!");
            base.InsertItem(index, item);
        }

        protected override bool Parse(string option, OptionContext c) => c.Option != null ? base.Parse(option, c) : !GetOptionParts(option, out var f, out var n, out var s, out var v) ? base.Parse(option, c) : base.Parse(f + n.ToLower() + (v != null && s != null ? s + v : ""), c);

        public Option GetOptionForName(string n) => base[n];

        public void CheckOptionParts(string option, bool er, string ef, string en, string es, string ev)
        {
            var r = GetOptionParts(option, out var f, out var n, out var s, out var v);
            Assert.Equal(r, er);
            Assert.Equal(f, ef);
            Assert.Equal(n, en);
            Assert.Equal(s, es);
            Assert.Equal(v, ev);
        }
    }

    private class ContextCheckerOption(string p, string d, string eName, string eValue, int index) : Option(p, d)
    {
        protected override void OnParseComplete(OptionContext c)
        {
            Assert.Single(c.OptionValues);
            Assert.Equal(c.OptionValues[0],    eValue);
            Assert.Equal(c.OptionName,         eName);
            Assert.Equal(c.OptionIndex,        index);
            Assert.Equal(c.Option,             this);
            Assert.Equal(c.Option.Description, Description);
        }
    }
}