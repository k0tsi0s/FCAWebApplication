using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace FCAWebApplication.Services
{
    static class MyLogger
    {

        private static List<String> LogRecords = new List<String>();
      

        public static void Record(string strMessage)
        {
           LogRecords.Add(strMessage);
        }
        
        public static String GetLogs()
        {
            String jsonContent = null;
            foreach (String logR in LogRecords)
            {
                jsonContent += logR + " ************\n";
            }



            return jsonContent;
        }
    }
}