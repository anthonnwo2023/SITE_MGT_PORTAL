using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.V1.Web.Pages.Components
{
    public partial class PatCheckboxList<TItem, TValue>
    {
        [Parameter] public List<TItem> ListOfItems { get; set; }

        [Parameter] public Func<TItem, string> TextField { get; set; }

        [Parameter] public Func<TItem, TValue> ValueField { get; set; }

        [Parameter] public string Style { get; set; }

        [Parameter] public string Class { get; set; } = "d-flex flex-column";

        [Parameter] public EventCallback<bool> OnItemSelection { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public List<TItem> SelectedValues { get; set; }

        [Parameter] public string ItemListTitle { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (SelectedValues != null)
            {
                SelectedValues.ForEach(x => { ((dynamic)x).IsSelected = true; });

                ListOfItems.ForEach(x => { ((dynamic)x).IsSelected = false; });

                ListOfItems.ForEach(x =>
                {
                    if (SelectedValues.Select(y => ((dynamic)y).Id).ToList().Contains(((dynamic)x).Id))
                    {
                        ((dynamic)x).IsSelected = true;
                    }
                });
            }
            else
            {
                SelectedValues = new List<TItem>();
            }

            await Task.CompletedTask;
        }

        protected async Task CheckboxChanged(ChangeEventArgs e, TItem data)
        {
            ((dynamic)data).IsSelected = false;
            await OnItemSelection.InvokeAsync((bool)e.Value);

            if ((bool)e.Value)
            {
                ((dynamic)data).IsSelected = true;
            }

            if (SelectedValues == null)
            {
                SelectedValues = new List<TItem>();
            }

            if (SelectedValues.Contains(data))
            {
                SelectedValues.Remove(data);
            }
            else
            {
                SelectedValues.Add(data);
            }
        }
        //public void CheckboxClicked(string aSelectedId, object aChecked)
        //{
        //    if ((bool)aChecked)
        //    {
        //        if (!SelectedValues.Contains(aSelectedId))
        //        {
        //            SelectedValues.Add(aSelectedId);
        //        }
        //    }
        //    else
        //    {
        //        if (SelectedValues.Contains(aSelectedId))
        //        {
        //            SelectedValues.Remove(aSelectedId);
        //        }
        //    }
        //    StateHasChanged();
        //}
    }
}
