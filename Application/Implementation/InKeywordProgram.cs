using Application.Interface;
using System.Diagnostics;

namespace Application.Implementation;

internal sealed class InKeywordProgram : IProgram
{
    private const int Iterations = 5_000_000;

    public void Run()
    {
        var sample = new NumberBlock(1, 2, 3, 4, 5, 6, 7, 8);

        var byValueTimer = Stopwatch.StartNew();
        var byValueResult = 0L;
        for (var i = 0; i < Iterations; i++)
        {
            byValueResult += SumByValue(sample);
        }
        byValueTimer.Stop();

        var inTimer = Stopwatch.StartNew();
        var inResult = 0L;
        for (var i = 0; i < Iterations; i++)
        {
            inResult += SumByIn(in sample);
        }
        inTimer.Stop();

        Console.WriteLine("Demonstrating the 'in' modifier with a readonly struct parameter:");
        Console.WriteLine($"By value total: {byValueResult}, elapsed: {byValueTimer.ElapsedMilliseconds} ms");
        Console.WriteLine($"By in total: {inResult}, elapsed: {inTimer.ElapsedMilliseconds} ms");
        Console.WriteLine("'in' passes the struct by readonly reference, avoiding copies.");
    }

    private static int SumByValue(NumberBlock block) =>
        block.A + block.B + block.C + block.D + block.E + block.F + block.G + block.H;

    private static int SumByIn(in NumberBlock block) =>
        block.A + block.B + block.C + block.D + block.E + block.F + block.G + block.H;

    private readonly record struct NumberBlock(
        int A,
        int B,
        int C,
        int D,
        int E,
        int F,
        int G,
        int H);
}
