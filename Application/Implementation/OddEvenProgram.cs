using Application.Interface;

namespace Application.Implementation
{
    internal class OddEvenProgram : IProgram
    {
        readonly AutoResetEvent event1 = new(false);
        readonly AutoResetEvent event2 = new(false);

        public void Run()
        {
            var t1 = Task.Factory.StartNew(() => PrintOddNumbers());
            var t2 = Task.Factory.StartNew(() => PrintEvenNumbers());

            Task.WaitAny(t1, t2);
        }

        private void PrintOddNumbers()
        {
            int[] arr = new int[] { 1, 3, 5, 7, 9, 11, 13, 15 };
            foreach (var item in arr)
            {
                Console.WriteLine(item);
                event2.Set();
                event1.WaitOne();
            }
        }

        private void PrintEvenNumbers()
        {
            int[] arr = new int[] { 2, 4, 6, 8, 10, 12, 14 };
            foreach (var item in arr)
            {
                event2.WaitOne();
                Console.WriteLine(item);
                event1.Set();
            }
            event1.Set();
        }
    }
}
