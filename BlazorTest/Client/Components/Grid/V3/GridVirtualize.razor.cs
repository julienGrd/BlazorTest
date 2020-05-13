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

namespace BlazorTest.Client.Components.Grid.V3
{
    public partial class GridVirtualize<T>
    {
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        [Parameter]
        public RenderFragment Columns { get; set; }

        [Parameter]
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

        [Parameter] public double ItemHeight { get; set; }

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

        protected ElementReference contentElement { get; set; }
        int numItemsToSkipBefore;
        int numItemsToShow;

        double translateY => numItemsToSkipBefore * ItemHeight;


        public List<GridColumnVirtualize<T>> InternalColumns = new List<GridColumnVirtualize<T>>();
        public void AddColumn(GridColumnVirtualize<T> column)
        {
            if (!InternalColumns.Contains(column))
            {
                InternalColumns.Add(column);
            }

            //this.StateHasChanged();
        }

        public void RemoveColumn(GridColumnVirtualize<T> column)
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
            
            base.OnAfterRender(firstRender);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            Console.WriteLine("GridVirtualized afterRender");
            if (firstRender)
            {
                var objectRef = DotNetObjectReference.Create(this);
                var initResult = await JsRuntime.InvokeAsync<ScrollEventArgs>("VirtualizedComponent._initialize", objectRef, contentElement);
                OnScroll(initResult);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        [JSInvokable]
        public void OnScroll(ScrollEventArgs args)
        {
            // TODO: Support horizontal scrolling too
            var relativeTop = args.ContainerRect.Top - args.ContentRect.Top;
            numItemsToSkipBefore = Math.Max(0, (int)(relativeTop / ItemHeight));

            var visibleHeight = args.ContainerRect.Bottom - (args.ContentRect.Top + numItemsToSkipBefore * ItemHeight);
            numItemsToShow = (int)Math.Ceiling(visibleHeight / ItemHeight) + 3;

            StateHasChanged();
        }

        public class ScrollEventArgs
        {
            public DOMRect ContainerRect { get; set; }
            public DOMRect ContentRect { get; set; }
        }

        public class DOMRect
        {
            public double Top { get; set; }
            public double Bottom { get; set; }
            public double Left { get; set; }
            public double Right { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
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

        public IEnumerable<T> GetOrderedItems()
        {
            IEnumerable<T> baseList;
            if (!this._currentDirection?.Equals(default(ValueTuple<string, ListSortDirection, int>)) ?? false)
            {
                baseList = Items.AsQueryable().OrderBy(this._currentDirection.OrderExpr + (this._currentDirection.Direction == ListSortDirection.Descending ? " descending" : string.Empty));
            }
            else
            {
                baseList = Items;
            }
            return baseList;
        }

        public IEnumerable<T> GetShowingResult()
        {
            return this.GetOrderedItems().Skip(numItemsToSkipBefore).Take(numItemsToShow);
        }

        protected void SelectNextResult()
        {
            SelectTo(1);

            //not sure about this above line of code
            if (numItemsToSkipBefore < (Items.Count() - numItemsToShow - 1))
            {
                numItemsToSkipBefore++;
            }
            StateHasChanged();


            //ScrollToCurrentItem(true);
        }
        protected void SelectPrevResult()
        {
            SelectTo(-1);
            numItemsToSkipBefore = numItemsToSkipBefore > 0 ? numItemsToSkipBefore - 1 : 0;


            StateHasChanged();
            //ScrollToCurrentItem(false);
        }

        protected void ScrollToCurrentItem(bool onTop)
        {
           // this.JsRuntime.InvokeVoidAsync("synchronizeTableScroll", ContainerId, onTop);
        }

        private void SelectTo(int step)
        {
            var lCurrentPosition = this.CurrentItem == null ? -1 : this.GetOrderedItems().ToList().IndexOf(this.CurrentItem);
            int toIdx = 0;
            if (lCurrentPosition > -1)
            {
                toIdx = lCurrentPosition + step;
            }
            if(toIdx >= 0 && toIdx < this.GetOrderedItems().Count())
            {
                var toItem = this.GetOrderedItems().ElementAt(toIdx);
                if (toItem != null)
                {
                    Console.WriteLine("go to step " + step);
                    //this.CurrentItem = toItem;
                    this.HandleSelect(toItem, false, null);
                }
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
