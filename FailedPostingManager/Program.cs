using System;

namespace FailedPostingManager
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var manager = new Manager();
            manager.process();
        }
    }
}
