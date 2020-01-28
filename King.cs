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

        public override bool ValidateMove(Position newPos)
        {
            if (base.ValidateDiagonalPathMove(false, newPos))
            {
                if (!validateNotNext2King(newPos) && (validateReachbleByEnemy(newPos) == null))
                    return true;
            }
            else if (base.ValidateHorizontalPathMove(false, newPos))
            {
                if (!validateNotNext2King(newPos) && (validateReachbleByEnemy(newPos) == null))
                    return true;
            }
            else if (base.ValidateVerticalPathMove(false, newPos))
            {
                if (!validateNotNext2King(newPos) && (validateReachbleByEnemy(newPos) == null))
                    return true;
            }
            else
                return false;
            return false;
        }

        public bool ValidateOutOfMove(Position newPos)
        {
            if (GameState.Board[newPos.Row, newPos.Column] != null && GameState.Board[newPos.Row, newPos.Column].IsWhite == this.IsWhite)
                return true;
            else
            {
                isUnderCheckCheck = true;
                bool validMove = ValidateMove(newPos);
                isUnderCheckCheck = false;
                return validMove && validateAreaUnderThreat(newPos);
            }               
        }

        public override bool Move(Position newPos)
        {
            if (checkIfMoveCastling(newPos))
            {
                if (validateCastling(newPos))
                {
                    if (newPos.Column == 6)
                    {
                        GameState.MovePiece(this.Position, newPos, this);
                        GameState.MovePiece(castlingRook.Position, newPos.OnTheFlyChanges(0, -1), castlingRook);
                        castlingRook.HasMoved = true;
                        this.HasMoved = true;
                        castlingRook.InitializePaths();
                        castlingRook = null;
                        this.InitializePaths();
                        GameState.ResetTurnLog();
                        return true;
                    }
                    else
                    {
                        GameState.MovePiece(this.Position, newPos, this);
                        GameState.MovePiece(castlingRook.Position, newPos.OnTheFlyChanges(0, 1), castlingRook);
                        castlingRook.HasMoved = true;
                        this.HasMoved = true;
                        castlingRook.InitializePaths();
                        this.InitializePaths();
                        castlingRook = null;
                        GameState.ResetTurnLog();
                        return true;
                    }
                }
                else
                {
                    if (ValidateMove(newPos))
                    {
                        GameState.MovePiece(this.Position, newPos, this);
                        if (this.HasMoved != true)
                            this.HasMoved = true;
                        this.InitializePaths();
                        return true;
                    }
                    else
                        return false;
                }
            }
            else
            {
                if (ValidateMove(newPos))
                {
                    GameState.MovePiece(this.Position, newPos, this);
                    if (this.HasMoved != true)
                        this.HasMoved = true;
                    this.InitializePaths();
                    return true;
                }
                else
                    return false;
            }           
        }

        public override void InitializePaths()
        {
            this.ProcessDiagonalPath(false);
            this.ProcessHorizontalPath(false);
            this.ProcessVerticalPath(false);
            this.ProcessDefault();
        }

        public override Piece GetCopy()
        {
            if (this != null)
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
            else
                return this;
        }


        #region King's Special Checks
        private bool validateAreaUnderThreat(Position newPos)
        {
            foreach (Piece piece in GameState.Board)
                if (piece != null && piece.IsWhite != this.IsWhite && !(piece is Ghost))
                    foreach (Position piecePos in piece.AttackPath.Paths)
                        if (piecePos != null)
                            if (newPos.Equals(piecePos))
                                return true;
            return false;
        }

        private Position validateReachbleByEnemy(Position newPos)
        {
            foreach (Piece piece in GameState.Board)
                if (piece != null && piece.IsWhite != this.IsWhite && !(piece is Ghost))
                    if (piece.ValidateMove(newPos))
                        return piece.Position.OnTheFlyChanges(0, 0);
            return null;
        }

        private bool validateIfAreaProtectable(Position newPos)
        {
            if (newPos != null)
                foreach (Piece piece in GameState.Board)
                    if (piece != null && piece.IsWhite == this.IsWhite && !(piece is Ghost) && !(piece is King))
                        foreach (Position piecePos in piece.MovePath.Paths)
                            if (piecePos != null)
                                if (newPos.Equals(piecePos))
                                    return true;
            return false;
        }

        private bool validateNotNext2King(Position newPos)
        {
            foreach (Piece piece in GameState.Board)
                if (piece != null && piece.IsWhite != this.IsWhite && piece is King)
                    foreach (Position piecePos in piece.AttackPath.Paths)
                        if (piecePos != null)
                            if (newPos.Equals(piecePos))
                                return true;
            return false;
        }

        public bool CheckForMate()
        {
            int checkableCount = 0;
            int differentPositionsCount = 1;
            int deffendableCount = 1;
            if (validateAreaUnderThreat(this.Position))
                checkableCount++;
            foreach (Position kingPos in this.MovePath.Paths)
                if (!kingPos.Equals(this.Position) && (validateAreaUnderThreat(kingPos) || validateIfAreaProtectable(validateReachbleByEnemy(kingPos))))
                {
                    checkableCount++;
                    differentPositionsCount++;
                }
                else if (!kingPos.Equals(this.Position))
                    checkableCount--;
            foreach (Position kingPos in this.MovePath.Paths)
                if (!kingPos.Equals(this.Position) && validateIfAreaProtectable(kingPos))
                    deffendableCount++;
            return (checkableCount >= differentPositionsCount && checkableCount > deffendableCount) ? true : false;
        }

        public void UpdateCheckState()
        {
            foreach (Piece piece in GameState.Board)
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

        bool validateCastling(Position castlingPos)
        {
            if (validateCastlingIsRook(castlingPos.Column, castlingPos.Row) && validateCastlingAreaClear(castlingPos.Column, castlingPos.Row) && !this.HasMoved)
                return true;
            return false;
        }

        bool validateCastlingIsRook(int column, int row)
        {
            column = (column == 6) ? column + 1 : column - 2;
            if (GameState.Board[row, column] is Rook && GameState.Board[row, column] != null && GameState.Board[row, column].IsWhite == this.IsWhite && !GameState.Board[row, column].HasMoved)
            {
                castlingRook = (Rook)GameState.Board[row, column];
                return true;
            }              
            return false;
        }

        bool validateCastlingAreaClear(int column, int row)
        {
            if (column == 6)
            {
                for (int i = 3; i > 0; i--, column--)
                    if (validateAreaUnderThreat(new Position(row, column)) || (GameState.Board[row, column] != null && !this.Position.Equals(new Position(row,column))))
                        return false;
            }
            else
                for (int i = 0; i < 3; i++, column++)
                    if (validateAreaUnderThreat(new Position(row, column)) || (GameState.Board[row, column] != null && !this.Position.Equals(new Position(row, column))))
                        return false;
            return true;
        }
        #endregion
        #endregion
    }
}
