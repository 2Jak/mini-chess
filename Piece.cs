using System;
using System.Collections.Generic;
using System.Text;

namespace MiniChess
{
    public class Piece
    {
        public bool HasMoved { get; set; }
        public char Piecesign { get; set; }
        public Position Position { get; set; }
        public Path MovePath { get; set; }
        public Path AttackPath { get; set; }
        public bool IsWhite { get; set; }
        protected bool biDirectional;

        public Piece() { }
        public Piece(char sign, Position initPos, bool white, bool dir)
        {
            this.HasMoved = false;
            this.Piecesign = sign;
            this.Position = initPos;
            this.IsWhite = white;
            this.biDirectional = dir;
            this.AttackPath = new Path();
            this.MovePath = new Path();
        }

        public virtual bool Move(Position newPos)
        {
            if (this.ValidateMove(newPos))
            {
                GameState.MovePiece(this.Position, newPos, this);
                if (this.HasMoved != true)
                    this.HasMoved = true;
                this.InitializePaths();
                return true;
            }
            return false;    
        }
        public virtual bool ValidateMove(Position newPos) { return false; }
        public virtual void InitializePaths() { }


        #region Diagonal Processing
        protected bool ValidateDiagonalPathMove(bool full, Position newPos)
        {
            if (this.biDirectional)
            {
                if (validateDiagonalPathMove(0, -1, -1, full, newPos))
                    return true;
                else if (validateDiagonalPathMove(2, -1, 1, full, newPos))
                    return true;
                else if (validateDiagonalPathMove(6, 1, -1, full, newPos))
                    return true;
                else if (validateDiagonalPathMove(8, 1, 1, full, newPos))
                    return true;
                return false;
            }
            else
            {
                if (this.IsWhite)
                {
                    if (validateDiagonalPathMove(0, -1, -1, full, newPos))
                        return true;
                    else if (validateDiagonalPathMove(2, -1, 1, full, newPos))
                        return true;
                }
                else
                {
                    if (validateDiagonalPathMove(6, 1, -1, full, newPos))
                        return true;
                    else if (validateDiagonalPathMove(8, 1, 1, full, newPos))
                        return true;
                }
            }
            return false;
        }

        protected bool ValidatedDiagonalPathAttack(bool full, Position newPos)
        {
            if (this.biDirectional)
            {
                if (validateDiagonalPathAttack(0, -1, -1, full, newPos))
                    return true;
                else if (validateDiagonalPathAttack(2, -1, 1, full, newPos))
                    return true;
                else if (validateDiagonalPathAttack(6, 1, -1, full, newPos))
                    return true;
                else if (validateDiagonalPathAttack(8, 1, 1, full, newPos))
                    return true;
                return false;
            }
            else
            {
                if (this.IsWhite)
                {
                    if (validateDiagonalPathAttack(0, -1, -1, full, newPos))
                        return true;
                    else if (validateDiagonalPathAttack(2, -1, 1, full, newPos))
                        return true;
                }
                else
                {
                    if (validateDiagonalPathAttack(6, 1, -1, full, newPos))
                        return true;
                    else if (validateDiagonalPathAttack(8, 1, 1, full, newPos))
                        return true;
                }
            }
            return false;
        }

        protected void ProcessDiagonalPath(bool full)
        {
            if (biDirectional)
            {
                processDiagonalMove(0, -1, -1, full);
                processDiagonalAttack(0, -1, -1, full);
                processDiagonalMove(2, -1, 1, full);
                processDiagonalAttack(2, -1, 1, full);
                processDiagonalMove(6, 1, -1, full);
                processDiagonalAttack(6, 1, -1, full);
                processDiagonalMove(8, 1, 1, full);
                processDiagonalAttack(8, 1, 1, full);
            }
            else
            {
                if (IsWhite)
                {
                    processDiagonalMove(0, -1, -1, full);
                    processDiagonalAttack(0, -1, -1, full);
                    processDiagonalMove(2, -1, 1, full);
                    processDiagonalAttack(2, -1, 1, full);
                }
                else
                {
                    processDiagonalMove(6, 1, -1, full);
                    processDiagonalAttack(6, 1, -1, full);
                    processDiagonalMove(8, 1, 1, full);
                    processDiagonalAttack(8, 1, 1, full);
                }
            }

        }

