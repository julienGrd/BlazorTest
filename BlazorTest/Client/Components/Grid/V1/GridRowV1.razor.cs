using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BlazorTest.Client.Components.Grid.V1
{
    public partial class GridRowV1<T> : IDisposable
    {
        [CascadingParameter]
        public GridV1<T> Parent { get; set; }

        [Parameter]
        public T Element { get; set; }


        [Parameter]
        public int Index { get; set; }


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
            Console.WriteLine($"GridV1 row {this.Index} afterRender");
            base.OnAfterRender(firstRender);
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
