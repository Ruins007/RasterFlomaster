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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
    }
    public class ScanConversion
    {
        public float Curr, Start, Lenth, Split, //Curr = X, Start = Y
            DeltaStart, DeltaEnd, DeltaSecond;
        public bool SplitStart = false;
        public enum Order:int { Minimum = 0, Middle = 1, Maximum = 2}
        public void FromTriangle(Vector A, Vector B, Vector C)
        {
            var Xs = Sort(A.X, B.X, C.X);
            var Xy = Sort(A.Y, B.Y, C.Y);
            Vector[] Vecs = { A, B, C };
            Vector[] VX = { Vecs[Xs[(int)Order.Minimum]], Vecs[Xs[(int)Order.Middle]], Vecs[Xs[(int)Order.Maximum]] },
                VY = { Vecs[Xy[(int)Order.Minimum]], Vecs[Xy[(int)Order.Middle]], Vecs[Xy[(int)Order.Maximum]] };

            Start = VY[(int)Order.Minimum].Y;
            Split = VY[(int)Order.Middle].Y - VY[(int)Order.Minimum].Y;
            Lenth = VY[(int)Order.Maximum].Y - VY[(int)Order.Minimum].Y;

            float temp_arg0 = Split, temp_arg1 = Lenth;

            if (VY[(int)Order.Middle].X - VY[(int)Order.Minimum].X > 0) SplitStart = true;
            else Swap(ref temp_arg0, ref temp_arg1);

            DeltaStart = VX[(int)Order.Minimum].X / temp_arg0;
            DeltaEnd = VX[(int)Order.Maximum].X / temp_arg1;
            int temp_arg3 = 0;
            if(VX[(int)Order.Middle].Y - VX[(int)Order.Minimum].Y > 0) temp_arg3++;
            DeltaSecond = (VX[(int)Order.Maximum].X - VX[temp_arg3].X) / (VX[(int)Order.Maximum].Y - VX[temp_arg3].Y);

        }
        public int[] Sort(float A, float B, float C)
        {
            float min = A, mid = B, max = C;
            int[] sorted = new int[3];
            if(min > mid)
            {
                Swap(ref sorted[(int)Order.Minimum], ref sorted[(int)Order.Middle]);
            }
            if(mid > max)
            {
                Swap(ref sorted[(int)Order.Middle], ref sorted[2]);
            }
            if(min > max)
            {
                Swap(ref sorted[2], ref sorted[(int)Order.Minimum]);
            }
            return sorted;
        }
        public void Swap<T>(ref T A, ref T B)
        {
            T temp = A;
            A = B;
            B = temp;
        }
    }
    public struct Vector
    {
        public float X, Y;
    }
 /*   public struct Vector<T>
    {
        public IArifmetic<T>[] data;
    }
    public interface IArifmetic<T>
    {
        T Add(T A, T B);
        T Sub(T A, T B);
        T Mult(T A, T B);
        T Div(T A, T B);
        T Rem(T A, T B);
    }*/
}