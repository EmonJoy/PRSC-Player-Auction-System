using System;
using System.Windows.Forms;

namespace PRSC_Player_Auction_System
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Global exception handlers — prevents silent crashes
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            Application.ThreadException += (sender, e) =>
            {
                MessageBox.Show(
                    $"Unexpected error:\n\n{e.Exception.Message}\n\n{e.Exception.StackTrace}",
                    "Application Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            };

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                MessageBox.Show(
                    $"Fatal error:\n\n{ex?.Message}\n\n{ex?.StackTrace}",
                    "Fatal Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Application.Run(new SplashForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to start application:\n\n{ex.Message}\n\n{ex.StackTrace}",
                    "Startup Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}