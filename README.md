# NDesk.Options
### An option parser for C#.

---

* CREATED BY: Latency McLaughlin
* FRAMEWORK:  .NET 4.7, .NET Core 2.0, .NET Standard 2.0
* SUPPORTS:   Visual Studio 2017, 2015, 2013, 2012, 2010, 2008
* UPDATED:    9/21/2017
* TAGS:       C# NDesk.Options API
* VERSION:    v0.2.2

<hr>

## Navigation
* <a href="#introduction">Introduction</a>
* <a href="#history">History</a>
* <a href="#solution">Solution</a>
* <a href="#usage">Usage</a>
* <a href="#installation">Installation</a>
* <a href="#license">License</a>

<hr>

<h2><a name="introduction">Introduction</a></h2>

It takes advantage of C# 3.0 features such as collection initializers and
lambda delegates to provide a short, concise specification of the option 
names to parse, whether or not those options support values, and what to do 
when the option is encountered.  It's entirely callback based:

<h2><a name="history">History</a></h2>

See:  <a href="http://www.ndesk.org/Options">http://www.ndesk.org/Options</a>

<h2><a name="usage">Usage</a></h2>

```csharp
var verbose = 0;
var show_help = false;
var names = new List<string> ();

var p = new OptionSet () {
  { "v|verbose", v => { if (v != null) ++verbose; } },
  { "h|?|help",  v => { show_help = v != null; } },
  { "n|name=",   v => { names.Add (v); } },
};
```

However, C# 3.0 features are not required, and can be used with C# 2.0:

```csharp
int          verbose   = 0;
bool         show_help = false;
List<string> names     = new List<string> ();

OptionSet p = new OptionSet ()
  .Add ("v|verbose", delegate (string v) { if (v != null) ++verbose; })
  .Add ("h|?|help",  delegate (string v) { show_help = v != null; })
  .Add ("n|name=",   delegate (string v) { names.Add (v); });
```

<h2><a name="installation">Installation</a></h2>

This library can be installed using NuGet:

<pre>
Enter the details:
Name:    NDesk.Options
Source:  nuget.org
</pre>

<h2><a name="license">License</a></h2>

[MIT LICENSE]

[//]: # (These are reference links used in the body of this note and get stripped out when the markdown processor does its job.)

   [MIT LICENSE]: <https://opensource.org/licenses/MIT>
