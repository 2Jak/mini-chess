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

        public override bool ValidateMove(Position newPos, GameState state)
        {
            if (ValidateVerticalPathAttack(true, newPos, state))
                return true;
            else if (ValidateHorizontalPathAttack(true, newPos, state))
                return true;
            return false;
        }

        public override void InitializePaths(GameState state)
        {
            this.ProcessHorizontalPath(true, state);
            this.ProcessVerticalPath(true, state);
            this.ProcessDefault();
        }
    }
}
