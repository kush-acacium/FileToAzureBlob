using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileToAzureBlob.Testing.Integration
{
    internal static class ProcessExtensions
    {
        private static string env_var = "ASPNETCORE_ENVIRONMENT";

        internal static void Initialise(this Process process, string pathToExe, string env)
        {
            process.StartInfo = new ProcessStartInfo(@pathToExe);
            InitBase(process, env);
        }

        internal static void Initialise(this Process process, string app, string command, string env)
        {
            process.StartInfo = new ProcessStartInfo(app, command);
            InitBase(process, env);
        }

        private static void InitBase(Process process, string env)
        {
            process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            process.StartInfo.CreateNoWindow = true;
            if (env != null)
            {
                process.StartInfo.EnvironmentVariables.Remove(env_var);
                process.StartInfo.EnvironmentVariables.Add(env_var, env);
            }
            process.Start();
        }

        internal static void Stop(this Process process)
        {
            process.Kill();
            process.Close();
        }        
    }
}
