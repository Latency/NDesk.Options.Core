// 04****************************************************************************
// Project:  Unit Tests
// File:     Utils.cs
// Author:   Latency McLaughlin
// Date:     04/04/2024
// ****************************************************************************

namespace Unit_Tests;

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

        if (actualType != exception)
            throw new InvalidOperationException($"Assertion failed: Expected Exception Type {exception}, got {actualType}.{Environment.NewLine}Actual Exception: {stack}");

        if (actualMessage != message)
            throw new InvalidOperationException($"Assertion failed:{Environment.NewLine}\tExpected: {message}{Environment.NewLine}\t  Actual: {actualMessage}");
    }
}