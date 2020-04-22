using BlazorTest.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace BlazorTest.Client.Components.Grid.V2
{
    public partial class GridV2<T>
    {
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        [Parameter]
        public RenderFragment Columns { get; set; }

        [Parameter]
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

        private T _currentItem;

        public T CurrentItem
        {
            get
            {
                return this._currentItem;
            }
            set
            {
                if (!(this._currentItem?.Equals(value) ?? false))
                {
                    this._currentItem = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public string Id { get; set; } = Guid.NewGuid().ToString();

        protected string ContainerId => $"{Id}Container";

        [Parameter]
        public Action<T> OnDblClick { get; set; }

        [Parameter]
        public Action<T> OnSelect { get; set; }


        public List<GridColumnV2<T>> InternalColumns = new List<GridColumnV2<T>>();
        public void AddColumn(GridColumnV2<T> column)
        {
            if (!InternalColumns.Contains(column))
            {
                InternalColumns.Add(column);
            }

            //this.StateHasChanged();
        }

        public void RemoveColumn(GridColumnV2<T> column)
        {
            if (InternalColumns.Contains(column))
            {
                InternalColumns.Remove(column);
            }

            //this.LaunchStateHasChanged();
        }

        //protected override bool ShouldRender()
        //{
        //    Console.WriteLine(Environment.StackTrace);
        //    return base.ShouldRender();
        //}
        protected override void OnAfterRender(bool firstRender)
        {
            Console.WriteLine("GridV2 afterRender");
            base.OnAfterRender(firstRender);
        }

        public void ResetOrdering(int idxExclude)
        {
            foreach (var c in this.InternalColumns.Where(c => c.Index != idxExclude))
            {
                c.ResetDirection();
            }
        }

        public virtual bool IsSelected(T item)
        {
            return this.CurrentItem?.Equals(item) ?? false;
        }


        public virtual bool HaveExpander()
        {
            return false;
        }

        private CurrentDirectionModel _currentDirection;

        public void Order(string orderExpr, ListSortDirection direction, int index)
        {
            this._currentDirection = new CurrentDirectionModel()
            {
                OrderExpr = orderExpr,
                Direction = direction,
                Index = index
            };
            this.ResetOrdering(this._currentDirection.Index);
            Console.WriteLine("geckosgrid Order");
            this.StateHasChanged();
        }

        public IEnumerable<T> GetOrderedItems(IEnumerable<T> source)
        {
            if (!this._currentDirection?.Equals(default(ValueTuple<string, ListSortDirection, int>)) ?? false)
            {
                return source.AsQueryable().OrderBy(this._currentDirection.OrderExpr + (this._currentDirection.Direction == ListSortDirection.Descending ? " descending" : string.Empty));
            }
            else
            {
                return source;
            }
        }

        protected void SelectNextResult()
        {
            SelectTo(1);
            ScrollToCurrentItem(true);
        }
        protected void SelectPrevResult()
        {
            SelectTo(-1);
            ScrollToCurrentItem(false);
        }

        protected void ScrollToCurrentItem(bool onTop)
        {
            this.JsRuntime.InvokeVoidAsync("synchronizeTableScroll", ContainerId, onTop);
        }

        private void SelectTo(int step)
        {
            var lCurrentPosition = this.CurrentItem == null ? -1 : this.Items.ToList().IndexOf(this.CurrentItem);
            int toIdx = 0;
            if (lCurrentPosition > -1)
            {
                toIdx = lCurrentPosition + step;
            }
            var toItem = this.Items.ElementAt(toIdx);
            if (toItem != null)
            {
                Console.WriteLine("go to step " + step);
                //this.CurrentItem = toItem;
                this.HandleSelect(toItem, false, null);
            }

        }

        protected void OnKeyUp(KeyboardEventArgs args)
        {
            var currentKey = KeyHelper.GetKeyFromString(args.Key);

            Console.WriteLine(currentKey.ToString());

            if (currentKey == Key.Up)
            {
                SelectPrevResult();
            }
            else if (currentKey == Key.Down)
            {
                SelectNextResult();
            }
            else if (currentKey == Key.Enter)
            {
                HandleDblClick(this.CurrentItem);
            }
        }

        public void HandleDblClick(T element)
        {
            this.CurrentItem = element;
            this.OnDblClick?.Invoke(element);
        }
        public virtual void HandleSelect(T element, bool rightClick, MouseEventArgs args)
        {
            this.CurrentItem = element;
            this.OnSelect?.Invoke(element);
        }
    }
    internal class CurrentDirectionModel
    {
        public string OrderExpr { get; set; }
        public ListSortDirection Direction { get; set; }
        public int Index { get; set; }
    }
}
