using System;
using System.Collections.Generic;
using System.Text;

namespace MiniChess
{
    public class Position
    {
        public int Column { get; set; }
        public int Row { get; set; }

        public Position(int x, int y)
        {
            this.Column = y;
            this.Row = x;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is Position)
                return (((Position)obj).Column == this.Column && ((Position)obj).Row == this.Row) ? true : false;
            else
                return false;
        }

        public Position OnTheFlyChanges(int x, int y)
        {
            return new Position(this.Row + x, this.Column + y);
        }

        public bool ValidateLegalPosion()
        {
            if (this.Column >= 0 && this.Column <= 7)
                if (this.Row >= 0 && this.Row <= 7)
                    return true;
                else
                    return false;
            else
                return false;
        }
    }
}