        private bool validateDiagonalPathMove(int position, int xChange, int yChange, bool full, Position newPos)
        {
            for (int row = this.Position.Row, col = this.Position.Column; checkDiagonalPathBounds(position, row, col, this.MovePath.Paths[position].Row, this.MovePath.Paths[position].Column); row += xChange, col += yChange)
                if (newPos.Equals(new Position(row, col)))
                    if (GameState.Board[row, col] != null)
                    {
                        if (GameState.Board[row, col].IsWhite == this.IsWhite)
                            return false;
                        else
                            return checkCapture(newPos);
                    }
                    else
                        return true;
            return false;
        }

        private bool validateDiagonalPathAttack(int position, int xChange, int yChange, bool full, Position newPos)
        {
            for (int row = this.Position.Row, col = this.Position.Column; checkDiagonalPathBounds(position, row, col, this.AttackPath.Paths[position].Row, this.AttackPath.Paths[position].Column); row += xChange, col += yChange)
                if (newPos.Equals(new Position(row, col)))
                    if (GameState.Board[row, col] != null)
                    {
                        if (GameState.Board[row, col].IsWhite == this.IsWhite)
                            return false;
                        else
                            return checkCapture(newPos);
                    }
                    else
                        return true;
            return false;
        }

        private void processDiagonalMove(int position, int xChange, int yChange, bool full)
        {
            if (full)
            {
                this.MovePath.Paths[position] = this.Position.OnTheFlyChanges(0,0);
                Position move = new Position(this.Position.Row, this.Position.Column);
                while (checkDiagonalBounds(position, move.Row, move.Column))
                {
                    move.Row += xChange;
                    move.Column += yChange;
                    if (GameState.Board[move.Row, move.Column] == null || GameState.Board[move.Row, move.Column] is Ghost)
                        this.MovePath.Paths[position] = new Position(move.Row, move.Column);
                    else if (GameState.Board[move.Row, move.Column].IsWhite != this.IsWhite)
                    {
                        this.MovePath.Paths[position] = new Position(move.Row, move.Column);
                        break;
                    }
                    else
                        break;
                }
            }
            else
            {
                this.MovePath.Paths[position] = this.Position.OnTheFlyChanges(0, 0);
                if (!(this is Pawn))
                    if (checkDiagonalBounds(position, this.Position.Row, this.Position.Column))
                        if (GameState.Board[(this.Position.Row + xChange), (this.Position.Column + yChange)] == null || GameState.Board[(this.Position.Row + xChange), (this.Position.Column + yChange)].IsWhite != this.IsWhite)
                            this.MovePath.Paths[position] = new Position(this.Position.Row + xChange, this.Position.Column + yChange);
            }
        }

        private void processDiagonalAttack(int position, int xChange, int yChange, bool full)
        {
            if (full)
            {
                this.AttackPath.Paths[position] = this.Position.OnTheFlyChanges(0, 0);
                Position move = new Position(this.Position.Row, this.Position.Column);
                while (checkDiagonalBounds(position, move.Row, move.Column))
                {
                    move.Row += xChange;
                    move.Column += yChange;
                    if (GameState.Board[move.Row, move.Column] == null || GameState.Board[move.Row, move.Column] is Ghost)
                        this.AttackPath.Paths[position] = new Position(move.Row, move.Column);
                    else if (GameState.Board[move.Row, move.Column].IsWhite != this.IsWhite)
                    {
                        this.AttackPath.Paths[position] = new Position(move.Row, move.Column);
                        break;
                    }
                    else
                        break;
                }
            }
            else
            {
                this.AttackPath.Paths[position] = this.Position.OnTheFlyChanges(0, 0);
                if (checkDiagonalBounds(position, this.Position.Row, this.Position.Column))
                    if (this is Pawn)
                    {
                        if (GameState.Board[(this.Position.Row + xChange), (this.Position.Column + yChange)] != null && GameState.Board[(this.Position.Row + xChange), (this.Position.Column + yChange)].IsWhite != this.IsWhite)
                            this.AttackPath.Paths[position] = new Position(this.Position.Row + xChange, this.Position.Column + yChange);
                    }
                    else
                    {
                        if (GameState.Board[(this.Position.Row + xChange), (this.Position.Column + yChange)] == null || GameState.Board[(this.Position.Row + xChange), (this.Position.Column + yChange)].IsWhite != this.IsWhite)
                            this.AttackPath.Paths[position] = new Position(this.Position.Row + xChange, this.Position.Column + yChange);
                    }
            }
        }

