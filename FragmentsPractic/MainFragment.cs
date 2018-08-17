using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Opengl;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;
using OpenTK.Platform.Android;


namespace FragmentsPractic
{
    public class MainFragment : Android.Support.V4.App.Fragment
    {
        private NoteStuff notestuff;
        private GLView glView;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View v= inflater.Inflate(Resource.Layout.fragment_main, container, false);
            glView = (GLView)v.FindViewById(Resource.Id.glview);

            notestuff = new NoteStuff(glView);
            glView.OnGLViewResize += delegate ()
            {
                notestuff.DrawCurrentState();
            };

            

            return v;

            
        }
        

    }

}