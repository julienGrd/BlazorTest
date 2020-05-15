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

namespace BlazorTest.Client.Components.Grid.V4
{
    public partial class GridVirtualizeV4<T>
    {
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        [Parameter]
        public RenderFragment Columns { get; set; }

        [Parameter]
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

        [Parameter] public double ItemHeight { get; set; }

        public IEnumerable<T> OrderedItems { get; set; } = Enumerable.Empty<T>();

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

        //protected ElementReference contentElement { get; set; }
        //int numItemsToSkipBefore;
        //int numItemsToShow;

        //double translateY => numItemsToSkipBefore * ItemHeight;

        DotNetObjectReference<GridVirtualizeV4<T>> selfReference;
        ElementReference spacerBefore;
        ElementReference spacerAfter;
        long renderCount;
        int itemsToSkipBefore;
        int itemsToShow;
        int ItemsToSkipAfter => Items.Count() - itemsToSkipBefore - itemsToShow;


        public List<GridColumnVirtualizeV4<T>> InternalColumns = new List<GridColumnVirtualizeV4<T>>();
        public void AddColumn(GridColumnVirtualizeV4<T> column)
        {
            if (!InternalColumns.Contains(column))
            {
                InternalColumns.Add(column);
            }

            //this.StateHasChanged();
        }

        public void RemoveColumn(GridColumnVirtualizeV4<T> column)
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
        protected override void OnInitialized()
        {
            this.ComputeOrderedItems();
            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            Console.WriteLine("GridVirtualized V4 afterRender");
            if (firstRender)
            {
                selfReference = DotNetObjectReference.Create(this);
                await JsRuntime.InvokeVoidAsync("VirtualList.init", selfReference, spacerBefore, spacerAfter);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        [JSInvokable]
        public void OnSpacerVisible(string spacerType, Rect visibleRect, double containerHeight, double spacerBeforeHeight, double spacerAfterHeight)
        {
            // Reset to match values corresponding to this event
            itemsToSkipBefore = (int)Math.Round(spacerBeforeHeight / ItemHeight);
            itemsToShow = Items.Count() - itemsToSkipBefore - (int)Math.Round(spacerAfterHeight / ItemHeight);

            if (spacerType == "before" && itemsToSkipBefore > 0)
            {
                var visibleTop = visibleRect.Top;
                var firstVisibleItemIndex = (int)Math.Floor(visibleTop / ItemHeight);
                itemsToShow = (int)Math.Ceiling(containerHeight / ItemHeight) + 1;
                itemsToSkipBefore = Math.Max(0, firstVisibleItemIndex);
                StateHasChanged();
            }
            else if (spacerType == "after" && ItemsToSkipAfter > 0)
            {
                var visibleBottom = visibleRect.Top + visibleRect.Height;
                var lastVisibleItemIndex = itemsToSkipBefore + itemsToShow + (int)Math.Ceiling(visibleBottom / ItemHeight);
                itemsToShow = (int)Math.Ceiling(containerHeight / ItemHeight) + 1;
                itemsToSkipBefore = Math.Max(0, lastVisibleItemIndex - itemsToShow);
                StateHasChanged();
            }
        }

        public class Rect
        {
            public double Top { get; set; }
            public double Left { get; set; }
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
            this.ComputeOrderedItems();
            Console.WriteLine("geckosgrid Order");
            this.StateHasChanged();
        }

        public void ComputeOrderedItems()
        {
            if (!this._currentDirection?.Equals(default(ValueTuple<string, ListSortDirection, int>)) ?? false)
            {
                OrderedItems = Items.AsQueryable().OrderBy(this._currentDirection.OrderExpr + (this._currentDirection.Direction == ListSortDirection.Descending ? " descending" : string.Empty));
            }
            else
            {
                OrderedItems = Items;
            }
        }

        public IEnumerable<T> GetShowingResult()
        {
            return this.OrderedItems.Skip(itemsToSkipBefore).Take(itemsToShow);
        }

        protected void SelectNextResult()
        {
            SelectTo(1);

            //not sure about this above line of code
            if (itemsToSkipBefore < (Items.Count() - itemsToSkipBefore - 1))
            {
                itemsToSkipBefore++;
            }
            StateHasChanged();


            //ScrollToCurrentItem(true);
        }
        protected void SelectPrevResult()
        {
            SelectTo(-1);
            itemsToSkipBefore = itemsToSkipBefore > 0 ? itemsToSkipBefore - 1 : 0;


            StateHasChanged();
            //ScrollToCurrentItem(false);
        }

        protected void ScrollToCurrentItem(bool onTop)
        {
           // this.JsRuntime.InvokeVoidAsync("synchronizeTableScroll", ContainerId, onTop);
        }

        private void SelectTo(int step)
        {
            var lCurrentPosition = this.CurrentItem == null ? -1 : this.OrderedItems.ToList().IndexOf(this.CurrentItem);
            int toIdx = 0;
            if (lCurrentPosition > -1)
            {
                toIdx = lCurrentPosition + step;
            }
            if(toIdx >= 0 && toIdx < this.OrderedItems.Count())
            {
                var toItem = this.OrderedItems.ElementAt(toIdx);
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
