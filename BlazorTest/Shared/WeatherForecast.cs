using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorTest.Shared
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public string Summary { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public City City { get; set; }
    }

    public class City
    {
        public string Name { get; set; }
    }
}
