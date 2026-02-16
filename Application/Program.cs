var programContext = new ProgramContext();
programContext.ShowAllOptions();

Console.Write("Enter option: ");
var input = Console.ReadLine();

if (!int.TryParse(input, out var option))
{
    Console.WriteLine("Invalid option. Please enter a number shown in the menu.");
    return;
}

programContext.RunWith(option);
