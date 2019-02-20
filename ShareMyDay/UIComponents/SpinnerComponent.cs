using Android.App;
using Android.Content;
using Android.Widget;
using System.Collections.Generic;

namespace ShareMyDay.UIComponents
{
    class SpinnerComponent
    {
        private readonly Spinner _spinner;
        private readonly Context _context;
        private string _selectedValue; 

        public SpinnerComponent(Activity activity, int id, Context context)
        {
            _spinner = activity.FindViewById<Spinner> (id);
            _context = context;
        }

        public Spinner Get()
        {
            return _spinner;
        }

        public void EventsPopulate()
        {
            //TO DO - make call to db to fetch all the events 
            List<string> list = new List<string> {"one", "two", "three", "four"};
            var adapter =  new ArrayAdapter<string>(_context,
                Android.Resource.Layout.SimpleSpinnerItem, list);

            adapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
            _spinner.Adapter = adapter; 

        }
 
        public void NFcTypePopulate()
        {
            List<string> list = new List<string> {"Leisure Activity", "Class Activity", "Class", "Item","Teacher","Friend","Visitor"};
            var adapter =  new ArrayAdapter<string>(_context,
                Android.Resource.Layout.SimpleSpinnerItem, list);

            adapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
            _spinner.Adapter = adapter; 

        }

        public void spinner_ItemSelected (object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string toast = string.Format ("{0} selected", spinner.GetItemAtPosition (e.Position));
            Toast.MakeText (_context, toast, ToastLength.Long).Show ();
            _selectedValue = (string) spinner.GetItemAtPosition(e.Position);
        }

        public string GetSelected()
        {
            return _selectedValue;
        }

        public void Setup()
        {
            _spinner.ItemSelected += spinner_ItemSelected;
            EventsPopulate();
        }

        public void SetupNFcDropDown()
        {
            _spinner.ItemSelected += spinner_ItemSelected;
            NFcTypePopulate();
        }

    }
}