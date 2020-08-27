using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultProbeGenerator.SERVICES
{
    public static class Logger
    {
        // VARIABLES
        private static string content =
            " ------------------------ " + Environment.NewLine +
            " ------------------------ " + Environment.NewLine +
            "| RESULT PROBE GENERATOR |" + Environment.NewLine +
            " ------------------------ " + Environment.NewLine +
            " ------------------------ " + Environment.NewLine + Environment.NewLine;

        private static NXOpen.Session theSession = NXOpen.Session.GetSession();
        private static NXOpen.ListingWindow lw = theSession.ListingWindow;

        /// <summary>
        /// Write a message to the log
        /// </summary>
        /// <param name="msg"></param>
        public static void Write(string msg)
        {
            content += msg + Environment.NewLine;
        }

        /// <summary>
        /// Show the log content in the ListingWindow of NX/Simcenter
        /// </summary>
        public static void Show()
        {
            lw.Open();
            lw.WriteFullline(content);
        }
    }
}
