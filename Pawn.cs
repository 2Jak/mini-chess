using System;
using System.Collections.Generic;
using System.Text;

namespace MiniChess
{
    public class Pawn : Piece
    {
        public Pawn(bool white, int position) : base(((white) ? (char)9823 : (char)9817), (white) ? new Position(6, position) : new Position(1, position), white, false) { }



        public override string ToString()
        {
            return string.Format("{0}P", IsWhite ? "W" : "B");
        }

        public override bool ValidateMove(Position newPos)
        {
            if (ValidatedDiagonalPathAttack(false, newPos))
                return true;
            else if (ValidateVerticalPathMove(true, newPos))
                if (checkTwoSquareMove(newPos))
                    if (this.HasMoved)
                        return false;
                    else
                        return true;
                else
                    return true;
            return false;
        }

        public override bool Move(Position newPos)
        {
            if (ValidateMove(newPos))
            {
                if (checkTwoSquareMove(newPos))
                {
                    GameState.MovePiece(this.Position, newPos, this);
                    createGhost(newPos);
                    GameState.noPawnMoves = 0;
                    GameState.ResetRepeats();
                    this.InitializePaths();
                    return true;
                }
                else
                {
                    GameState.MovePiece(this.Position, newPos, this);
                    GameState.noPawnMoves = 0;
                    GameState.ResetRepeats();
                    this.InitializePaths();
                    return true;
                }
            }
            return false;
        }

        public override void InitializePaths()
        {
            this.ProcessDiagonalPath(false);
            this.ProcessVerticalPath(false);
            if (this.IsWhite && !this.HasMoved)
                this.MovePath.Paths[1].Row--;
            else if (!this.IsWhite && !this.HasMoved)
                this.MovePath.Paths[7].Row++;
            this.ProcessDefault();
        }

        public override Piece GetCopy()
        {
            if (this != null)
            {
                Pawn pieceCopy = new Pawn(this.IsWhite, this.Position.Column);
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



        private bool checkTwoSquareMove(Position newPos)
        {
            if (this.IsWhite)
                return this.Position.OnTheFlyChanges(-2, 0).Equals(newPos);
            else
                return this.Position.OnTheFlyChanges(2, 0).Equals(newPos);
        }

        private void createGhost(Position newPos)
        {
            if (this.IsWhite)
                GameState.Board[(newPos.Row + 1), newPos.Column] = new Ghost(this);
            else
                GameState.Board[(newPos.Row - 1), newPos.Column] = new Ghost(this);
        }
    }
}
