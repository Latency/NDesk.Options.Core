// 04****************************************************************************
// Project:  NDesk.Options.Core
// File:     OptionAction.cs
// Author:   Latency McLaughlin
// Date:     04/04/2024
// ****************************************************************************

namespace NDesk.Options;

public delegate void OptionAction<in TKey, in TValue>(TKey key, TValue value);