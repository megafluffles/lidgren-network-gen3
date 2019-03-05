using System;
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Avalonia;
using Avalonia.Android;
using MSClient.ViewModels;
using MSClient.Views;

namespace MSClient
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AvaloniaActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            if (Avalonia.Application.Current == null)
            {
                AppBuilder.Configure(new App())
                    .UseAndroid()
                    .SetupWithoutStarting();
                var view = new MainView();
                view.DataContext = new MainViewViewModel();
                Content = view;
            }
            base.OnCreate(savedInstanceState);
        }

        //public override bool OnCreateOptionsMenu(IMenu menu)
        //{
        //    MenuInflater.Inflate(Resource.Menu.menu_main, menu);
        //    return true;
        //}

        //public override bool OnOptionsItemSelected(IMenuItem item)
        //{
        //    int id = item.ItemId;
        //    if (id == Resource.Id.action_settings)
        //    {
        //        return true;
        //    }

        //    return base.OnOptionsItemSelected(item);
        //}

        //private void FabOnClick(object sender, EventArgs eventArgs)
        //{
        //    View view = (View) sender;
        //    //Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
        //    //    .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        //}
	}
}

