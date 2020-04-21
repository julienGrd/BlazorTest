﻿using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace blazorWebassembly3._2Preview1.Client.Components.Grid
{
    public partial class GridColumn<T> : IDisposable
    {
        [CascadingParameter]
        public Grid<T> Parent { get; set; }

        [Parameter]
        public RenderFragment<T> Body { get; set; }

        [Parameter]
        public string Header { get; set; }

        [Parameter]
        public string Width { get; set; }

        [Parameter]
        public bool CanOrder { get; set; }

        [Parameter]
        public string OrderingExpression { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public int Index { get; set; }

        protected override void OnInitialized()
        {
            if (Parent == null)
                throw new ArgumentNullException(nameof(Parent));
            Parent.AddColumn(this);
            base.OnInitialized();
        }


        public ListSortDirection? CurrentDirection { get; set; }

        public string GetDirectionClass()
        {
            string className = string.Empty;
            if (this.CanOrder)
            {
                className = "orderable";
                if (this.CurrentDirection.HasValue)
                {
                    className += " ordered";
                    if (this.CurrentDirection == ListSortDirection.Ascending)
                    {
                        className += " asc";
                    }
                    else
                    {
                        className += " desc";
                    }
                }
            }
            return className;
        }

        public void Dispose()
        {
            //if (Parent != null)
            //{
            //    Parent.RemoveColumn(this);
            //}

        }

        protected void Order()
        {
            if (this.CanOrder)
            {
                var currentDirection = CurrentDirection == null ? ListSortDirection.Ascending
                                    : (CurrentDirection == ListSortDirection.Descending ? ListSortDirection.Ascending
                                    : ListSortDirection.Descending);
                this.CurrentDirection = currentDirection;
                this.Parent.Order(this.OrderingExpression, currentDirection, this.Index);
            }

        }

        public void ResetDirection()
        {
            this.CurrentDirection = null;
            this.StateHasChanged();
        }
    }
}
