using System;
using System.Collections.Generic;
using System.Text;

namespace MiniChess
{
    class Queen : Piece
    {
        public Queen(bool white) : base(((white) ? (char)9819 : (char)9813), (white) ? new Position(7, 3) : new Position(0, 3), white, true) { }



        public override string ToString()
        {
            return string.Format("{0}Q", IsWhite ? "W" : "B");
        }

        public override Piece GetCopy()
        {
            if (this != null)
            {
                Queen pieceCopy = new Queen(this.IsWhite);
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
            if (ValidatedDiagonalPathAttack(true, newPos))
                return true;
            else if (ValidateHorizontalPathAttack(true, newPos))
                return true;
            else if (ValidateVerticalPathAttack(true, newPos))
                return true;
            return false;
        }

        public override void InitializePaths()
        {
            this.ProcessDiagonalPath(true);
            this.ProcessHorizontalPath(true);
            this.ProcessVerticalPath(true);
            this.ProcessDefault();
        }
    }
}
