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

namespace FragmentsPractic
{
    class NoteStuff
    {
        private readonly GLView glView;
        private readonly int linesDivider = 30; // Divide viewport on parts

        public NoteStuff(GLView glView)
        {
            this.glView = glView;
            glView.BackgroundColor = new ColorArray(255, 255, 255, 0);
        }

        public void DrawCurrentState()
        {
            glView.GLLines.Clear();
            DrawNoteStuff();
        }

        protected void DrawNoteStuff()
        {
            DrawNoteStuffLines(); //

        }

        protected void DrawNoteStuffLines()
        {
            int partHeight = glView.Height / linesDivider;
            for (int i = 1; i<= linesDivider; i++)
            {
                if ((i > 3 && i < 9) || (i > 10 && i < 16))
                {
                    GLLine ln = new GLLine(0, i * partHeight, 0, glView.Width, i * partHeight, 0, new ColorArray(0, 0, 0,1));
                    glView.GLLines.Add(ln);
                }
            }

        }


    }
}