using System;
using System.Collections.Generic;
using System.Text;

namespace MiniChess
{
    public class King : Piece
    {
        private Rook castlingRook = null;
        internal bool isUnderCheckCheck = false;

        public bool UnderCheck { get; set; }

        public King(bool white) : base(((white) ? (char)9818 : (char)9812), (white) ? new Position(7, 4) : new Position(0, 4), white, true) { }



        public override string ToString()
        {
            return string.Format("{0}K", IsWhite ? "W" : "B");
        }

        public override bool ValidateMove(Position newPos, GameState state)
        {
            if (base.ValidateDiagonalPathMove(false, newPos, state))
            {
                if (!validateNotNext2King(newPos, state) && (validateReachbleByEnemy(newPos, state) == null))
                    return true;
            }
            else if (base.ValidateHorizontalPathMove(false, newPos, state))
            {
                if (!validateNotNext2King(newPos, state) && (validateReachbleByEnemy(newPos, state) == null))
                    return true;
            }
            else if (base.ValidateVerticalPathMove(false, newPos, state))
            {
                if (!validateNotNext2King(newPos, state) && (validateReachbleByEnemy(newPos, state) == null))
                    return true;
            }
            else
                return false;
            return false;
        }

        public bool ValidateOutOfMove(Position newPos, GameState state)
        {
            if (state.GetBoard()[newPos.Row, newPos.Column] != null && state.GetBoard()[newPos.Row, newPos.Column].IsWhite == this.IsWhite)
                return true;
            else
            {
                isUnderCheckCheck = true;
                bool validMove = ValidateMove(newPos, state);
                isUnderCheckCheck = false;
                return validMove && validateAreaUnderThreat(newPos, state);
            }               
        }

        public override bool Move(Position newPos, GameState state)
        {
            if (checkIfMoveCastling(newPos))
            {
                if (validateCastling(newPos, state))
                {
                    if (newPos.Column == 6)
                    {
                        state.MovePiece(this.Position, newPos, this);
                        state.MovePiece(castlingRook.Position, newPos.OnTheFlyChanges(0, -1), castlingRook);
                        castlingRook.HasMoved = true;
                        this.HasMoved = true;
                        castlingRook.InitializePaths(state);
                        castlingRook = null;
                        this.InitializePaths(state);
                        state.ResetTurnLog();
                        return true;
                    }
                    else
                    {
                        state.MovePiece(this.Position, newPos, this);
                        state.MovePiece(castlingRook.Position, newPos.OnTheFlyChanges(0, 1), castlingRook);
                        castlingRook.HasMoved = true;
                        this.HasMoved = true;
                        castlingRook.InitializePaths(state);
                        this.InitializePaths(state);
                        castlingRook = null;
                        state.ResetTurnLog();
                        return true;
                    }
                }
                else
                {
                    if (ValidateMove(newPos, state))
                    {
                        state.MovePiece(this.Position, newPos, this);
                        if (this.HasMoved != true)
                            this.HasMoved = true;
                        this.InitializePaths(state);
                        return true;
                    }
                    else
                        return false;
                }
            }
            else
            {
                if (ValidateMove(newPos, state))
                {
                    state.MovePiece(this.Position, newPos, this);
                    if (this.HasMoved != true)
                        this.HasMoved = true;
                    this.InitializePaths(state);
                    return true;
                }
                else
                    return false;
            }           
        }

        public override void InitializePaths(GameState state)
        {
            this.ProcessDiagonalPath(false, state);
            this.ProcessHorizontalPath(false, state);
            this.ProcessVerticalPath(false, state);
            this.ProcessDefault();
        }

        public override Piece GetCopy()
        {
            King pieceCopy = new King(this.IsWhite);
            pieceCopy.HasMoved = this.HasMoved;
            pieceCopy.Piecesign = this.Piecesign;
            pieceCopy.Position = this.Position;
            pieceCopy.AttackPath = this.AttackPath;
            pieceCopy.MovePath = this.MovePath;
            pieceCopy.IsWhite = this.IsWhite;
            pieceCopy.biDirectional = this.biDirectional;
            return pieceCopy;
        }


        #region King's Special Checks
        private bool validateAreaUnderThreat(Position newPos, GameState state)
        {
            foreach (Piece piece in state.GetBoard())
                if (piece != null && piece.IsWhite != this.IsWhite && !(piece is Ghost))
                    foreach (Position piecePos in piece.AttackPath.Paths)
                        if (piecePos != null)
                            if (newPos.Equals(piecePos))
                                return true;
            return false;
        }

