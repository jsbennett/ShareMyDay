using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ShareMyDay.MenuItems
{
    class SpinnerComponent
    {
        private Spinner _spinner;

        public SpinnerComponent(Activity activity)
        {
            _spinner = activity.FindViewById<Spinner>(Resource.Id.eventSelector);
        }


    }
}