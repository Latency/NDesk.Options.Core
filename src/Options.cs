// 1****************************************************************************
// Project:  NDesk.Options
// File:     Options.cs
// Author:   Latency McLaughlin
// Date:     1/21/2024
// ****************************************************************************

//
// A Getopt::Long-inspired option parsing library for C#.
//
// NDesk.Options.OptionSet is built upon a key/value table, where the
// key is a option format string and the value is a delegate that is 
// invoked when the format string is matched.
//
// Option format strings:
//  Regex-like BNF Grammar: 
//    name: .+
//    type: [=:]
//    sep: ( [^{}]+ | '{' .+ '}' )?
//    aliases: ( name type sep ) ( '|' name type sep )*
// 
// Each '|'-delimited name is an alias for the associated action.  If the
// format string ends in a '=', it has a required value.  If the format
// string ends in a ':', it has an optional value.  If neither '=' or ':'
// is present, no value is supported.  `=' or `:' need only be defined on one
// alias, but if they are provided on more than one they must be consistent.
//
// Each alias portion may also end with a "key/value separator", which is used
// to split option values if the option accepts > 1 value.  If not specified,
// it defaults to '=' and ':'.  If specified, it can be any character except
// '{' and '}' OR the *string* between '{' and '}'.  If no separator should be
// used (i.e. the separate values should be distinct arguments), then "{}"
// should be used as the separator.
//
// Options are extracted either from the current option by looking for
// the option name followed by an '=' or ':', or is taken from the
// following option IFF:
//  - The current option does not contain a '=' or a ':'
//  - The current option requires a value (i.e. not a Option type of ':')
//
// The `name' used in the option format string does NOT include any leading
// option indicator, such as '-', '--', or '/'.  All three of these are
// permitted/required on any named option.
//
// Option bundling is permitted so long as:
//   - '-' is used to start the option group
//   - all of the bundled options are a single character
//   - at most one of the bundled options accepts a value, and the value
//     provided starts from the next character to the end of the string.
//
// This allows specifying '-a -b -c' as '-abc', and specifying '-D name=value'
// as '-Dname=value'.
//
// Option processing is disabled by specifying "--".  All options after "--"
// are returned by OptionSet.Parse() unchanged and unprocessed.
//
// Unprocessed options are returned from OptionSet.Parse().
//
// Examples:
//  int verbose = 0;
//  OptionSet p = new OptionSet ()
//    .Add ("v", v => ++verbose)
//    .Add ("name=|value=", v => Console.WriteLine (v));
//  p.Parse (new string[]{"-v", "--v", "/v", "-name=A", "/name", "B", "extra"});
//
// The above would parse the argument string array, and would invoke the
// lambda expression three times, setting `verbose' to 3 when complete.  
// It would also print out "A" and "B" to standard output.
// The returned array would contain the string "extra".
//
// C# 3.0 collection initializers are supported and encouraged:
//  var p = new OptionSet () {
//    { "h|?|help", v => ShowHelp () },
//  };
//
// System.ComponentModel.TypeConverter is also supported, allowing the use of
// custom data types in the callback type; TypeConverter.ConvertFromString()
// is used to convert the value option to an instance of the specified
// type:
//
//  var p = new OptionSet () {
//    { "foo=", (Foo f) => Console.WriteLine (f.ToString ()) },
//  };
//
// Random other tidbits:
//  - Boolean options (those w/o '=' or ':' in the option format string)
//    are explicitly enabled if they are followed with '+', and explicitly
//    disabled if they are followed with '-':
//      string a = null;
//      var p = new OptionSet () {
//        { "a", s => a = s },
//      };
//      p.Parse (new string[]{"-a"});   // sets v != null
//      p.Parse (new string[]{"-a+"});  // sets v != null
//      p.Parse (new string[]{"-a-"});  // sets v == null
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
#if LINQ
using System.Linq;
#endif

#if TEST
using NDesk.Options;
#endif

