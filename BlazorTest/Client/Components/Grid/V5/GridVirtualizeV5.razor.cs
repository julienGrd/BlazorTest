using BlazorTest.Client.Components.Grid.V4;
using BlazorTest.Client.Services;
using BlazorTest.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Threading.Tasks;

namespace BlazorTest.Client.Components.Grid.V5
{
    public partial class GridVirtualizeV5<T>
    {
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        [Parameter]
        public RenderFragment Columns { get; set; }

        [Parameter]
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

        public IEnumerable<T> OrderedItems { get; set; } = Enumerable.Empty<T>();

        private IEnumerable<IMeasurableItem> MeasurableItems => Items.Cast<IMeasurableItem>();

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

        double translateY = 0;

        private bool canManageDifferentHeight => typeof(IMeasurableItem).GetTypeInfo().IsAssignableFrom(typeof(T).Ge‌​tTypeInfo());

        //private Dictionary<int, (double height, bool isReal)> measuredItems = new Dictionary<int, (double height, bool isReal)>();

        public double PaddingDiv { get; set; }


        public List<GridColumnVirtualizeV5<T>> InternalColumns = new List<GridColumnVirtualizeV5<T>>();
        public void AddColumn(GridColumnVirtualizeV5<T> column)
        {
            if (!InternalColumns.Contains(column))
            {
                InternalColumns.Add(column);
            }

            //this.StateHasChanged();
        }

        //public void ReplaceHeight(IMeasurableItem item)
        //{
        //    if (canManageDifferentHeight)
        //    {
        //        if (measuredItems.ContainsKey(item.GetHashCode()) && item.RealMesure.HasValue)
        //        {
        //            measuredItems[item.GetHashCode()] = (item.RealMesure.Value, true);
        //        }
        //    }
        //}

        public void RemoveColumn(GridColumnVirtualizeV5<T> column)
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

        protected override void OnInitialized()
        {
            this.ComputeOrderedItems();
            objectRef = DotNetObjectReference.Create(this);
            base.OnInitialized();
        }

