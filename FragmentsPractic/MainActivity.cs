using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using FragmentsPractic;
using System;

namespace FragmentsPractic
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            


            var mainButton = (Button)this.FindViewById(Resource.Id.mainButton);
            mainButton.Click += delegate (object sender, System.EventArgs e)
            {
                SetFragment(typeof(MainFragment));

            };



            var optionsButton = (Button)this.FindViewById(Resource.Id.optionsButton);
            optionsButton.Click += delegate (object sender, System.EventArgs e)
            {
                SetFragment(typeof(OptionsFragment));
            };


            var aboutButton = (Button)this.FindViewById(Resource.Id.aboutButton);
            aboutButton.Click += delegate (object sender, System.EventArgs e)
            {
                SetFragment(typeof(AboutFragment));
            };
           

        }

        protected void SetFragment(Type type)
        {
            var fragment = SupportFragmentManager.FindFragmentById(Resource.Id.fragmentContainer);
            if (fragment != null)
                SupportFragmentManager.BeginTransaction()
                   .Remove(fragment)
                   .Commit();

            fragment = Activator.CreateInstance(type) as Android.Support.V4.App.Fragment;
            SupportFragmentManager.BeginTransaction()
              .Add(Resource.Id.fragmentContainer, fragment)
              .Commit();

        }


    }
}

