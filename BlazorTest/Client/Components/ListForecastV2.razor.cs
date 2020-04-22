using BlazorTest.Client.Components.Grid;
using BlazorTest.Client.Components.Grid.V2;
using BlazorTest.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorTest.Client.Components
{
    public partial class ListForecastV2
    {
        [Parameter]
        public IEnumerable<WeatherForecast> Items { get; set; }

        public GridV2<WeatherForecast> Grid;

        protected override void OnAfterRender(bool firstRender)
        {
            Console.WriteLine("ListForecastV2 onAfterRender");
            base.OnAfterRender(firstRender);
        }
    }
}
