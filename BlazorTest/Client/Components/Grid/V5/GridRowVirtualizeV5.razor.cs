using BlazorTest.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;

namespace BlazorTest.Client.Components.Grid.V5
{
    public partial class GridRowVirtualizeV5<T> : IDisposable
    {
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        [CascadingParameter]
        public GridVirtualizeV5<T> Parent { get; set; }

        [Parameter]
        public T Element { get; set; }

        private IMeasurableItem MeasurableItem => Element as IMeasurableItem;


        [Parameter]
        public int Index { get; set; }

        private bool isMeasurable => typeof(IMeasurableItem).GetTypeInfo().IsAssignableFrom(typeof(T).Ge‌​tTypeInfo());

        private string Id => $"{Parent.Id}Row{Index}";


        protected bool IsSelected { get; set; }

        protected override void OnParametersSet()
        {
            this.ManageSuscribe(true);
            this.ManageSelection();
            base.OnParametersSet();
        }

        protected override void OnInitialized()
        {
            this.Parent.PropertyChanged -= Container_PropertyChanged;
            this.Parent.PropertyChanged += Container_PropertyChanged;
            base.OnInitialized();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            //Console.WriteLine($"GridVirtualized row {this.Index} afterRender");
            base.OnAfterRender(firstRender);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(isMeasurable && !MeasurableItem.RealMesure.HasValue)
            {
                //on le mesure concrètement
                MeasurableItem.RealMesure = await JsRuntime.InvokeAsync<double>("mesureHeight", Id);
                //Parent.ReplaceHeight(MeasurableItem);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        private void Container_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //on s'abonne au changement de selectedItem savoir si on doit rafraichir notre élément
            if (e.PropertyName == nameof(this.Parent.CurrentItem))
            {

                this.ManageSelection();
            }
        }

        protected IList<int> ColumnsEdited { get; set; } = new List<int>();


        //protected void OnCellDblClick(GridColumn<T> column)
        //{
        //    if (column.CanEdit)
        //    {
        //        if (!ColumnsEdited.Contains(column.Index))
        //        {
        //            ColumnsEdited.Add(column.Index);
        //        }
        //    }
        //}

        private void ManageSelection()
        {
            //si le current item est notre elemnt, on le sélectionne
            if (!this.IsSelected && this.Parent.IsSelected(Element))
            {
                Console.WriteLine("grid row become selected");
                this.IsSelected = true;
                this.StateHasChanged();
            }
            else if (this.IsSelected && !this.Parent.IsSelected(Element))
            {
                //on n'est plus sélectionné
                Console.WriteLine("grid row become unselected");
                this.IsSelected = false;
                this.StateHasChanged();
            }
        }

        protected void RightClickElement(T element, MouseEventArgs args)
        {
            this.Parent.HandleSelect(element, true, args);
            //this.LaunchStateHasChanged();
        }

        protected void SelectElement(T element, MouseEventArgs args)
        {
            //pas besoin de recharger, on va laisser l'event de changement de prop selected du container se lever et faire ca proprement
            this.Parent.HandleSelect(element, false, args);
            //this.LaunchStateHasChanged();
        }

        protected void SelectAndValideElement(T element, MouseEventArgs args)
        {
            this.Parent.HandleDblClick(element);
        }

        private void ManageSuscribe(bool withSuscribe)
        {
            var propertyChangedObject = this.Element as INotifyPropertyChanged;
            if (propertyChangedObject != null)
            {
                propertyChangedObject.PropertyChanged -= PropertyChangedObject_PropertyChanged;
                if (withSuscribe)
                {
                    propertyChangedObject.PropertyChanged += PropertyChangedObject_PropertyChanged;
                }

            }
        }

        private void PropertyChangedObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.StateHasChanged();
        }

        public void Dispose()
        {
            this.Parent.PropertyChanged -= Container_PropertyChanged;
            this.ManageSuscribe(false);
        }
    }
}
