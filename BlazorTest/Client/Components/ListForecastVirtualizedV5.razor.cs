using BlazorTest.Client.Components.Grid.V5;
using BlazorTest.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

namespace BlazorTest.Client.Components
{
    public partial class ListForecastVirtualizedV5
    {
        [Parameter]
        public IEnumerable<MeasurableItem> Items { get; set; }

        public GridVirtualizeV5<MeasurableItem> Grid;

        protected override void OnAfterRender(bool firstRender)
        {
            Console.WriteLine("ListForecastV5 onAfterRender");
            base.OnAfterRender(firstRender);
        }
    }
}
