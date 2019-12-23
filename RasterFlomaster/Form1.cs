using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RasterFlomaster
{
    //instances
    public partial class FormOutpurRender : Form
    {
        public Windower<Color> WM = new Windower<Color>();
        Random Rnd = new Random();
        Vector[] VertexBuffer;
        public Bitmap FromColorArray(Color[,] map)
        {
            Bitmap bmp = new Bitmap(map.GetLength(0), map.GetLength(1));
            for(int X = 0; X < bmp.Width; X++)
                for(int Y = 0; Y < bmp.Height; Y++)
                {
                    bmp.SetPixel(X, Y, map[X, Y]);
                }
            return bmp;
        }
        public FormOutpurRender()
        {
            InitializeComponent();
            WM.RasterizerT = new Rasterizer<Color>(new Color[Width, Height]);
            WM.RasterizerT.DefaultShader = new DrawWhite();
            VertexBuffer = new Vector[3]
            {
                new Vector
                {
                    X = (float)0.3 * (Width),
                    Y = (float)0.5 * (Height)
                },
                new Vector
                {
                    X = (float)0.75 * (Width),
                    Y = (float)0.75 * (Height)
                },
                new Vector
                {
                    X = (float)0.35 * (Width),
                    Y = (float)0.35 * (Height)
                }
            };
            /*for(int n = 0; n < VertexBuffer.Length; n++)
            {
                VertexBuffer[n] = new Vector
                {
                    X = (float)Rnd.NextDouble() * (Width),
                    Y = (float)Rnd.NextDouble() * (Height)
                };
            }*/
            Pass();
        }
        public void Pass()
        {
            WM.Conversion.FromTriangle(VertexBuffer[0], VertexBuffer[1], VertexBuffer[2]);
            //WM.Windowing();
            WM.Raster();
        }

        private void FormOutpurRender_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(FromColorArray(WM.RasterizerT.RenderTarget), 0, 0);
            e.Graphics.DrawRectangles(
                new Pen(Color.Blue),
                new []
                {
                    new Rectangle((int)VertexBuffer[0].X, (int)VertexBuffer[0].Y, 4, 4),
                    new Rectangle((int)VertexBuffer[1].X, (int)VertexBuffer[1].Y, 4, 4),
                    new Rectangle((int)VertexBuffer[2].X, (int)VertexBuffer[2].Y, 4, 4)
                }
            );
        }
    }
    public class DrawWhite : PixelShader<Color>
    {
        public override Color Compute(Color inp)
        {
            return Color.Red;
        }
    }
    //workers
    public class Windower<T>
    {
        //Window[] Windows;
        public ScanConversion Conversion = new ScanConversion();
        public Rasterizer<T> RasterizerT;
        public virtual ScanLine Line(float Y)
        {
            /*
            ScanLine line;
            if(Y < Conversion.Split)
            {
                line = new ScanLine
                {
                    Start = Round( Conversion.DeltaStart * Y ),
                    End = Round( Conversion.DeltaEnd * Y, false )
                };
            }
            else
            {
                if (Conversion.SplitStart)
                    line = new ScanLine
                    {
                        Start = Round(Conversion.DeltaSecond * Y),
                        End = Round(Conversion.DeltaEnd * Y, false)
                    };
                else
                    line = new ScanLine
                    {
                        Start = Round(Conversion.DeltaStart * Y),
                        End = Round(Conversion.DeltaSecond * Y, false)
                    };
            }
            return line;*/
            return null;
        }
        public virtual int Round( float inp, bool Forward = true ) 
        {
            return Convert.ToInt32(Math.Round(inp));
        }
        public virtual void Raster()
        {
            float deltaStart = Conversion.DeltaStart, deltaStop = Conversion.DeltaEnd,
                LineStart = 0, LineEnd = 0;
            int splitting = 0;
            for (int m = 0; m < Conversion.Lenth; m++)
            {
                LineStart = deltaStart * m;
                LineEnd = deltaStop * m;
                if (m > Conversion.Split)
                {
                    splitting++;
                    (Conversion.SplitStart ?
                        ref LineStart : ref LineEnd).Setup(Conversion.DeltaSecond * splitting);
                }
                int LineBegin = Round(LineStart + Conversion.Cursor),
                    LineWidth = Round(LineEnd - LineStart, false);
                for (int l = 0; l < LineWidth; l++)
                {
                    RasterizerT[LineBegin + l, Round(Conversion.Start + m)] = default;
                }
            }
        }
        /*public virtual void Windowing()
        {
            Window window = new Window
            {   
                X = Round(Conversion.Cursor),
                Y = Round(Conversion.Start),
                Height = Round(Conversion.Split),

                DeltaStart = Conversion.DeltaStart,
                DeltaEnd = Conversion.DeltaEnd
            };
            if(Conversion.Lenth != Conversion.Split)
            {
                Window SecondWindow = new Window
                {
                    X = Round(Conversion.Cursor),
                    Y = Round(Conversion.Split + Conversion.Start),
                    Height = Round(Conversion.Lenth - Conversion.Split),

                    DeltaStart = Conversion.DeltaStart,
                    DeltaEnd = Conversion.DeltaSecond
                };
                if (Conversion.SplitStart)
                    ScanConversion.Swap(ref SecondWindow.DeltaStart, ref SecondWindow.DeltaEnd);
                Windows = new[] { window, SecondWindow };
            }
            else Windows = new []{ window };
        }*/
    }
    public class ScanConversion
    {
        public float Cursor, Start, Split, Lenth, //Curr = X, Start = Y
            DeltaStart, DeltaEnd, DeltaSecond;
        public bool SplitStart = false;
        public enum Order:int { Minimum = 0, Middle = 1, Maximum = 2}
        public virtual void FromTriangle(Vector A, Vector B, Vector C)
        {
            //Sorting
            //var Xs = Sort(A.X, B.X, C.X);
            var Xy = Sort(A.Y, B.Y, C.Y);

            Vector[] Vecs = { A, B, C };
            Vector[] //VX = { Vecs[Xs[(int)Order.Minimum]], Vecs[Xs[(int)Order.Middle]], Vecs[Xs[(int)Order.Maximum]] },
                VY = { Vecs[Xy[(int)Order.Minimum]], Vecs[Xy[(int)Order.Middle]], Vecs[Xy[(int)Order.Maximum]] };
            //Vertical stride setup
            Start = VY[(int)Order.Minimum].Y;
            Split = VY[(int)Order.Middle].Y - VY[(int)Order.Minimum].Y;
            Lenth = VY[(int)Order.Maximum].Y - VY[(int)Order.Minimum].Y;

            Cursor = VY[(int)Order.Minimum].X;
            //
            float DBig = (VY[(int)Order.Maximum].X - Cursor) / Lenth,
                DSmall = (VY[(int)Order.Middle].X - Cursor) / Split;
            DeltaStart = DBig;
            DeltaEnd = DSmall;
            DeltaSecond = (VY[(int)Order.Maximum].X - Start - VY[(int)Order.Middle].X) / (Lenth - Split);

            if (DBig * Split < VY[(int)Order.Middle].X)
            {   
                SplitStart = true; 
                Swap(ref DeltaStart, ref DeltaEnd); }
        }
        public static int[] Sort(float A, float B, float C)
        {
            float min = A, mid = B, max = C;
            int[] sorted = new int[] { 0, 1, 2 };
            if(min > mid)
            {
                Swap(ref min, ref mid);
                Swap(ref sorted[(int)Order.Minimum], ref sorted[(int)Order.Middle]);
            }
            if(mid > max)
            {
                Swap(ref max, ref mid);
                Swap(ref sorted[(int)Order.Middle], ref sorted[(int)Order.Maximum]);
            }
            if(min > mid)
            {
                Swap(ref min, ref mid);
                Swap(ref sorted[(int)Order.Middle], ref sorted[(int)Order.Minimum]);
            }
            return sorted;
        }
        public static void Swap<T>(ref T A, ref T B)
        {
            T temp = A;
            A = B;
            B = temp;
        }
    }
    public class Rasterizer<T>
    {
        public T[,] RenderTarget;
        public PixelShader<T> DefaultShader = new PixelShader<T>();
        public Rasterizer(T[,] Target = null)
        {
            if (Target == null) RenderTarget = new T[Program.render.Width,Program.render.Height];
            else RenderTarget = Target;
        }
        public virtual T this[int X, int Y, PixelShader<T> Shader = null]
        {
            get
            {
                return RenderTarget[X, Y];
            }
            set
            {
                if(X < RenderTarget.GetLength(0))
                RenderTarget[X, Y] = (Shader ?? DefaultShader).Compute(value);
            }
        }
    }
    public class PixelShader<T>
    {
        public virtual T Compute(T inp)
        {
            return inp;
        }
    }
    //data
    public class Window
    {
        public int X, Y, Height;
        public float DeltaStart, DeltaEnd;
    }
    public class Vector
    {
        public float X, Y;
    }
    public class ScanLine
    {
        public int Start, End;
    }
}