        private static DotNetObjectReference<GridVirtualizeV5<T>> objectRef;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            Console.WriteLine("GridVirtualized afterRender");
            if (firstRender)
            {
                this.tableWidth = await JsRuntime.InvokeAsync<double>("getWidthTable");
                //MeasureItems();
                PaddingDiv = OrderedItems.Cast<IMeasurableItem>().Sum(i => i.GetHeight(this.tableWidth));

                await Initialize();
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task Initialize()
        {
            var initResult = await JsRuntime.InvokeAsync<ScrollEventArgs>("VirtualizedComponent._initialize", objectRef, contentElement);
            OnScroll(initResult);
        }

        private double tableWidth;

        //private void MeasureItems()
        //{
        //    if (canManageDifferentHeight)
        //    {
        //        this.MeasurableItems.ToList().ForEach(i => i.RealMesure = null);
        //        measuredItems = MeasurableItems.ToDictionary(i => i.GetHashCode(), i => (i.GetHeight(tableWidth), false));
        //    }
        //}

        [JSInvokable]
        public void OnScroll(ScrollEventArgs args)
        {
            if(tableWidth != args.WidthColumn)
            {
                Console.WriteLine("reset measure");
                this.MeasurableItems.ToList().ForEach(i => i.RealMesure = null);
                tableWidth = args.WidthColumn;
                //this.MeasureItems();
            }

            // TODO: Support horizontal scrolling too
            var relativeTop = args.ContainerRect.Top - args.ContentRect.Top;

            //Console.WriteLine("relativeTop " + relativeTop);

            var measurableOreredItems = OrderedItems.Cast<IMeasurableItem>();


            if (canManageDifferentHeight)
            {
                double currentTop = 0;
                var idx = 0;
                while (currentTop < relativeTop && idx < measurableOreredItems.Count())
                {
                    currentTop += measurableOreredItems.ElementAt(idx).GetHeight(tableWidth);
                    idx++;
                }
                numItemsToSkipBefore = idx;
                translateY = currentTop;

                double additionnalTranslate = 0;
                
               

                //Console.WriteLine("numItemsToSkipBefore " + numItemsToSkipBefore);
                //Console.WriteLine("translateY " + translateY.ToString()); ;

                var visibleHeight = args.ContainerRect.Bottom - (args.ContentRect.Top + currentTop);
                //Console.WriteLine("args.ContainerRect.Bottom  " + args.ContainerRect.Bottom);
                //Console.WriteLine("args.ContentRect.Top " + args.ContentRect.Top);
                //Console.WriteLine("currentTop " + currentTop);
               // Console.WriteLine("visibleHeight " + visibleHeight);

                double currentHeight = 0;
                numItemsToShow = 0;
                while (currentHeight < visibleHeight && idx < measurableOreredItems.Count())
                {
                    currentHeight += measurableOreredItems.ElementAt(idx).GetHeight(tableWidth);

                    //Console.WriteLine("add element with height " + measuredItems[OrderedItems.ElementAt(idx).GetHashCode()].height);

                    idx++;
                    numItemsToShow++;
                }

                var rest = this.OrderedItems.Count() - numItemsToSkipBefore - numItemsToShow;

                //Console.WriteLine("numItemsToSkipBefore " + numItemsToSkipBefore);
                //Console.WriteLine("numItemsToShow " + numItemsToShow);
                //Console.WriteLine("rest " + rest);

                if (numItemsToSkipBefore > 0 && rest > 2)
                {
                    //Console.WriteLine("remove one item to skip and add one item to show");

                    numItemsToSkipBefore--;

                    numItemsToShow++;

                    additionnalTranslate = measurableOreredItems.ElementAt(idx).GetHeight(tableWidth);

                    translateY -= additionnalTranslate;
                }

                if (rest > 1)
                {
                    var addingItemToShow = Math.Min(rest, 3);
                    //Console.WriteLine("addingItemToShow " + addingItemToShow);
                    numItemsToShow += addingItemToShow;
                }
                else
                {
                    //Console.WriteLine("additionate translateY");
                    //translateY += additionnalTranslate;
                }



                //Console.WriteLine("numItemsToShow " + numItemsToShow);

                //if ((numItemsToSkipBefore + numItemsToShow) > OrderedItems.Count())
                //{
                //    numItemsToShow = OrderedItems.Count() - numItemsToSkipBefore;
                //}


                PaddingDiv = measurableOreredItems.Sum(i => i.GetHeight(tableWidth)) - visibleHeight;
                //numItemsToShow = (int)Math.Ceiling(visibleHeight / ItemHeight) + 3;
                //Console.WriteLine("PaddingDiv " + PaddingDiv.ToString());
            }
            else
            {
                numItemsToSkipBefore = Math.Max(0, (int)(relativeTop / ItemHeight));
                var visibleHeight = args.ContainerRect.Bottom - (args.ContentRect.Top + numItemsToSkipBefore * ItemHeight);
                numItemsToShow = (int)Math.Ceiling(visibleHeight / ItemHeight) + 3;
                translateY = numItemsToSkipBefore * ItemHeight;
                PaddingDiv = (Items.Count() - numItemsToShow) * ItemHeight;
            }
            
            StateHasChanged();
        }

        public class ScrollEventArgs
        {
            public DOMRect ContainerRect { get; set; }
            public DOMRect ContentRect { get; set; }

            public double WidthColumn { get; set; }
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
            ComputeOrderedItems();
            Console.WriteLine("geckosgrid Order");
            //this.StateHasChanged();
            //this.Initialize();
            this.StateHasChanged();
        }

        public void ComputeOrderedItems()
        {
            if (!this._currentDirection?.Equals(default(ValueTuple<string, ListSortDirection, int>)) ?? false)
            {
                OrderedItems = Items.AsQueryable().OrderBy(this._currentDirection.OrderExpr + (this._currentDirection.Direction == ListSortDirection.Descending ? " descending" : string.Empty)).ToList();
            }
            else
            {
                OrderedItems = Items;
            }
        }

        public IEnumerable<T> GetShowingResult()
        {
            return this.OrderedItems.Skip(numItemsToSkipBefore).Take(numItemsToShow);
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
            var lCurrentPosition = this.CurrentItem == null ? -1 : OrderedItems.ToList().IndexOf(this.CurrentItem);
            int toIdx = 0;
            if (lCurrentPosition > -1)
            {
                toIdx = lCurrentPosition + step;
            }
            if(toIdx >= 0 && toIdx < OrderedItems.Count())
            {
                var toItem = OrderedItems.ElementAt(toIdx);
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
