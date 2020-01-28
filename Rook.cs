using System;
using System.Collections.Generic;
using System.Text;

namespace MiniChess
{
    class Rook : Piece
    {
        public Rook(bool white, int position) : base(((white) ? (char)9820 : (char)9814), (white) ? new Position(7, position) : new Position(0, position), white, true) { }




        public override string ToString()
        {
            return string.Format("{0}R", IsWhite ? "W" : "B");
        }

        public override Piece GetCopy()
        {
            if (this != null)
            {
                Rook pieceCopy = new Rook(this.IsWhite, this.Position.Column);
                pieceCopy.HasMoved = this.HasMoved;
                pieceCopy.Piecesign = this.Piecesign;
                pieceCopy.Position = this.Position;
                pieceCopy.AttackPath = this.AttackPath;
                pieceCopy.MovePath = this.MovePath;
                pieceCopy.IsWhite = this.IsWhite;
                pieceCopy.biDirectional = this.biDirectional;
                return pieceCopy;
            }
            else
                return this;
        }

        public override bool ValidateMove(Position newPos)
        {
            if (ValidateVerticalPathAttack(true, newPos))
                return true;
            else if (ValidateHorizontalPathAttack(true, newPos))
                return true;
            return false;
        }

        public override void InitializePaths()
        {
            this.ProcessHorizontalPath(true);
            this.ProcessVerticalPath(true);
            this.ProcessDefault();
        }
    }
}
