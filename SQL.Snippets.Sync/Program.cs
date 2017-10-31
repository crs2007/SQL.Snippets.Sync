using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQL.Snippets.Sync
{
    class Program
    {
        static void Main(string[] args)
        {

            string Source;
            string Target;
            try
            {

                Source = ConfigHelper.GetConfigValue("Source");
                Target = ConfigHelper.GetConfigValue("Target");

                DirectoryInfo sourcedinfo = new DirectoryInfo(Source);
                DirectoryInfo destinfo = new DirectoryInfo(Target);

                ConfigHelper.CopyAll(sourcedinfo, destinfo);

                ConfigHelper.CopyAll(destinfo, sourcedinfo);
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
