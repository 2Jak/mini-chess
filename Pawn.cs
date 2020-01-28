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

        public override bool ValidateMove(Position newPos, GameState state)
        {
            if (ValidatedDiagonalPathAttack(false, newPos, state))
                return true;
            else if (ValidateVerticalPathMove(true, newPos, state))
                if (checkTwoSquareMove(newPos))
                    if (this.HasMoved)
                        return false;
                    else
                        return true;
                else
                    return true;
            return false;
        }

        public override bool Move(Position newPos, GameState state)
        {
            if (ValidateMove(newPos, state))
            {
                if (checkTwoSquareMove(newPos))
                {
                    state.MovePiece(this.Position, newPos, this);
                    createGhost(newPos, state);
                    state.noPawnMoves = 0;
                    state.ResetRepeats();
                    this.InitializePaths(state);
                    return true;
                }
                else
                {
                    state.MovePiece(this.Position, newPos, this);
                    state.noPawnMoves = 0;
                    state.ResetRepeats();
                    this.InitializePaths(state);
                    return true;
                }
            }
            return false;
        }

        public override void InitializePaths(GameState state)
        {
            this.ProcessDiagonalPath(false, state);
            this.ProcessVerticalPath(false, state);
            if (this.IsWhite && !this.HasMoved)
                this.MovePath.Paths[1].Row--;
            else if (!this.IsWhite && !this.HasMoved)
                this.MovePath.Paths[7].Row++;
            this.ProcessDefault();
        }

        public override Piece GetCopy()
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



        private bool checkTwoSquareMove(Position newPos)
        {
            if (this.IsWhite)
                return this.Position.OnTheFlyChanges(-2, 0).Equals(newPos);
            else
                return this.Position.OnTheFlyChanges(2, 0).Equals(newPos);
        }

        private void createGhost(Position newPos, GameState state)
        {
            if (this.IsWhite)
                state.GetBoard()[(newPos.Row + 1), newPos.Column] = new Ghost(this);
            else
                state.GetBoard()[(newPos.Row - 1), newPos.Column] = new Ghost(this);
        }
    }
}
