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
        public static readonly Dictionary<string, double> RATE_P100to500 = new Dictionary<string, double>()
        {
            {"bonus" ,2.0},
            {"num1", 0.9},
            {"num2", 1.2},
            {"num3", 3},
            {"high",1.2 },
            {"low",1.2 },
        };
        public static readonly Dictionary<string, double> RATE_P500to1000 = new Dictionary<string, double>()
        {
            {"bonus" ,1.0},
            {"num1", 0.7},
            {"num2", 1},
            {"num3", 3},
            {"high",1.0 },
            {"low",1.0 },
        };
        public static readonly Dictionary<string, double> RATE = new Dictionary<string, double>()
        {
            {"bonus" ,2.0},
            {"num1", 1},
            {"num2", 2},
            {"num3", 3},
            {"high",1.4 },
            {"low",1.4 },
        };
    }
}
