using BlazorTest.Client.Components.Grid;
using BlazorTest.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorTest.Client.Components
{
    public partial class ListForecastV1
    {
        [Parameter]
        public IEnumerable<WeatherForecast> Items { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            Console.WriteLine("ListForecastV1 onAfterRender");
            base.OnAfterRender(firstRender);
        }
    }
}
