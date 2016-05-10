﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

// Add a using directive and a reference for System.Net.Http;
using System.Net.Http;

namespace AsyncTracer
{
    public partial class MainWindow2 : Window
    {
        public MainWindow2()
        {
            InitializeComponent();
        }

        private async void startButton_Click(object sender, RoutedEventArgs e)
        {
            // Add dumb comments
            // The display lines in the example lead you through the control shifts.
            resultsTextBox.Text += "ONE:   Entering startButton_Click at thread \r\n" + System.Threading.Thread.CurrentThread.GetHashCode() +
                "           Calling AccessTheWebAsync.\r\n";
            Task<int> getLengthTask = AccessTheWebAsync();

            resultsTextBox.Text += "\r\nFOUR:  Back in startButton_Click at thread \r\n" + System.Threading.Thread.CurrentThread.GetHashCode() +
                "           Task getLengthTask is started.\r\n" +
                "           About to await getLengthTask -- no caller to return to.\r\n";

            int contentLength = await getLengthTask;

            resultsTextBox.Text += "\r\nSIX:   Back in startButton_Click at thread \r\n" + System.Threading.Thread.CurrentThread.GetHashCode() +
                "           Task getLengthTask is finished.\r\n" +
                "           Result from AccessTheWebAsync is stored in contentLength.\r\n" +
                "           About to display contentLength and exit.\r\n";

            resultsTextBox.Text +=
                String.Format("\r\nLength of the downloaded string: {0}.\r\n", contentLength); 
        }


        async Task<int> AccessTheWebAsync()
        {
            resultsTextBox.Text += "\r\nTWO:   Entering AccessTheWebAsync at thread \r\n" + System.Threading.Thread.CurrentThread.GetHashCode();

            // Declare an HttpClient object and increase the buffer size. The default
            // buffer size is 65,536.
            HttpClient client =
                new HttpClient() { MaxResponseContentBufferSize = 1000000 };

            resultsTextBox.Text += "\r\n           Calling HttpClient.GetStringAsync at thread \r\n" + System.Threading.Thread.CurrentThread.GetHashCode();

            //for (int i = 0; i < 1000; i++)
            //    resultsTextBox.Text += String.Format("loop{0}, ", i);

            string urlContents = string.Empty;

                // GetStringAsync returns a Task<string>. 
                //Task<string> getStringTask = client.GetStringAsync("http://msdn.microsoft.com");
            Task<string> getStringTask = AccessString();

                resultsTextBox.Text += "\r\nTHREE: Back in AccessTheWebAsync at thread \r\n" + System.Threading.Thread.CurrentThread.GetHashCode() +
                    "           Task getStringTask is started.";

                // AccessTheWebAsync can continue to work until getStringTask is awaited.

                resultsTextBox.Text +=
                    "\r\n           About to await getStringTask and return a Task<int> to startButton_Click.\r\n";

                // Retrieve the website contents when task is complete.
                urlContents = await getStringTask;

                resultsTextBox.Text += "\r\nFIVE:  Back in AccessTheWebAsync at thread \r\n" + System.Threading.Thread.CurrentThread.GetHashCode() +
                    "\r\n           Task getStringTask is complete." +
                    "\r\n           Processing the return statement." +
                    "\r\n           Exiting from AccessTheWebAsync.\r\n";

                return urlContents.Length;
        }

        async Task<string> AccessString()
        {
            Task<string> hello = GetString();
            await hello;
            return hello.Result;
        }

        Task<string> GetString()
        {
            var task = new Task<string>(() => { return CreateString(); });
            task.Start();
            return task;
        }

        string CreateString()
        {
            string hello = "Hello Asynchronous Programming!";
            return hello;
        }
    }
}

// Sample Output:

// ONE:   Entering startButton_Click.
//           Calling AccessTheWebAsync.

// TWO:   Entering AccessTheWebAsync.
//           Calling HttpClient.GetStringAsync.

// THREE: Back in AccessTheWebAsync.
//           Task getStringTask is started.
//           About to await getStringTask and return a Task<int> to startButton_Click.

// FOUR:  Back in startButton_Click.
//           Task getLengthTask is started.
//           About to await getLengthTask -- no caller to return to.

// FIVE:  Back in AccessTheWebAsync.
//           Task getStringTask is complete.
//           Processing the return statement.
//           Exiting from AccessTheWebAsync.

// SIX:   Back in startButton_Click.
//           Task getLengthTask is finished.
//           Result from AccessTheWebAsync is stored in contentLength.
//           About to display contentLength and exit.

// Length of the downloaded string: 33946.
