using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xnb.Exporter
{
    public static class Debug
    {
        public static void LogMessage(string message)
        {
            Console.WriteLine($"[DEBUG] {message}");

        }

        public static void LogException(string errorMessage)
        {
            DisplayErrorMessage(errorMessage);
        }

        public static void DisplayErrorMessage(string errorMessage)
        {

            MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }
    }
}

