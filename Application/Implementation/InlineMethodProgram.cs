using Application.Interface;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Application.Implementation
{
    internal class InlineMethodProgram : IProgram
    {
        const int _max = 100000000;

        public void Run()
        {
            var a = Method1();
            var b = Method2();
            int sum = 0;

            var s1 = Stopwatch.StartNew();
            for (int i = 0; i < _max; i++)
            {
                sum += a;
            }
            s1.Stop();

            sum = 0;
            var s2 = Stopwatch.StartNew();
            for (int i = 0; i < _max; i++)
            {
                sum += b;
            }
            s2.Stop();

            Console.WriteLine(((double)(s1.Elapsed.TotalMilliseconds * 1000000) / _max).ToString("0.00 ns"));
            Console.WriteLine(((double)(s2.Elapsed.TotalMilliseconds * 1000000) / _max).ToString("0.00 ns"));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int Method2()
        {
            // ... Aggressive inlining.
            return "one".Length + "two".Length + "three".Length +
                "four".Length + "five".Length + "six".Length +
                "seven".Length + "eight".Length + "nine".Length +
                "ten".Length;
        }
                
        private int Method1()
        {
            // ... No inlining suggestion.
            return "one".Length + "two".Length + "three".Length +
                "four".Length + "five".Length + "six".Length +
                "seven".Length + "eight".Length + "nine".Length +
                "ten".Length;
        }
    }
}