        private bool checkDiagonalPathBounds(int position, int row, int col, int nxtRow, int nxtCol)
        {
            switch (position)
            {
                case 0:
                    return row >= nxtRow && col >= nxtCol;
                case 2:
                    return row >= nxtRow && col <= nxtCol;
                case 6:
                    return row <= nxtRow && col >= nxtCol;
                case 8:
                    return row <= nxtRow && col <= nxtCol;
                default:
                    return false;
            }
        }

        private bool checkDiagonalBounds(int position, int row, int col)
        {
            switch (position)
            {
                case 0:                   
                    return row >= 1 && col >= 1;
                case 2:
                    return row >= 1 && col < 7;
                case 6:
                    return row < 7 && col >= 1;
                case 8:
                    return row < 7 && col < 7;
                default:
                    return false;
            }
        }
        #endregion

        #region Vertical Processing
        protected bool ValidateVerticalPathAttack(bool full, Position newPos)
        {
            if (this.biDirectional)
            {
                if (validateVerticalPathAttack(1, -1, full, newPos))
                    return true;
                else if (validateVerticalPathAttack(7, 1, full, newPos))
                    return true;
            }
            else
            {
                if (this.IsWhite)
                {
                    if (validateVerticalPathAttack(1, -1, full, newPos))
                        return true;
                }
                else
                    if (validateVerticalPathAttack(7, 1, full, newPos))
                    return true;
            }
            return false;
        }

        protected bool ValidateVerticalPathMove(bool full, Position newPos)
        {

            if (this.biDirectional)
            {
                if (validateVerticalPathMove(1, -1, full, newPos))
                    return true;
                else if (validateVerticalPathMove(7, 1, full, newPos))
                    return true;
            }
            else
            {
                if (this.IsWhite)
                {
                    if (validateVerticalPathMove(1, -1, full, newPos))
                        return true;
                }           
                else
                    if (validateVerticalPathMove(7, 1, full, newPos))
                        return true;
            }
            return false;
        }

        protected void ProcessVerticalPath(bool full)
        {
            if (this.biDirectional)
            {
                processVerticalMove(1, -1, full);
                processVerticalAttack(1, -1, full);
                processVerticalMove(7, 1, full);
                processVerticalAttack(7, 1, full);
            }
            else
            {
                if (this.IsWhite)
                {
                    processVerticalMove(1, -1, full);
                    processVerticalAttack(1, -1, full);
                }
                else
                {
                    processVerticalMove(7, 1, full);
                    processVerticalAttack(7, 1, full);
                }
            }
        }

        private bool validateVerticalPathMove(int position, int xChange, bool full, Position newPos)
        {
            for (int row = this.Position.Row, col = this.Position.Column; checkVerticalPathBounds(position, row, this.MovePath.Paths[position].Row); row += xChange)
                if (newPos.Equals(new Position(row, col)))
                    if (GameState.Board[row, col] != null)
                    {
                        if (GameState.Board[row, col].IsWhite == this.IsWhite)
                            return false;
                        else
                        {
                            if (this is Pawn)
                                return false;
                            else
                                return checkCapture(newPos);
                        }
                            
                    }
                    else
                        return true;

            return false;
        }

        private bool validateVerticalPathAttack(int position, int xChange, bool full, Position newPos)
        {
            for (int row = this.Position.Row, col = this.Position.Column; checkVerticalPathBounds(position, row, this.AttackPath.Paths[position].Row); row += xChange)
                if (newPos.Equals(new Position(row, col)))
                    if (GameState.Board[row, col] != null)
                    {
                        if (GameState.Board[row, col].IsWhite == this.IsWhite)
                            return false;
                        else
                            return checkCapture(newPos);
                    }
                    else
                        return true;
            return false;
        }

