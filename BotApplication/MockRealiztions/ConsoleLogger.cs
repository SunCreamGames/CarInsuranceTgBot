using Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MockRealiztions
{
    public class ConsoleLogger : ILogger
    {
        public void LogError(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"Error : ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(error);
        }

        public void LogMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
