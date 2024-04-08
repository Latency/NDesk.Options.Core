# NDesk.Options.Core
### A callback-based program option parser for C#.


---


|              |   Description                                                  |
|--------------|----------------------------------------------------------------|
| CREATED BY:  | Latency McLaughlin                                                     |
| UPDATED:     | 4/8/2024                          |
| FRAMEWORK:   | [net452], [netstandard2.0], [netstandard2.1] ([Latest])                                |
| LANGUAGE:    | [C#] preview                                            |
| OUTPUT TYPE: | Library [API]                                            |
| SUPPORTS:    | [Visual Studio]                                                |
| GFX SUBSYS:  | [None]                                                     |
| TAGS:        | [NDesk.Options C#]                                                |
| STATUS       | [![.NET](https://github.com/Latency/NDesk.Options.Core/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Latency/NDesk.Options.Core/actions/workflows/dotnet.yml)                                                     |
| LICENSE:     | [![License](https://img.shields.io/badge/MIT-License-yellowgreen.svg)](https://github.com/Latency/NDesk.Options.Core/blob/master/MIT-LICENSE.txt)                                                    |
| VERSION:     | [![GitHub Release](https://img.shields.io/badge/Version-1.2.7-blue)](https://github.com/Latency/NDesk.Options.Core/releases)                                                    |


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

It takes advantage of C# features such as collection initializers and
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

This library can be installed using NuGet found [here](https://www.myget.org/feed/Packages/ndesk-options).

<h2><a name="license">License</a></h2>

The source code for the site is licensed under the MIT license, which you can find in
the [MIT-LICENSE].txt file.

All graphical assets are licensed under the
[Creative Commons Attribution 3.0 Unported License](https://creativecommons.org/licenses/by/3.0/).

[//]: # (These are reference links used in the body of this note and get stripped out when the markdown processor does its job.)

   [.NET]: <https://en.wikipedia.org/wiki/.NET_Framework/>
   [Console Application]: <https://en.wikipedia.org/wiki/Console_application>
   [API]: <https://en.wikipedia.org/wiki/Application_programming_interface>
   [C#]: <https://en.wikipedia.org/wiki/C_Sharp_(programming_language)>
   [DLL]: <https://en.wikipedia.org/wiki/Dynamic-link_library>
   [Latency McLaughlin]: <https://www.linkedin.com/in/Latency/>
   [MIT-License]: <http://choosealicense.com/licenses/mit/>
   [NuGet]: <https://www.nuget.org/>
   [NDesk.Options.Core]: <https://github.com/Latency/NDesk.Options.Core/>
   [Visual Studio]: <https://en.wikipedia.org/wiki/Microsoft_Visual_Studio/>