namespace NDesk.Options
{
    public class OptionValueCollection : IList, IList<string>
    {
        private readonly OptionContext c;

        private readonly List<string> values = [];

        internal OptionValueCollection(OptionContext c) => this.c = c;

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator() => values.GetEnumerator();

        #endregion

        #region IEnumerable<T>

        public IEnumerator<string> GetEnumerator() => values.GetEnumerator();

        #endregion

        public List<string> ToList() => [..values];

        public string[] ToArray() => values.ToArray();

        public override string ToString() => string.Join(", ", [.. values]);

        #region ICollection

        void ICollection.CopyTo(Array array, int index) => (values as ICollection).CopyTo(array, index);

        bool ICollection.  IsSynchronized => (values as ICollection).IsSynchronized;
        object ICollection.SyncRoot       => (values as ICollection).SyncRoot;

        #endregion

        #region ICollection<T>

        public void Add(string item) => values.Add(item);

        public void Clear() => values.Clear();

        public bool Contains(string item) => values.Contains(item);

        public void CopyTo(string[] array, int arrayIndex) => values.CopyTo(array, arrayIndex);

        public bool Remove(string item) => values.Remove(item);
        public int  Count               => values.Count;
        public bool IsReadOnly          => false;

        #endregion

        #region IList

        int IList. Add(object      value) => (values as IList).Add(value);
        bool IList.Contains(object value) => (values as IList).Contains(value);
        int IList. IndexOf(object  value) => (values as IList).IndexOf(value);

        void IList.Insert(int index, object value) => (values as IList).Insert(index, value);

        void IList.Remove(object value) => (values as IList).Remove(value);

        void IList.RemoveAt(int index) => (values as IList).RemoveAt(index);

        bool IList.IsFixedSize => false;

        object IList.this[int index]
        {
            get => this[index];
            set => (values as IList)[index] = value;
        }

        #endregion

        #region IList<T>

        public int IndexOf(string item) => values.IndexOf(item);

        public void Insert(int index, string item) => values.Insert(index, item);

        public void RemoveAt(int index) => values.RemoveAt(index);

        private void AssertValid(int index)
        {
            if (c.Option == null)
                throw new InvalidOperationException("OptionContext.Option is null.");
            if (index >= c.Option.MaxValueCount)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (c.Option.OptionValueType == OptionValueType.Required && index >= values.Count)
                throw new OptionException(string.Format(c.OptionSet.MessageLocalizer("Missing required value for option '{0}'."), c.OptionName), c.OptionName);
        }

        public string this[int index]
        {
            get
            {
                AssertValid(index);
                return index >= values.Count ? null : values[index];
            }
            set => values[index] = value;
        }

        #endregion
    }

    public class OptionContext
    {
        public OptionContext(OptionSet set)
        {
            OptionSet    = set;
            OptionValues = new(this);
        }

        public Option Option { get; set; }

        public string OptionName { get; set; }

        public int OptionIndex { get; set; }

        public OptionSet OptionSet { get; }

        public OptionValueCollection OptionValues { get; }
    }

    public enum OptionValueType
    {
        None,
        Optional,
        Required
    }

    public abstract class Option
    {
        private static readonly char[] NameTerminator =
        [
            '=',
            ':'
        ];

        protected Option(string prototype, string description) : this(prototype, description, 1)
        { }

        protected Option(string prototype, string description, int maxValueCount)
        {
            if (prototype == null)
                throw new ArgumentNullException(nameof(prototype));
            if (prototype.Length == 0)
                throw new ArgumentException("Cannot be the empty string.", nameof(prototype));
            if (maxValueCount < 0)
                throw new ArgumentOutOfRangeException(nameof(maxValueCount));

            Prototype       = prototype;
            Names           = prototype.Split('|');
            Description     = description;
            MaxValueCount   = maxValueCount;
            OptionValueType = ParsePrototype();

            if (MaxValueCount == 0 && OptionValueType != OptionValueType.None)
                throw new ArgumentException("Cannot provide maxValueCount of 0 for OptionValueType.Required or " + "OptionValueType.Optional.", nameof(maxValueCount));
            if (OptionValueType == OptionValueType.None && maxValueCount > 1)
                throw new ArgumentException($"Cannot provide maxValueCount of {maxValueCount} for OptionValueType.None.", nameof(maxValueCount));
            if (Array.IndexOf(Names, "<>") >= 0 && ((Names.Length == 1 && OptionValueType != OptionValueType.None) || (Names.Length > 1 && MaxValueCount > 1)))
                throw new ArgumentException("The default option handler '<>' cannot require values.", nameof(prototype));
        }

