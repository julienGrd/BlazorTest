using BlazorTest.Client.Components.Grid.V2;
using BlazorTest.Client.Components.Grid.V3;
using BlazorTest.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

namespace BlazorTest.Client.Components
{
    public partial class ListForecastVirtualizedV3
    {
        [Parameter]
        public IEnumerable<WeatherForecast> Items { get; set; }

        public GridVirtualizeV3<WeatherForecast> Grid;

        protected override void OnAfterRender(bool firstRender)
        {
            Console.WriteLine("ListForecastV2 onAfterRender");
            base.OnAfterRender(firstRender);
        }
    }
}