        private void processVerticalMove(int position, int xChange, bool full)
        {
            if (full)
            {
                this.MovePath.Paths[position] = this.Position.OnTheFlyChanges(0, 0);
                Position move = new Position(this.Position.Row, this.Position.Column);
                while (checkVerticalBounds(position, move.Row))
                {
                    move.Row += xChange;
                    if (GameState.Board[move.Row, move.Column] == null || GameState.Board[move.Row, move.Column] is Ghost)
                        this.MovePath.Paths[position] = new Position(move.Row, move.Column);
                    else if (GameState.Board[move.Row, move.Column].IsWhite != this.IsWhite)
                    {
                        this.MovePath.Paths[position] = new Position(move.Row, move.Column);
                        break;
                    }
                    else
                        break;
                }
            }
            else
            {
                this.MovePath.Paths[position] = this.Position.OnTheFlyChanges(0, 0);
                if (checkVerticalBounds(position, this.Position.Row))
                    if (GameState.Board[(this.Position.Row + xChange), this.Position.Column] == null || GameState.Board[(this.Position.Row + xChange), this.Position.Column].IsWhite != this.IsWhite || GameState.Board[this.Position.Row + xChange, this.Position.Column] is Ghost)
                        this.MovePath.Paths[position] = new Position(this.Position.Row + xChange, this.Position.Column);
            }
        }

        private void processVerticalAttack(int position, int xChange, bool full)
        {
            if (full)
            {
                this.AttackPath.Paths[position] = this.Position.OnTheFlyChanges(0, 0);
                Position move = new Position(this.Position.Row, this.Position.Column);
                while (checkVerticalBounds(position, move.Row))
                {
                    move.Row += xChange;
                    if (GameState.Board[move.Row, move.Column] == null || GameState.Board[move.Row, move.Column] is Ghost)
                        this.AttackPath.Paths[position] = new Position(move.Row, move.Column);
                    else if (GameState.Board[move.Row, move.Column].IsWhite != this.IsWhite)
                    {
                        this.AttackPath.Paths[position] = new Position(move.Row, move.Column);
                        break;
                    }
                    else
                        break;
                }
            }
            else
            {
                this.AttackPath.Paths[position] = this.Position.OnTheFlyChanges(0, 0);
                if (!(this is Pawn))
                    if (checkVerticalBounds(position, this.Position.Row))
                        if (GameState.Board[(this.Position.Row + xChange), this.Position.Column] == null || GameState.Board[(this.Position.Row + xChange), this.Position.Column].IsWhite != this.IsWhite || GameState.Board[this.Position.Row + xChange, this.Position.Column] is Ghost)
                            this.AttackPath.Paths[position] = new Position(this.Position.Row + xChange, this.Position.Column);
            }
        }

        private bool checkVerticalPathBounds(int position, int row, int nxtRow)
        {
            switch (position)
            {
                case 1:
                    return row >= nxtRow;
                case 7:
                    return row <= nxtRow;
                default:
                    return false;
            }
        }

        private bool checkVerticalBounds(int position, int row)
        {
            switch (position)
            {
                case 1:
                    return row >= 1;
                case 7:
                    return row < 7;
                default:
                    return false;
            }
        }
        #endregion

        #region Horizontal Processing
        protected bool ValidateHorizontalPathAttack(bool full, Position newPos)
        {
            if (validateHorizontalPathAttack(3, -1, full, newPos))
                return true;
            else if (validateHorizontalPathAttack(5, 1, full, newPos))
                return true;
            return false;
        }

        protected bool ValidateHorizontalPathMove(bool full, Position newPos)
        {
            if (validateHorizontalPathMove(3, -1, full, newPos))
                return true;
            else if (validateHorizontalPathMove(5, 1, full, newPos))
                return true;
            return false;
        }

        protected void ProcessHorizontalPath(bool full)
        {
            processHorizontalMove(3, -1, full);
            processHorizontalAttack(3, -1, full);
            processHorizontalMove(5, 1, full);
            processHorizontalAttack(5, 1, full);
        }

        private bool validateHorizontalPathMove(int position, int yChange, bool full, Position newPos)
        {
            for (int row = this.Position.Row, col = this.Position.Column; checkHorizontalPathBounds(position, col, this.MovePath.Paths[position].Column); col += yChange)
                if (newPos.Equals(new Position(row, col)))
                    if (GameState.Board[row, col] != null)
                    {
                        if (GameState.Board[row, col].IsWhite == this.IsWhite)
                            return false;
                        else
                            return checkCapture(newPos);
                    }
                    else
                        return true;
            return false;
        }