        public string Prototype { get; }

        public string Description { get; }

        public OptionValueType OptionValueType { get; }

        public int MaxValueCount { get; }

        internal string[] Names { get; }

        internal string[] ValueSeparators { get; private set; }

        public string[] GetNames() => (string[])Names.Clone();

        public string[] GetValueSeparators() => ValueSeparators == null ? [] : (string[])ValueSeparators.Clone();

        protected static T Parse<T>(string value, OptionContext c)
        {
            var conv = TypeDescriptor.GetConverter(typeof(T));
            T   t    = default;
            try
            {
                if (value != null)
                    t = (T)conv.ConvertFromString(value);
            }
            catch (Exception e)
            {
                throw new OptionException(string.Format(c.OptionSet.MessageLocalizer("Could not convert string `{0}' to type {1} for option `{2}'."), value, typeof(T).Name, c.OptionName), c.OptionName, e);
            }

            return t;
        }

        private OptionValueType ParsePrototype()
        {
            var type = '\0';
            var seps = new List<string>();
            for (var i = 0; i < Names.Length; ++i)
            {
                var name = Names[i];
                if (name.Length == 0)
                    throw new ArgumentException("Empty option names are not supported.", "prototype");

                var end = name.IndexOfAny(NameTerminator);
                if (end == -1)
                    continue;
                Names[i] = name[..end];
                type = type == '\0' || type == name[end]
                           ? name[end]
                           : throw new ArgumentException($"Conflicting option types: '{type}' vs. '{name[end]}'.", "prototype");
                AddSeparators(name, end, seps);
            }

            if (type == '\0')
                return OptionValueType.None;

            ValueSeparators = MaxValueCount switch
            {
                <= 1 when seps.Count != 0 => throw new ArgumentException($"Cannot provide key/value separators for Options taking {MaxValueCount} value(s).", "prototype"),
                > 1                       => seps.Count == 0 ? [":", "="] : seps.Count == 1 && seps[0].Length == 0 ? null : [.. seps],
                _                         => ValueSeparators
            };

            return type == '=' ? OptionValueType.Required : OptionValueType.Optional;
        }

        private static void AddSeparators(string name, int end, ICollection<string> seps)
        {
            var start = -1;
            for (var i = end + 1; i < name.Length; ++i)
                switch (name[i])
                {
                    case '{':
                        if (start != -1)
                            throw new ArgumentException($"Ill-formed name/value separator found in \"{name}\".", nameof(name));
                        start = i + 1;
                        break;
                    case '}':
                        if (start == -1)
                            throw new ArgumentException($"Ill-formed name/value separator found in \"{name}\".", nameof(name));
                        seps.Add(name.Substring(start, i - start));
                        start = -1;
                        break;
                    default:
                        if (start == -1)
                            seps.Add(name[i].ToString());
                        break;
                }

            if (start != -1)
                throw new ArgumentException($"Ill-formed name/value separator found in \"{name}\".", nameof(name));
        }

        public void Invoke(OptionContext c)
        {
            OnParseComplete(c);
            c.OptionName = null;
            c.Option     = null;
            c.OptionValues.Clear();
        }

        protected abstract void OnParseComplete(OptionContext c);

        public override string ToString() => Prototype;
    }



    public delegate void OptionAction<in TKey, in TValue>(TKey key, TValue value);
}