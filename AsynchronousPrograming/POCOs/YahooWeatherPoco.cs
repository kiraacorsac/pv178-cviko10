using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsynchronousPrograming.POCOs
{
    public class Condition
    {
        public string text { get; set; }
    }

    public class Item
    {
        public Condition condition { get; set; }
    }

    public class Channel
    {
        public Item item { get; set; }
    }

    public class Results
    {
        public Channel channel { get; set; }
    }

    public class Query
    {
        public int count { get; set; }
        public string created { get; set; }
        public string lang { get; set; }
        public Results results { get; set; }
    }

    public class YahooWeatherPoco
    {
        public Query query { get; set; }
    }
}