        private bool validateHorizontalPathAttack(int position, int yChange, bool full, Position newPos)
        {
            for (int row = this.Position.Row, col = this.Position.Column; checkHorizontalPathBounds(position, col, this.AttackPath.Paths[position].Column); col += yChange)
                if (newPos.Equals(new Position(row, col)))
                    if (GameState.Board[row, col] != null)
                    {
                        if (GameState.Board[row, col].IsWhite == this.IsWhite)
                            return false;
                        else
                            return checkCapture(newPos);
                    }
                    else
                        return true;
            return false;
        }

        private void processHorizontalMove(int position, int yChange, bool full)
        {
            if (full)
            {
                this.MovePath.Paths[position] = this.Position.OnTheFlyChanges(0, 0);
                Position move = new Position(this.Position.Row, this.Position.Column);
                while (checkHorizontalBounds(position, move.Column))
                {
                    move.Column += yChange;
                    if (GameState.Board[move.Row, move.Column] == null || GameState.Board[move.Row, move.Column] is Ghost)
                        this.MovePath.Paths[position] = new Position(move.Row, move.Column);
                    else if (GameState.Board[move.Row, move.Column].IsWhite != this.IsWhite)
                    {
                        this.MovePath.Paths[position] = new Position(move.Row, move.Column);
                        break;
                    }
                    else
                        break;
                }
            }
            else
            {
                this.MovePath.Paths[position] = this.Position.OnTheFlyChanges(0, 0);
                if (checkHorizontalBounds(position, this.Position.Column))
                    if (GameState.Board[this.Position.Row, (this.Position.Column + yChange)] == null || GameState.Board[this.Position.Row, (this.Position.Column + yChange)].IsWhite != this.IsWhite || GameState.Board[this.Position.Row, this.Position.Column + yChange] is Ghost)
                        this.MovePath.Paths[position] = new Position(this.Position.Row, this.Position.Column + yChange);
            }
        }

        private void processHorizontalAttack(int position, int yChange, bool full)
        {
            if (full)
            {
                this.AttackPath.Paths[position] = this.Position.OnTheFlyChanges(0, 0);
                Position move = new Position(this.Position.Row, this.Position.Column);
                while (checkHorizontalBounds(position, move.Column))
                {
                    move.Column += yChange;
                    if (GameState.Board[move.Row, move.Column] == null || GameState.Board[move.Row, move.Column] is Ghost)
                        this.AttackPath.Paths[position] = new Position(move.Row, move.Column);
                    else if (GameState.Board[move.Row, move.Column].IsWhite != this.IsWhite)
                    {
                        this.AttackPath.Paths[position] = new Position(move.Row, move.Column);
                        break;
                    }
                    else
                        break;
                }
            }
            else
            {
                this.AttackPath.Paths[position] = this.Position.OnTheFlyChanges(0, 0);
                if (checkHorizontalBounds(position, this.Position.Column))
                    if (GameState.Board[this.Position.Row, (this.Position.Column + yChange)] == null || GameState.Board[this.Position.Row, (this.Position.Column + yChange)].IsWhite != this.IsWhite || GameState.Board[this.Position.Row, this.Position.Column + yChange] is Ghost)
                        this.AttackPath.Paths[position] = new Position(this.Position.Row, this.Position.Column + yChange);
            }
        }

        private bool checkHorizontalPathBounds(int position, int col, int nxtCol)
        {
            switch (position)
            {
                case 3:
                    return col >= nxtCol;
                case 5:
                    return col <= nxtCol;
                default:
                    return false;
            }
        }

        private bool checkHorizontalBounds(int position, int col)
        {
            switch (position)
            {
                case 3:
                    return col >= 1;
                case 5:
                    return col < 7;
                default:
                    return false;
            }
        }
        #endregion

        #region Knight Processing
        protected void ProcessKnightPaths()
        {
            processKnightPathAttack();
            processKnightPathMove();
        }

        protected bool ValidateKnightAttack(Position newPos)
        {
            foreach (Position pathPos in this.AttackPath.Paths)
                if (pathPos != null && pathPos.Equals(newPos))
                    if (GameState.Board[newPos.Row, newPos.Column] != null)
                    {
                        if (GameState.Board[newPos.Row, newPos.Column].IsWhite == this.IsWhite)
                            return false;
                        else
                            return checkCapture(newPos);
                    }
                    else
                        return true;
            return false;
        }

