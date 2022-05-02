using Application.Interface;

namespace Application.Implementation
{
    internal class TestProgram : IProgram
    {
        public void Run()
        {
            Console.WriteLine("Test");
        }
    }
}
