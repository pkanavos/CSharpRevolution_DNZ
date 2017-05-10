using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    public static class Cutoff
    {
        //Load the stored cutoff value, MinDate otherwise
        public static DateTime LoadFrom(string cutoffFile,
            Func<string, bool> exists = null, Func<string, string> read = null)
        {
            exists = exists ?? File.Exists;
            read = read ?? File.ReadAllText;

            if (!exists(cutoffFile)) return DateTime.MinValue;

            //out variable - The only C# 7 specific feature here!
            DateTime.TryParse(read(cutoffFile), out DateTime cutoff1);
            return cutoff1;
        }

        //Store the current time as a cutoff value.
        public static void StoreTo(string cutoffFile, Action<string, string> write = null)
        {
            write = write ?? File.WriteAllText;
            write(cutoffFile, DateTime.UtcNow.ToString("u"));
        }

    }
}
