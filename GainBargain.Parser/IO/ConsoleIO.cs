using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GainBargain.Parser.IO
{
    internal static class ConsoleIO
    {
        /// <summary>
        /// User has to input an integer and the method returns it
        /// </summary>
        public static int AskInt(string message = null, Predicate<int> criterium = null)
        {
            int result = 0;
            while (true)
            {
                int intResult = 0;
                AskString(
                    message: message,
                    canEmpty: false,
                    criterium: str =>
                    {
                        bool parsed = Int32.TryParse(str, out intResult);

                        return parsed;
                    });
                result = intResult;


                if ((criterium != null && criterium(result)) || criterium == null)
                {
                    return result;
                }
            }
        }


        /// <summary>
        /// User inputs a string and the method returns it
        /// </summary>
        public static string AskString(string message = null, bool canEmpty = false, Predicate<string> criterium = null)
        {
            string result;
            // Ask while the string is not empty
            while (true)
            {
                // If there is a message
                if (message != null)
                {
                    // Display it
                    Console.WriteLine(message);
                }
                result = Console.ReadLine();
                if (canEmpty || (!canEmpty && !String.IsNullOrWhiteSpace(result)))
                {
                    if (criterium == null || (criterium != null && criterium(result)))
                    {
                        return result;
                    }
                }
            }

        }
    }
}
