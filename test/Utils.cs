// 1****************************************************************************
// Project:  Tests
// File:     Utils.cs
// Author:   Latency McLaughlin
// Date:     1/26/2024
// ****************************************************************************

using System;

namespace Tests;

internal static class Utils
{
    public static void AssertException<T>(Type exception, string message, T a, Action<T> action)
    {
        Type   actualType    = null;
        string stack         = null;
        string actualMessage = null;
        try
        {
            action(a);
        }
        catch (Exception e)
        {
            actualType    = e.GetType();
            actualMessage = e.Message;
            if (!(actualType == exception))
                stack = e.ToString();
        }

        if (!(actualType == exception))
            throw new InvalidOperationException($"Assertion failed: Expected Exception Type {exception}, got {actualType}.\n" + $"Actual Exception: {stack}");
        if (actualMessage != message)
            throw new InvalidOperationException($"Assertion failed:\n\tExpected: {message}\n\t  Actual: {actualMessage}");
    }
}