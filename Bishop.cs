using System;
using System.Collections.Generic;
using System.Text;

namespace MiniChess
{
    class Bishop : Piece
    {
        public bool WhiteField { get; set; }


        public Bishop(bool white, int position) : base(((white) ? (char)9821 : (char)9815), (white) ? new Position(7, position) : new Position(0, position), white, true)
        {
            if (white)
                this.WhiteField = (position == 2) ? false : true;
            else
                this.WhiteField = (position == 2) ? true : false;
        }



        public override string ToString()
        {
            return string.Format("{0}B", IsWhite ? "W" : "B");
        }

        public override Piece GetCopy()
        {
            if (this != null)
            {
                Bishop pieceCopy = new Bishop(this.IsWhite, this.Position.Column);
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
            return false;
        }

        public override void InitializePaths()
        {
            this.ProcessDiagonalPath(true);
            this.ProcessDefault();
        }
    }
}
