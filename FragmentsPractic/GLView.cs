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
    public class ColorArray
    {
        public float Red { get; set; }
        public float Green { get; set; }
        public float Blue { get; set; }
        public float Alpha { get; set; }
        public float[] Color { get; set; }

        public ColorArray(float red, float green, float blue, float alpha)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;

            Color = new float[4];
            Color[0] = red;
            Color[1] = green;
            Color[2] = blue;
            Color[3] = alpha;

        }

    }

    public class GLGraphicObject
    {
        public ColorArray Color { get; set; }
        public GLGraphicObject()
        {
            Color = new ColorArray(0, 0, 0, 1); 
        }
    }



    class GLLine : GLGraphicObject
    {
        public float X1 { get; set; }
        public float Y1 { get; set; }
        public float Z1 { get; set; }

        public float X2 { get; set; }
        public float Y2 { get; set; }
        public float Z2 { get; set; }

     
       

        public GLLine(float x1,float y1, float z1, float x2, float y2, float z2, ColorArray color)
        {
            Color = color;
            X1 = x1;
            Y1 = y1;
            Z1 = z1;
            X2 = x2;
            Y2 = y2;
            Z2 = z2;
        }

    }

    delegate void EventHandler();

    class GLView : AndroidGameView
    {
        public ColorArray BackgroundColor { get; set; }
        public int ViewportWidth { get; set; }
        public int ViewportHeight { get; set; }

        public List<GLLine> GLLines { get; set; }
        public event EventHandler OnGLViewResize;

        private int colorHandle;
        private int MVPMatrixHandle;
        private int program;

        public GLView(Context context, IAttributeSet attrs) :
           base(context, attrs)
        {
            Init();
        }

        public GLView(IntPtr handle, Android.Runtime.JniHandleOwnership transfer)
            : base(handle, transfer)
        {
            Init();
        }

        void Init()
        {
            BackgroundColor = new ColorArray(0, 0, 0, 1);
            GL.Viewport(0, 0, ViewportWidth, ViewportHeight);
            GLLines = new List<GLLine>();
        }


        // This method is called everytime the context needs
        // to be recreated. Use it to set any egl-specific settings
        // prior to context creation
        protected override void CreateFrameBuffer()
        {
            GLContextVersion = GLContextVersion.Gles2_0;

            // the default GraphicsMode that is set consists of (16, 16, 0, 0, 2, false)
            try
            {
                Log.Verbose("GLTriangle", "Loading with default settings");

                // if you don't call this, the context won't be created
                base.CreateFrameBuffer();
                return;
            }
            catch (Exception ex)
            {
                Log.Verbose("GLTriangle", "{0}", ex);
            }

            // this is a graphics setting that sets everything to the lowest mode possible so
            // the device returns a reliable graphics setting.
            try
            {
                Log.Verbose("GLTriangle", "Loading with custom Android settings (low mode)");
                GraphicsMode = new AndroidGraphicsMode(0, 0, 0, 0, 0, false);

                // if you don't call this, the context won't be created
                base.CreateFrameBuffer();
                return;
            }
            catch (Exception ex)
            {
                Log.Verbose("GLTriangle", "{0}", ex);
            }
            throw new Exception("Can't load egl, aborting");
        }

    
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            RenderObjects();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ViewportHeight = Height;
            ViewportWidth = Width;

            // Vertex and fragment shaders
            string vertexShaderSrc =
                "uniform mat4 uMVPMatrix;                  \n" +
                "attribute vec4 vPosition;                 \n" +
                "void main()                               \n" +
                "{                                         \n" +
         //       "   gl_Position = vPosition;               \n" +
                "   gl_Position = uMVPMatrix * vPosition;  \n" +
                "}                                         \n";

            string fragmentShaderSrc =
                "precision mediump float;                  \n" +
                "uniform vec4 vColor;                      \n" +
                "void main()                               \n" +
                "{                                         \n" +
                "  gl_FragColor = vColor;                  \n" +
                "}                                         \n";

            int vertexShader = LoadShader(All.VertexShader, vertexShaderSrc);
            int fragmentShader = LoadShader(All.FragmentShader, fragmentShaderSrc);
            program = GL.CreateProgram();
            if (program == 0)
                throw new InvalidOperationException("Unable to create program");

            GL.AttachShader(program, vertexShader);
            GL.AttachShader(program, fragmentShader);

            GL.BindAttribLocation(program, 0, "vPosition");
            GL.LinkProgram(program);

            int linked = 0;
            GL.GetProgram(program, All.LinkStatus, ref linked);
            if (linked == 0)
            {
                // link failed
                int length = 0;
                GL.GetProgram(program, All.InfoLogLength, ref length);
                if (length > 0)
                {
                    var log = new StringBuilder(length);
                    GL.GetProgramInfoLog(program, length, ref length, log);
                    Log.Debug("GL2", "Couldn't link program: " + log.ToString());
                }

                GL.DeleteProgram(program);
                throw new InvalidOperationException("Unable to link program");
            }
            colorHandle = GL.GetUniformLocation(program, "vColor");
            MVPMatrixHandle = GL.GetUniformLocation(program, "uMVPMatrix");

            ////ParamsChanged();
            Run(30); // 25 кадр не используем...

        }

        int LoadShader(All type, string source)
        {
            int shader = GL.CreateShader(type);
            if (shader == 0)
                throw new InvalidOperationException("Unable to create shader");

            int length = 0;
            GL.ShaderSource(shader, 1, new string[] { source }, (int[])null);
            GL.CompileShader(shader);

            int compiled = 0;
            GL.GetShader(shader, All.CompileStatus, ref compiled);
            if (compiled == 0)
            {
                length = 0;
                GL.GetShader(shader, All.InfoLogLength, ref length);
                if (length > 0)
                {
                    var log = new StringBuilder(length);
                    GL.GetShaderInfoLog(shader, length, ref length, log);
                    Log.Debug("GL2", "Couldn't compile shader: " + log.ToString());
                }

                GL.DeleteShader(shader);
                throw new InvalidOperationException("Unable to compile shader of type : " + type.ToString());
            }

            return shader;
        }

        /*
        void RendPolyline(ref Verticles2D _polyline)
        {
               float[] vertices = new float[_polyline.Count * 3];
               for (int i = 0; i <= _polyline.Count - 1; i++)
               {
                   vertices[i * 3] = _polyline[i].Y;
                   vertices[i * 3 + 1] = _polyline[i].X;
                   vertices[i * 3 + 2] = 0;

               }

               unsafe
               {
                   fixed (float* pvertices = vertices)
                   {
                       GL.VertexAttribPointer(0, 3, All.Float, false, 0, new IntPtr(pvertices));
                       GL.EnableVertexAttribArray(0);
                       GL.Uniform4(colorHandle, 1, color);
                       GL.DrawArrays(All.LineLoop, 0, _polyline.Count);
                   }
               }

           }
          
    */

        public float[] CreateOrthoProjectionMatrix(float left, float right, float top, float bottom, float near, float far)
        {
            float[] m = new float[16];

            m[0] = 2.0f / (right - left);
            m[1] = 0.0f;
            m[2] = 0.0f;
            m[3] = 0.0f;

            m[4] = 0.0f;
            m[5] = 2.0f / (top - bottom);
            m[6] = 0.0f;
            m[7] = 0.0f;

            m[8] = 0.0f;
            m[9] = 0.0f;
            m[10] = -2.0f / (far - near);
            m[11] = 0.0f;

            m[12] = -(right + left) / (right - left);
            m[13] = -(top + bottom) / (top - bottom);
            m[14] = -(far + near) / (far - near);
            m[15] = 1.0f;

            return m;
        }


        protected void RenderObjects()
        {

            GL.ClearColor(BackgroundColor.Red , BackgroundColor.Green, BackgroundColor.Blue, BackgroundColor.Alpha);
            GL.Clear((int)All.ColorBufferBit);
            GL.Viewport(0, 0, ViewportWidth, ViewportHeight);
            GL.UseProgram(program);

            float[] mx = CreateOrthoProjectionMatrix(0.0f, ViewportWidth, 0.0f, ViewportHeight, -1.0f, 1.0f); 
            GL.UniformMatrix4(MVPMatrixHandle, 1, false, mx);

            DrawCurrentState();

            GL.Finish();
            SwapBuffers();
        }

        // this is called whenever android raises the SurfaceChanged event
        protected override void OnResize(EventArgs e)
        {
            ViewportHeight = Height;
            ViewportWidth = Width;
            MakeCurrent();
            OnGLViewResize();

        }

        protected void DrawCurrentState()
        {
            DrawGLLines();
        }

        protected void DrawGLLines()
        {
            float[] vertices = new float[6];
            foreach (var ln in GLLines)
            {
                vertices[0] = ln.X1;
                vertices[1] = ln.Y1;
                vertices[2] = ln.Z1;
                vertices[3] = ln.X2;
                vertices[4] = ln.Y2;
                vertices[5] = ln.Z2;
           
                unsafe
                {
                    fixed (float* pvertices = vertices)
                    {
                        GL.VertexAttribPointer(0, 3, All.Float, false, 0, new IntPtr(pvertices));
                        GL.EnableVertexAttribArray(0);
                        GL.Uniform4(colorHandle, 1, ln.Color.Color);
                        GL.DrawArrays(All.LineLoop, 0, 2);
                    }
                }
            }

        }

    }
}