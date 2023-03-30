using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplePlugin
{
    internal class Constants
    {
        public const string WINDOWNAME_MAIN = "MainWindow";
        public const string WINDOWNAME_CONF = "ConfigWindow";
        public const int BET_MAX = 1000;
        public static readonly Dictionary<string, double> RATE4 = new Dictionary<string, double>()
        {
            {"bonus" ,4.0},
            {"high",0.85 },
            {"low",0.85 },
        };
        public static readonly Dictionary<string, double> RATE3 = new Dictionary<string, double>()
        {
            { "bonus", 3.0 } , 
            { "high", 0.9 } , 
            { "low", 0.9 },
        };
        public static readonly Dictionary<string, double> RATE2 = new Dictionary<string, double>()
        {
            {"bonus" ,2.0},
            {"high",0.95 },
            {"low",0.95 },
        };
        public static readonly Dictionary<string, double> RATE = new Dictionary<string, double>()
        {
            {"bonus" ,0.1},
            {"high",1.0 },
            {"low",1.0 },
        };
    }
}
