using System;
using System.Collections.Generic;
using System.Text;

namespace MiniChess
{
    public class Path
    {
        //0 - topLeft 1 - top 2 - topRight 3 - left 4 - current 5 - right 6 - downLeft 7 - down 8 - downRight
        public Position[] Paths { get; set; }

        public Path()
        {
            this.Paths = new Position[9];
        }
    }
}
