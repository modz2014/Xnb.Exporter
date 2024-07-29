namespace Xnb.Exporter
{
    public static class Debug
    {
        /**
        * @brief Logs a debug message to the console.
        *
        * This method writes the specified message to the console with a "[DEBUG]" prefix.
        *
        * @param message The message to be logged to the console.
        */
        public static void LogMessage(string message)
        {
            Console.WriteLine($"[DEBUG] {message}");

        }

        /**
        * @brief Logs an exception by displaying an error message.
        *
        * This method calls DisplayErrorMessage to show an error message to the user.
        *
        * @param errorMessage The error message to be displayed.
        */
        public static void LogException(string errorMessage)
        {
            DisplayErrorMessage(errorMessage);
        }

        /**
        * @brief Displays an error message in a message box.
        *
        * This method shows a message box with the provided error message and an error icon.
        *
        * @param errorMessage The error message to be displayed in the message box.
        */
        public static void DisplayErrorMessage(string errorMessage)
        {

            MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }
    }
}

