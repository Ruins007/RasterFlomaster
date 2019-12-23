using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RasterFlomaster
{
    static class Program
    {
        public static FormOutpurRender render;
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            render = new FormOutpurRender();
            Application.Run(render);
        }
        public static void Setup(this ref float me, float val)
        {
            me = val;
        }
    }
}