using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace SpeedSneaker.Startup
{
    class SpeedSneakerMain
    {
        static string[] commandLineArgs = null;

        public static string[] CommandLineArgs
        {
            get
            {
                return commandLineArgs;
            }
        }
        static bool UseExceptionBox
        {
            get
            {
#if DEBUG
                if (Debugger.IsAttached) return false;
#endif
                foreach (string arg in commandLineArgs)
                {
                    if (arg.Contains("noExceptionBox")) return false;
                }
                return true;
            }
        }




        [STAThread()]
        static void Main(string[] args)
        {
            commandLineArgs = args; // Needed by UseExceptionBox

            // Do not use LoggingService here (see comment in Run(string[]))
            if (UseExceptionBox)
            {
                try
                {
                    Run();
                }
                catch (Exception ex)
                {
                    try
                    {
                        HandleMainException(ex);
                    }
                    catch (Exception loadError)
                    {
                        // HandleMainException can throw error when log4net is not found
                        MessageBox.Show(loadError.ToString(), "Critical error (Logging service defect?)");
                    }
                }
            }
            else
            {
                Run();
            }

        }

        static void HandleMainException(Exception ex)
        {
            LoggingService.Fatal(ex);
            try
            {
                Application.Run(new ExceptionBox(ex, "Unhandled exception terminated SharpDevelop", true));
            }
            catch
            {
                MessageBox.Show(ex.ToString(), "Critical error (cannot use ExceptionBox)");
            }
        }
    }
}
