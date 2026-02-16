using Application.Interface;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Application.Implementation;

internal sealed class InlineMethodProgram : IProgram
{
    private const int Max = 100_000_000;

    public void Run()
    {
        var nonInlinedValue = Method1();
        var inlinedValue = Method2();
        var sum = 0;

        var timerWithoutInlining = Stopwatch.StartNew();
        for (var i = 0; i < Max; i++)
        {
            sum += nonInlinedValue;
        }
        timerWithoutInlining.Stop();

        sum = 0;
        var timerWithInlining = Stopwatch.StartNew();
        for (var i = 0; i < Max; i++)
        {
            sum += inlinedValue;
        }
        timerWithInlining.Stop();

        Console.WriteLine($"Method1 (no hint): {ToNanoseconds(timerWithoutInlining):0.00} ns");
        Console.WriteLine($"Method2 (AggressiveInlining): {ToNanoseconds(timerWithInlining):0.00} ns");
    }

    private static double ToNanoseconds(Stopwatch sw) => (sw.Elapsed.TotalMilliseconds * 1_000_000) / Max;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Method2() =>
        "one".Length + "two".Length + "three".Length +
        "four".Length + "five".Length + "six".Length +
        "seven".Length + "eight".Length + "nine".Length +
        "ten".Length;

    private static int Method1() =>
        "one".Length + "two".Length + "three".Length +
        "four".Length + "five".Length + "six".Length +
        "seven".Length + "eight".Length + "nine".Length +
        "ten".Length;
}
