using Android.App;
using Android.Content;
using Android.Widget;
using ShareMyDay.Database.Models;
using ShareMyDay.Story.StoryFunctions;
using System.Collections.Generic;

namespace ShareMyDay.UIComponents
{
    /*
     * Class name: SpinnerComponent
     * Purpose: To control the spinner UI element 
     */
    public class SpinnerComponent
    {
        private readonly Spinner _spinner;
        private readonly Context _context;
        private string _selectedValue; 

        /*
         * Constructor
         * To set the activity and context for the spinner 
         */
        public SpinnerComponent(Activity activity, int id, Context context)
        {
            _spinner = activity.FindViewById<Spinner> (id);
            _context = context;
        }

        /*
         * Method Name: EventsPopulate
         * Purpose: To populate the spinner with events 
         */
        public void EventsPopulate()
        {
            StoryGeneration generator = new StoryGeneration(new Database.Database(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),"ShareMyDay.db3"), _context);
            List<StoryEvent> events = generator.GetEvents();
            List<string> list = new List<string>
            {
                "New Event"
            };
            foreach (var i in events)
            {
                list.Add(i.Value);
            }
            
            var adapter =  new ArrayAdapter<string>(_context,
                Android.Resource.Layout.SimpleSpinnerItem, list);

            adapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
            _spinner.Adapter = adapter; 

        }
 
        /*
         * Method Name: NFcTypePopulate
         * Purpose: Used to populate the spinner with the different types of NFC cards 
         */
        public void NFcTypePopulate()
        {
            List<string> list = new List<string> {"Leisure Activity", "Class Activity", "Class", "Item","Teacher","Friend","Visitor", "Teacher - Menu Access"};
            var adapter =  new ArrayAdapter<string>(_context,
                Android.Resource.Layout.SimpleSpinnerItem, list);

            adapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
            _spinner.Adapter = adapter; 

        }

        /*
         * Method name: Disable
         * Purpose: To disable the spinner 
         */
        public void Disable()
        {
            _spinner.Enabled = false;
        }

        /*
         * Method name: spinner_ItemSelected
         * Purpose: To determine what item has been selected on the  spinner 
         */
        public void spinner_ItemSelected (object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string toast = string.Format ("{0} selected", spinner.GetItemAtPosition (e.Position));
            Toast.MakeText (_context, toast, ToastLength.Long).Show ();
            _selectedValue = (string) spinner.GetItemAtPosition(e.Position);
        }

        /*
         * Method name: GetSelected
         * Purpose: To get the item selected 
         */
        public string GetSelected()
        {
            return _selectedValue;
        }

        /*
         * Method name: Setup
         * Purpose: To setup the spinner with events 
         */
        public void Setup()
        {
            _spinner.ItemSelected += spinner_ItemSelected;
            EventsPopulate();
        }

        /*
         * Method name: SetupNFcDropDown
         * Purpose: Used to set up the spinner with NFC type options
         */
        public void SetupNFcDropDown()
        {
            _spinner.ItemSelected += spinner_ItemSelected;
            NFcTypePopulate();
        }

    }
}