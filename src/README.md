# NDesk.Options
### A callback-based program option parser for C#.


---


|              |   Description                                                  |
|--------------|----------------------------------------------------------------|
| CREATED BY:  | Latency McLaughlin                                                     |
| COMPANY:     | Bio-Hazard Industries                                                     |
| UPDATED:     | 1/26/2024                          |
| FRAMEWORK:   |  ([Latest])                                |
| LANGUAGE:    | [C#] preview                                            |
| OUTPUT TYPE: | Library [API]                                            |
| SUPPORTS:    | [Visual Studio]                                                |
| GFX SUBSYS:  | [None]                                                     |
| TAGS:        | [C#], [.NET], [API], [Visual Studio], [MyGet], [NuGet], [NDesk.Options]                                                |
| STATUS:      | [![Appveyor Build status](https://ci.appveyor.com/api/projects/status/7v7qid5n8l9ca277/branch/master?svg=true)](https://ci.appveyor.com/project/Latency/ndesk-options)                                                    |
|              | [![MyGet Build Status](https://www.myget.org/BuildSource/Badge/NDesk.Options?identifier=8e359284-c64e-4aeb-b7c2-2122f6ad6347)](https://www.myget.org/)                                                    |
|              | [![.NET](https://github.com/Latency/NDesk.Options/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Latency/NDesk.Options/actions/workflows/dotnet.yml)                                                    |
| LICENSE:     | [![License](https://img.shields.io/badge/MIT-License-yellowgreen.svg)](https://github.com/Latency/NDesk.Options/blob/master/LICENSE)                                                    |
| VERSION:     | [![GitHub release](https://img.shields.io/github/release/Latency/NDesk.Options.svg)](https://github.com/Latency/NDesk.Options/releases) - v1.2.6                                                    |


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

[MIT LICENSE]

[//]: # (These are reference links used in the body of this note and get stripped out when the markdown processor does its job.)

   [.NET]: <https://en.wikipedia.org/wiki/.NET_Framework/>
   [Console Application]: <https://en.wikipedia.org/wiki/Console_application>
   [API]: <https://en.wikipedia.org/wiki/Application_programming_interface>
   [C#]: <https://en.wikipedia.org/wiki/C_Sharp_(programming_language)>
   [DLL]: <https://en.wikipedia.org/wiki/Dynamic-link_library>
   [Latency McLaughlin]: <https://www.linkedin.com/in/Latency/>
   [MIT License]: <http://choosealicense.com/licenses/mit/>
   [MyGet]: <https://www.myget.org/features>
   [NuGet]: <https://www.nuget.org/>
   [Visual Studio]: <https://en.wikipedia.org/wiki/Microsoft_Visual_Studio/>
