using BlazorTest.Client.Components.Grid.V4;
using BlazorTest.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

namespace BlazorTest.Client.Components
{
    public partial class ListForecastVirtualizedV4
    {
        [Parameter]
        public IEnumerable<WeatherForecast> Items { get; set; }

        public GridVirtualizeV4<WeatherForecast> Grid;

        protected override void OnAfterRender(bool firstRender)
        {
            Console.WriteLine("ListForecastV2 onAfterRender");
            base.OnAfterRender(firstRender);
        }
    }
}
