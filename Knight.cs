using System;
using System.Collections.Generic;
using System.Text;

namespace MiniChess
{
    public class Knight : Piece
    {
        public static int[][] PositionChangers = new int[8][] { new int[] { 2, -1 }, new int[] { 2, 1 }, new int[] { 1, -2 }, new int[] { -1, -2 }, new int[] { -2, 1 }, new int[] { -2, -1 }, new int[] { -1, 2 }, new int[] { 1 ,2 } };


        public Knight(bool white, int position) : base(((white) ? (char)9822 : (char)9816), (white) ? new Position(7, position) : new Position(0, position), white, true) { }




        public override string ToString()
        {
            return string.Format("{0}N", IsWhite ? "W" : "B");
        }

        public override Piece GetCopy()
        {
            if (this != null)
            {
                Knight pieceCopy = new Knight(this.IsWhite, this.Position.Column);
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
            if (ValidateKnightAttack(newPos))
                return true;
            return false;
        }

        public override void InitializePaths()
        {
            this.ProcessKnightPaths();
        }
    }
}