        protected bool ValidateKnightMove(Position newPos)
        {
            foreach (Position pathPos in this.MovePath.Paths)
                if (pathPos != null && pathPos.Equals(newPos))
                    if (GameState.Board[newPos.Row, newPos.Column] != null)
                    {
                        if (GameState.Board[newPos.Row, newPos.Column].IsWhite == this.IsWhite)
                            return false;
                        else
                            return checkCapture(newPos);
                    }
                    else
                        return true;
            return false;
        }

        private void processKnightPathAttack()
        {
            for (int i = 0; i < 8; i++)
            {
                Position newKnightPos = this.Position.OnTheFlyChanges(Knight.PositionChangers[i][0], Knight.PositionChangers[i][1]);
                if (newKnightPos.Row >= 0 && newKnightPos.Row <= 7 && newKnightPos.Column >= 0 && newKnightPos.Column <= 7)
                    if (GameState.Board[newKnightPos.Row, newKnightPos.Column] != null)
                    {
                        if (!(GameState.Board[newKnightPos.Row, newKnightPos.Column] is Ghost))
                        {
                            if (GameState.Board[newKnightPos.Row, newKnightPos.Column].IsWhite != this.IsWhite)
                                this.AttackPath.Paths[i] = newKnightPos;
                        }
                        else this.AttackPath.Paths[i] = newKnightPos;
                    }
                    else
                        this.AttackPath.Paths[i] = newKnightPos;
                else
                {
                    this.AttackPath.Paths[i] = this.Position.OnTheFlyChanges(0, 0);
                }
            }
            this.AttackPath.Paths[8] = this.Position.OnTheFlyChanges(0, 0);
        }

        private void processKnightPathMove()
        {
            for (int i = 0; i < 8; i++)
            {
                Position newKnightPos = this.Position.OnTheFlyChanges(Knight.PositionChangers[i][0], Knight.PositionChangers[i][1]);
                if (newKnightPos.Row >= 0 && newKnightPos.Row <= 7 && newKnightPos.Column >= 0 && newKnightPos.Column <= 7)
                    if (GameState.Board[newKnightPos.Row, newKnightPos.Column] != null)
                    {
                        if (!(GameState.Board[newKnightPos.Row, newKnightPos.Column] is Ghost))
                        {
                            if (GameState.Board[newKnightPos.Row, newKnightPos.Column].IsWhite != this.IsWhite)
                                this.MovePath.Paths[i] = newKnightPos;
                        }
                        else this.MovePath.Paths[i] = newKnightPos;
                    }
                    else
                        this.MovePath.Paths[i] = newKnightPos;
                else
                {
                    this.MovePath.Paths[i] = this.Position.OnTheFlyChanges(0, 0);
                }
            }
            this.MovePath.Paths[8] = this.Position.OnTheFlyChanges(0, 0);
        }
        #endregion

        protected void ProcessDefault()
        {
            this.MovePath.Paths[4] = this.Position.OnTheFlyChanges(0, 0);
            this.AttackPath.Paths[4] = this.Position.OnTheFlyChanges(0, 0);
        }

        private bool checkCapture(Position destenation)
        {
            if (GameState.Board[destenation.Row, destenation.Column] != null)
                if (this is King)
                {
                    if (GameState.Board[destenation.Row, destenation.Column] is King)
                        return false;
                    else if (((King)this).isUnderCheckCheck)
                    {
                        return true;
                    }
                    else
                    {
                        return capture(destenation);
                    }
                }
                else
                {
                    return capture(destenation);
                }
            else
                return true;
        }

        private bool capture(Position destenation)
        {
            if (GameState.Board[destenation.Row, destenation.Column] is Ghost && this is Pawn)
            {
                GameState.RemovePawn(((Ghost)GameState.Board[destenation.Row, destenation.Column]).PawnSoul);
                GameState.noCaptures = 0;
                GameState.ResetRepeats();
                if (this.IsWhite)
                    GameState.blackPieceCount--;
                else
                    GameState.whitePieceCount--;
                return true;
            }
            else if (!(GameState.Board[destenation.Row, destenation.Column] is Ghost))
            {
                GameState.noCaptures = 0;
                GameState.ResetRepeats();
                if (this.IsWhite)
                    GameState.blackPieceCount--;
                else
                    GameState.whitePieceCount--;
                return true;
            }
            else
                return true;
        }

        public virtual Piece GetCopy()
        {
            if (this != null && !(this is Ghost))
            {
                Piece pieceCopy = new Piece();
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
    }
}
