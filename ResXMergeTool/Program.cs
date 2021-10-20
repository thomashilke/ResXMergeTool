using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace ResXMergeTool
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            try
            {
                if (Environment.GetCommandLineArgs().Length != 5)
                {
                    // Show usage and abort.
                    Console.WriteLine(
                        "Usage: ResXMergeTool.exe File.resx.BASE[.resx] File.resx.LOCAL[.resx] File.resx.REMOTE[.resx] Pathname");

                    Environment.ExitCode = 1;
                }
                else
                {
                    Console.WriteLine("Starting GUI.. Don't close this window until you're done!");
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    FrmResXDifferences frmDiff =
                        new FrmResXDifferences(
                            new string[]
                            {
                                Environment.GetCommandLineArgs()[1],
                                Environment.GetCommandLineArgs()[2],
                                Environment.GetCommandLineArgs()[3],
                                Environment.GetCommandLineArgs()[4]
                            });

                    Application.Run(frmDiff);
                }
            }
            catch (Exception)
            {
                Environment.ExitCode = 1;
            }
        }
    }
}
