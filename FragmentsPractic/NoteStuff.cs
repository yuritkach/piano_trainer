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
    public class DelayedEvent
    {
        public int Offset { get; set; }
        public DelayedEvent()
        {
            Offset = 0; 
        }
    }

    class NoteStuff
    {
        private readonly GLView glView;
        private readonly int linesDivider = 30; // Divide viewport on parts
        private List<FragmentsPractic.DelayedEvent> delayedEvents;

        private int tickOffset = 10000; // count
        private int tickShift = 30; // %


        public NoteStuff(GLView glView)
        {
            this.glView = glView;
            glView.BackgroundColor = new ColorArray(255, 255, 255, 0);
            delayedEvents = new List<FragmentsPractic.DelayedEvent>();

        }

        public void MakeTick()
        {
            foreach (var de in delayedEvents)
            {
                de.Offset++;
                if (de.Offset == tickOffset) PushEvent(de);

            }

        }


        protected void PushEvent(DelayedEvent de)
        {



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