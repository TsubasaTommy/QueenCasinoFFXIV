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
        public const int BET_MAX = 500;
        public static readonly Dictionary<string, double> RATE_P = new Dictionary<string, double>()
        {
            {"bonus" ,4.0},
            {"num1", 1},
            {"num2", 2},
            {"num3", 10},
            {"high",1.5 },
            {"low",1.5 },
        };
        public static readonly Dictionary<string, double> RATE = new Dictionary<string, double>()
        {
            {"bonus" ,2.0},
            {"num1", 1},
            {"num2", 2},
            {"num3", 7},
            {"high",1.3 },
            {"low",1.3 },
        };
    }
}