        private Position validateReachbleByEnemy(Position newPos, GameState state)
        {
            foreach (Piece piece in state.GetBoard())
                if (piece != null && piece.IsWhite != this.IsWhite && !(piece is Ghost))
                    if (piece.ValidateMove(newPos,state))
                        return piece.Position.OnTheFlyChanges(0, 0);
            return null;
        }

        private bool validateIfAreaProtectable(Position newPos, GameState state)
        {
            if (newPos != null)
                foreach (Piece piece in state.GetBoard())
                    if (piece != null && piece.IsWhite == this.IsWhite && !(piece is Ghost) && !(piece is King))
                        foreach (Position piecePos in piece.MovePath.Paths)
                            if (piecePos != null)
                                if (newPos.Equals(piecePos))
                                    return true;
            return false;
        }

        private bool validateNotNext2King(Position newPos, GameState state)
        {
            foreach (Piece piece in state.GetBoard())
                if (piece != null && piece.IsWhite != this.IsWhite && piece is King)
                    foreach (Position piecePos in piece.AttackPath.Paths)
                        if (piecePos != null)
                            if (newPos.Equals(piecePos))
                                return true;
            return false;
        }

        public bool CheckForMate(GameState state)
        {
            int checkableCount = 0;
            int differentPositionsCount = 1;
            int deffendableCount = 1;
            if (validateAreaUnderThreat(this.Position, state))
                checkableCount++;
            foreach (Position kingPos in this.MovePath.Paths)
                if (!kingPos.Equals(this.Position) && (validateAreaUnderThreat(kingPos, state) || validateIfAreaProtectable(validateReachbleByEnemy(kingPos, state),state)))
                {
                    checkableCount++;
                    differentPositionsCount++;
                }
                else if (!kingPos.Equals(this.Position))
                    checkableCount--;
            foreach (Position kingPos in this.MovePath.Paths)
                if (!kingPos.Equals(this.Position) && validateIfAreaProtectable(kingPos, state))
                    deffendableCount++;
            return (checkableCount >= differentPositionsCount && checkableCount > deffendableCount) ? true : false;
        }

        public void UpdateCheckState(GameState state)
        {
            foreach (Piece piece in state.GetBoard())
                if (piece != null && piece.IsWhite != this.IsWhite && !(piece is Ghost))
                    foreach (Position piecePos in piece.AttackPath.Paths)
                        if (piecePos != null)
                            if (this.Position.Equals(piecePos))
                            {
                                this.UnderCheck = true;
                                return;
                            }
            this.UnderCheck = false;
        }

        #region Castling Checks
        bool checkIfMoveCastling(Position newPos)
        {
            if (this.IsWhite)
            {
                if (newPos.Equals(new Position(7, 2)) || newPos.Equals(new Position(7, 6)))
                    return true;
                else
                    return false;
            }
            else
            {
                if (newPos.Equals(new Position(0, 2)) || newPos.Equals(new Position(0, 6)))
                    return true;
                else
                    return false;
            }
        }

        bool validateCastling(Position castlingPos, GameState state)
        {
            if (validateCastlingIsRook(castlingPos.Column, castlingPos.Row, state) && validateCastlingAreaClear(castlingPos.Column, castlingPos.Row, state) && !this.HasMoved)
                return true;
            return false;
        }

        bool validateCastlingIsRook(int column, int row, GameState state)
        {
            column = (column == 6) ? column + 1 : column - 2;
            if (state.GetBoard()[row, column] is Rook && state.GetBoard()[row, column] != null && state.GetBoard()[row, column].IsWhite == this.IsWhite && !state.GetBoard()[row, column].HasMoved)
            {
                castlingRook = (Rook)state.GetBoard()[row, column];
                return true;
            }              
            return false;
        }

        bool validateCastlingAreaClear(int column, int row, GameState state)
        {
            if (column == 6)
            {
                for (int i = 3; i > 0; i--, column--)
                    if (validateAreaUnderThreat(new Position(row, column), state) || (state.GetBoard()[row, column] != null && !this.Position.Equals(new Position(row,column))))
                        return false;
            }
            else
                for (int i = 0; i < 3; i++, column++)
                    if (validateAreaUnderThreat(new Position(row, column), state) || (state.GetBoard()[row, column] != null && !this.Position.Equals(new Position(row, column))))
                        return false;
            return true;
        }
        #endregion
        #endregion
    }
}
