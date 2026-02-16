using Application.Interface;

namespace Application.Implementation;

internal sealed class OddEvenProgram : IProgram
{
    private readonly AutoResetEvent _oddTurn = new(false);
    private readonly AutoResetEvent _evenTurn = new(false);

    public void Run()
    {
        var oddTask = Task.Run(PrintOddNumbers);
        var evenTask = Task.Run(PrintEvenNumbers);

        Task.WaitAll(oddTask, evenTask);
    }

    private void PrintOddNumbers()
    {
        int[] oddNumbers = [1, 3, 5, 7, 9, 11, 13, 15];

        foreach (var value in oddNumbers)
        {
            Console.WriteLine(value);
            _evenTurn.Set();
            _oddTurn.WaitOne();
        }
    }

    private void PrintEvenNumbers()
    {
        int[] evenNumbers = [2, 4, 6, 8, 10, 12, 14];

        foreach (var value in evenNumbers)
        {
            _evenTurn.WaitOne();
            Console.WriteLine(value);
            _oddTurn.Set();
        }

        _oddTurn.Set();
    }
}
