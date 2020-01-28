using System;
using System.Collections.Generic;
using System.Text;

namespace MiniChess
{
    class Ghost : Piece
    {
        public Pawn PawnSoul { get; set; }

        public Ghost(Pawn pawn)
        {
            this.PawnSoul = pawn;
            this.IsWhite = pawn.IsWhite;
        }

        public override Piece GetCopy()
        {
            Ghost pieceCopy = new Ghost(this.PawnSoul);
            return pieceCopy;
        }
    }
}
