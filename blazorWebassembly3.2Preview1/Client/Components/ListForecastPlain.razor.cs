using blazorWebassembly3._2Preview1.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blazorWebassembly3._2Preview1.Client.Components
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

