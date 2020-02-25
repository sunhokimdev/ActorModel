using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TeamClient
{
    public class GraphLine
    {
        public int LocalID { get; set; }
        public System.Windows.Media.Brush StrokeColor { get; set; }
        public Point From { get; set; }
        public Point To { get; set; }
    }
}
