using Application.Implementation;
using Application.Interface;

internal sealed class ProgramContext
{
    private readonly IReadOnlyList<IProgram> _programs =
    [
        new OddEvenProgram(),
        new InlineMethodProgram(),
        new InKeywordProgram()
    ];

    internal void ShowAllOptions()
    {
        for (var i = 0; i < _programs.Count; i++)
        {
            Console.WriteLine($"Option {i}: {_programs[i].GetType().Name}");
        }
    }

    internal void RunWith(int option)
    {
        if (option < 0 || option >= _programs.Count)
        {
            Console.WriteLine("Option out of range.");
            return;
        }

        _programs[option].Run();
    }
}
