﻿using BlazorTest.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

namespace BlazorTest.Client.Components
{
    public partial class ListForecastPlain
    {
        [Parameter]
        public IEnumerable<WeatherForecast> Items { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            Console.WriteLine("ListForecastPlain onAfterRender");
            base.OnAfterRender(firstRender);
        }
    }
}

