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

namespace ShareMyDay.UIComponents
{
    class SpinnerComponent
    {
        private Spinner spinner;
        private Context context;

        public SpinnerComponent(Activity activity, int id, Context context)
        {
            spinner = activity.FindViewById<Spinner> (id);
            this.context = context;
        }

        public Spinner Get()
        {
            return spinner;
        }

        public void Populate()
        {
            //TO DO - make call to db to fetch all the events 
            List<string> list = new List<string> {"one", "two", "three", "four"};
            var adapter =  new ArrayAdapter<string>(context,
                Android.Resource.Layout.SimpleSpinnerItem, list);

            adapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter; 

        }
 
        public void spinner_ItemSelected (object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string toast = string.Format ("{0} selected", spinner.GetItemAtPosition (e.Position));
            Toast.MakeText (context, toast, ToastLength.Long).Show ();
        }

        public void Setup()
        {
            spinner.ItemSelected += spinner_ItemSelected;
            Populate();
        }

    }
}