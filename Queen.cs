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

        public override bool ValidateMove(Position newPos, GameState state)
        {
            if (ValidatedDiagonalPathAttack(true, newPos, state))
                return true;
            else if (ValidateHorizontalPathAttack(true, newPos, state))
                return true;
            else if (ValidateVerticalPathAttack(true, newPos, state))
                return true;
            return false;
        }

        public override void InitializePaths(GameState state)
        {
            this.ProcessDiagonalPath(true, state);
            this.ProcessHorizontalPath(true, state);
            this.ProcessVerticalPath(true, state);
            this.ProcessDefault();
        }
    }
}
