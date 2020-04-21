using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blazorWebassembly3._2Preview1.Client.Components.AdvanceGrid
{
    public partial class AdvanceGrid<T>
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        protected string ContainerId => $"{Id}Container";

        [Parameter]
        public RenderFragment TableHeader { get; set; }

        [Parameter]
        public RenderFragment<T> RowTemplate { get; set; }

        [Parameter]
        public IReadOnlyList<T> Items { get; set; }
    }
}
