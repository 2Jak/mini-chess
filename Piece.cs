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

        public virtual bool Move(Position newPos, GameState state)
        {
            if (this.ValidateMove(newPos, state))
            {
                state.MovePiece(this.Position, newPos, this);
                if (this.HasMoved != true)
                    this.HasMoved = true;
                this.InitializePaths(state);
                return true;
            }
            return false;    
        }
        public virtual bool ValidateMove(Position newPos, GameState state) { return false; }
        public virtual void InitializePaths(GameState state) { }


        #region Diagonal Processing
        protected bool ValidateDiagonalPathMove(bool full, Position newPos, GameState state)
        {
            if (this.biDirectional)
            {
                if (validateDiagonalPathMove(0, -1, -1, full, newPos, state))
                    return true;
                else if (validateDiagonalPathMove(2, -1, 1, full, newPos, state))
                    return true;
                else if (validateDiagonalPathMove(6, 1, -1, full, newPos, state))
                    return true;
                else if (validateDiagonalPathMove(8, 1, 1, full, newPos, state))
                    return true;
                return false;
            }
            else
            {
                if (this.IsWhite)
                {
                    if (validateDiagonalPathMove(0, -1, -1, full, newPos, state))
                        return true;
                    else if (validateDiagonalPathMove(2, -1, 1, full, newPos, state))
                        return true;
                }
                else
                {
                    if (validateDiagonalPathMove(6, 1, -1, full, newPos, state))
                        return true;
                    else if (validateDiagonalPathMove(8, 1, 1, full, newPos, state))
                        return true;
                }
            }
            return false;
        }

        protected bool ValidatedDiagonalPathAttack(bool full, Position newPos, GameState state)
        {
            if (this.biDirectional)
            {
                if (validateDiagonalPathAttack(0, -1, -1, full, newPos, state))
                    return true;
                else if (validateDiagonalPathAttack(2, -1, 1, full, newPos, state))
                    return true;
                else if (validateDiagonalPathAttack(6, 1, -1, full, newPos, state))
                    return true;
                else if (validateDiagonalPathAttack(8, 1, 1, full, newPos, state))
                    return true;
                return false;
            }
            else
            {
                if (this.IsWhite)
                {
                    if (validateDiagonalPathAttack(0, -1, -1, full, newPos, state))
                        return true;
                    else if (validateDiagonalPathAttack(2, -1, 1, full, newPos, state))
                        return true;
                }
                else
                {
                    if (validateDiagonalPathAttack(6, 1, -1, full, newPos, state))
                        return true;
                    else if (validateDiagonalPathAttack(8, 1, 1, full, newPos, state))
                        return true;
                }
            }
            return false;
        }

        protected void ProcessDiagonalPath(bool full, GameState state)
        {
            if (biDirectional)
            {
                processDiagonalMove(0, -1, -1, full, state);
                processDiagonalAttack(0, -1, -1, full, state);
                processDiagonalMove(2, -1, 1, full, state);
                processDiagonalAttack(2, -1, 1, full, state);
                processDiagonalMove(6, 1, -1, full, state);
                processDiagonalAttack(6, 1, -1, full, state);
                processDiagonalMove(8, 1, 1, full, state);
                processDiagonalAttack(8, 1, 1, full, state);
            }
            else
            {
                if (IsWhite)
                {
                    processDiagonalMove(0, -1, -1, full, state);
                    processDiagonalAttack(0, -1, -1, full, state);
                    processDiagonalMove(2, -1, 1, full, state);
                    processDiagonalAttack(2, -1, 1, full, state);
                }
                else
                {
                    processDiagonalMove(6, 1, -1, full, state);
                    processDiagonalAttack(6, 1, -1, full, state);
                    processDiagonalMove(8, 1, 1, full, state);
                    processDiagonalAttack(8, 1, 1, full, state);
                }
            }

        }

        private bool validateDiagonalPathMove(int position, int xChange, int yChange, bool full, Position newPos, GameState state)
        {
            for (int row = this.Position.Row, col = this.Position.Column; checkDiagonalPathBounds(position, row, col, this.MovePath.Paths[position].Row, this.MovePath.Paths[position].Column); row += xChange, col += yChange)
                if (newPos.Equals(new Position(row, col)))
                    if (state.GetBoard()[row, col] != null)
                    {
                        if (state.GetBoard()[row, col].IsWhite == this.IsWhite)
                            return false;
                        else
                            return checkCapture(newPos, state);
                    }
                    else
                        return true;
            return false;
        }

        private bool validateDiagonalPathAttack(int position, int xChange, int yChange, bool full, Position newPos, GameState state)
        {
            for (int row = this.Position.Row, col = this.Position.Column; checkDiagonalPathBounds(position, row, col, this.AttackPath.Paths[position].Row, this.AttackPath.Paths[position].Column); row += xChange, col += yChange)
                if (newPos.Equals(new Position(row, col)))
                    if (state.GetBoard()[row, col] != null)
                    {
                        if (state.GetBoard()[row, col].IsWhite == this.IsWhite)
                            return false;
                        else
                            return checkCapture(newPos, state);
                    }
                    else
                        return true;
            return false;
        }

        private void processDiagonalMove(int position, int xChange, int yChange, bool full, GameState state)
        {
            if (full)
            {
                this.MovePath.Paths[position] = this.Position.OnTheFlyChanges(0,0);
                Position move = new Position(this.Position.Row, this.Position.Column);
                while (checkDiagonalBounds(position, move.Row, move.Column))
                {
                    move.Row += xChange;
                    move.Column += yChange;
                    if (state.GetBoard()[move.Row, move.Column] == null || state.GetBoard()[move.Row, move.Column] is Ghost)
                        this.MovePath.Paths[position] = new Position(move.Row, move.Column);
                    else if (state.GetBoard()[move.Row, move.Column].IsWhite != this.IsWhite)
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
                        if (state.GetBoard()[(this.Position.Row + xChange), (this.Position.Column + yChange)] == null || state.GetBoard()[(this.Position.Row + xChange), (this.Position.Column + yChange)].IsWhite != this.IsWhite)
                            this.MovePath.Paths[position] = new Position(this.Position.Row + xChange, this.Position.Column + yChange);
            }
        }

        private void processDiagonalAttack(int position, int xChange, int yChange, bool full, GameState state)
        {
            if (full)
            {
                this.AttackPath.Paths[position] = this.Position.OnTheFlyChanges(0, 0);
                Position move = new Position(this.Position.Row, this.Position.Column);
                while (checkDiagonalBounds(position, move.Row, move.Column))
                {
                    move.Row += xChange;
                    move.Column += yChange;
                    if (state.GetBoard()[move.Row, move.Column] == null || state.GetBoard()[move.Row, move.Column] is Ghost)
                        this.AttackPath.Paths[position] = new Position(move.Row, move.Column);
                    else if (state.GetBoard()[move.Row, move.Column].IsWhite != this.IsWhite)
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
                        if (state.GetBoard()[(this.Position.Row + xChange), (this.Position.Column + yChange)] != null && state.GetBoard()[(this.Position.Row + xChange), (this.Position.Column + yChange)].IsWhite != this.IsWhite)
                            this.AttackPath.Paths[position] = new Position(this.Position.Row + xChange, this.Position.Column + yChange);
                    }
                    else
                    {
                        if (state.GetBoard()[(this.Position.Row + xChange), (this.Position.Column + yChange)] == null || state.GetBoard()[(this.Position.Row + xChange), (this.Position.Column + yChange)].IsWhite != this.IsWhite)
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
        protected bool ValidateVerticalPathAttack(bool full, Position newPos, GameState state)
        {
            if (this.biDirectional)
            {
                if (validateVerticalPathAttack(1, -1, full, newPos, state))
                    return true;
                else if (validateVerticalPathAttack(7, 1, full, newPos, state))
                    return true;
            }
            else
            {
                if (this.IsWhite)
                {
                    if (validateVerticalPathAttack(1, -1, full, newPos, state))
                        return true;
                }
                else
                    if (validateVerticalPathAttack(7, 1, full, newPos, state))
                    return true;
            }
            return false;
        }

        protected bool ValidateVerticalPathMove(bool full, Position newPos, GameState state)
        {

            if (this.biDirectional)
            {
                if (validateVerticalPathMove(1, -1, full, newPos, state))
                    return true;
                else if (validateVerticalPathMove(7, 1, full, newPos, state))
                    return true;
            }
            else
            {
                if (this.IsWhite)
                {
                    if (validateVerticalPathMove(1, -1, full, newPos, state))
                        return true;
                }           
                else
                    if (validateVerticalPathMove(7, 1, full, newPos, state))
                        return true;
            }
            return false;
        }

        protected void ProcessVerticalPath(bool full, GameState state)
        {
            if (this.biDirectional)
            {
                processVerticalMove(1, -1, full, state);
                processVerticalAttack(1, -1, full, state);
                processVerticalMove(7, 1, full, state);
                processVerticalAttack(7, 1, full, state);
            }
            else
            {
                if (this.IsWhite)
                {
                    processVerticalMove(1, -1, full, state);
                    processVerticalAttack(1, -1, full, state);
                }
                else
                {
                    processVerticalMove(7, 1, full, state);
                    processVerticalAttack(7, 1, full, state);
                }
            }
        }

        private bool validateVerticalPathMove(int position, int xChange, bool full, Position newPos, GameState state)
        {
            for (int row = this.Position.Row, col = this.Position.Column; checkVerticalPathBounds(position, row, this.MovePath.Paths[position].Row); row += xChange)
                if (newPos.Equals(new Position(row, col)))
                    if (state.GetBoard()[row, col] != null)
                    {
                        if (state.GetBoard()[row, col].IsWhite == this.IsWhite)
                            return false;
                        else
                        {
                            if (this is Pawn)
                                return false;
                            else
                                return checkCapture(newPos, state);
                        }
                            
                    }
                    else
                        return true;

            return false;
        }

        private bool validateVerticalPathAttack(int position, int xChange, bool full, Position newPos, GameState state)
        {
            for (int row = this.Position.Row, col = this.Position.Column; checkVerticalPathBounds(position, row, this.AttackPath.Paths[position].Row); row += xChange)
                if (newPos.Equals(new Position(row, col)))
                    if (state.GetBoard()[row, col] != null)
                    {
                        if (state.GetBoard()[row, col].IsWhite == this.IsWhite)
                            return false;
                        else
                            return checkCapture(newPos, state);
                    }
                    else
                        return true;
            return false;
        }

        private void processVerticalMove(int position, int xChange, bool full, GameState state)
        {
            if (full)
            {
                this.MovePath.Paths[position] = this.Position.OnTheFlyChanges(0, 0);
                Position move = new Position(this.Position.Row, this.Position.Column);
                while (checkVerticalBounds(position, move.Row))
                {
                    move.Row += xChange;
                    if (state.GetBoard()[move.Row, move.Column] == null || state.GetBoard()[move.Row, move.Column] is Ghost)
                        this.MovePath.Paths[position] = new Position(move.Row, move.Column);
                    else if (state.GetBoard()[move.Row, move.Column].IsWhite != this.IsWhite)
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
                    if (state.GetBoard()[(this.Position.Row + xChange), this.Position.Column] == null || state.GetBoard()[(this.Position.Row + xChange), this.Position.Column].IsWhite != this.IsWhite || state.GetBoard()[this.Position.Row + xChange, this.Position.Column] is Ghost)
                        this.MovePath.Paths[position] = new Position(this.Position.Row + xChange, this.Position.Column);
            }
        }

        private void processVerticalAttack(int position, int xChange, bool full, GameState state)
        {
            if (full)
            {
                this.AttackPath.Paths[position] = this.Position.OnTheFlyChanges(0, 0);
                Position move = new Position(this.Position.Row, this.Position.Column);
                while (checkVerticalBounds(position, move.Row))
                {
                    move.Row += xChange;
                    if (state.GetBoard()[move.Row, move.Column] == null || state.GetBoard()[move.Row, move.Column] is Ghost)
                        this.AttackPath.Paths[position] = new Position(move.Row, move.Column);
                    else if (state.GetBoard()[move.Row, move.Column].IsWhite != this.IsWhite)
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
                        if (state.GetBoard()[(this.Position.Row + xChange), this.Position.Column] == null || state.GetBoard()[(this.Position.Row + xChange), this.Position.Column].IsWhite != this.IsWhite || state.GetBoard()[this.Position.Row + xChange, this.Position.Column] is Ghost)
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
        protected bool ValidateHorizontalPathAttack(bool full, Position newPos, GameState state)
        {
            if (validateHorizontalPathAttack(3, -1, full, newPos, state))
                return true;
            else if (validateHorizontalPathAttack(5, 1, full, newPos, state))
                return true;
            return false;
        }

        protected bool ValidateHorizontalPathMove(bool full, Position newPos, GameState state)
        {
            if (validateHorizontalPathMove(3, -1, full, newPos, state))
                return true;
            else if (validateHorizontalPathMove(5, 1, full, newPos, state))
                return true;
            return false;
        }

        protected void ProcessHorizontalPath(bool full, GameState state)
        {
            processHorizontalMove(3, -1, full, state);
            processHorizontalAttack(3, -1, full, state);
            processHorizontalMove(5, 1, full, state);
            processHorizontalAttack(5, 1, full, state);
        }

        private bool validateHorizontalPathMove(int position, int yChange, bool full, Position newPos, GameState state)
        {
            for (int row = this.Position.Row, col = this.Position.Column; checkHorizontalPathBounds(position, col, this.MovePath.Paths[position].Column); col += yChange)
                if (newPos.Equals(new Position(row, col)))
                    if (state.GetBoard()[row, col] != null)
                    {
                        if (state.GetBoard()[row, col].IsWhite == this.IsWhite)
                            return false;
                        else
                            return checkCapture(newPos, state);
                    }
                    else
                        return true;
            return false;
        }

        private bool validateHorizontalPathAttack(int position, int yChange, bool full, Position newPos, GameState state)
        {
            for (int row = this.Position.Row, col = this.Position.Column; checkHorizontalPathBounds(position, col, this.AttackPath.Paths[position].Column); col += yChange)
                if (newPos.Equals(new Position(row, col)))
                    if (state.GetBoard()[row, col] != null)
                    {
                        if (state.GetBoard()[row, col].IsWhite == this.IsWhite)
                            return false;
                        else
                            return checkCapture(newPos, state);
                    }
                    else
                        return true;
            return false;
        }

        private void processHorizontalMove(int position, int yChange, bool full, GameState state)
        {
            if (full)
            {
                this.MovePath.Paths[position] = this.Position.OnTheFlyChanges(0, 0);
                Position move = new Position(this.Position.Row, this.Position.Column);
                while (checkHorizontalBounds(position, move.Column))
                {
                    move.Column += yChange;
                    if (state.GetBoard()[move.Row, move.Column] == null || state.GetBoard()[move.Row, move.Column] is Ghost)
                        this.MovePath.Paths[position] = new Position(move.Row, move.Column);
                    else if (state.GetBoard()[move.Row, move.Column].IsWhite != this.IsWhite)
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
                    if (state.GetBoard()[this.Position.Row, (this.Position.Column + yChange)] == null || state.GetBoard()[this.Position.Row, (this.Position.Column + yChange)].IsWhite != this.IsWhite || state.GetBoard()[this.Position.Row, this.Position.Column + yChange] is Ghost)
                        this.MovePath.Paths[position] = new Position(this.Position.Row, this.Position.Column + yChange);
            }
        }

        private void processHorizontalAttack(int position, int yChange, bool full, GameState state)
        {
            if (full)
            {
                this.AttackPath.Paths[position] = this.Position.OnTheFlyChanges(0, 0);
                Position move = new Position(this.Position.Row, this.Position.Column);
                while (checkHorizontalBounds(position, move.Column))
                {
                    move.Column += yChange;
                    if (state.GetBoard()[move.Row, move.Column] == null || state.GetBoard()[move.Row, move.Column] is Ghost)
                        this.AttackPath.Paths[position] = new Position(move.Row, move.Column);
                    else if (state.GetBoard()[move.Row, move.Column].IsWhite != this.IsWhite)
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
                    if (state.GetBoard()[this.Position.Row, (this.Position.Column + yChange)] == null || state.GetBoard()[this.Position.Row, (this.Position.Column + yChange)].IsWhite != this.IsWhite || state.GetBoard()[this.Position.Row, this.Position.Column + yChange] is Ghost)
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
        protected void ProcessKnightPaths(GameState state)
        {
            processKnightPathAttack(state);
            processKnightPathMove(state);
        }

        protected bool ValidateKnightAttack(Position newPos, GameState state)
        {
            foreach (Position pathPos in this.AttackPath.Paths)
                if (pathPos != null && pathPos.Equals(newPos))
                    if (state.GetBoard()[newPos.Row, newPos.Column] != null)
                    {
                        if (state.GetBoard()[newPos.Row, newPos.Column].IsWhite == this.IsWhite)
                            return false;
                        else
                            return checkCapture(newPos, state);
                    }
                    else
                        return true;
            return false;
        }

        protected bool ValidateKnightMove(Position newPos, GameState state)
        {
            foreach (Position pathPos in this.MovePath.Paths)
                if (pathPos != null && pathPos.Equals(newPos))
                    if (state.GetBoard()[newPos.Row, newPos.Column] != null)
                    {
                        if (state.GetBoard()[newPos.Row, newPos.Column].IsWhite == this.IsWhite)
                            return false;
                        else
                            return checkCapture(newPos, state);
                    }
                    else
                        return true;
            return false;
        }

        private void processKnightPathAttack(GameState state)
        {
            for (int i = 0; i < 8; i++)
            {
                Position newKnightPos = this.Position.OnTheFlyChanges(Knight.PositionChangers[i][0], Knight.PositionChangers[i][1]);
                if (newKnightPos.Row >= 0 && newKnightPos.Row <= 7 && newKnightPos.Column >= 0 && newKnightPos.Column <= 7)
                    if (state.GetBoard()[newKnightPos.Row, newKnightPos.Column] != null)
                    {
                        if (!(state.GetBoard()[newKnightPos.Row, newKnightPos.Column] is Ghost))
                        {
                            if (state.GetBoard()[newKnightPos.Row, newKnightPos.Column].IsWhite != this.IsWhite)
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

        private void processKnightPathMove(GameState state)
        {
            for (int i = 0; i < 8; i++)
            {
                Position newKnightPos = this.Position.OnTheFlyChanges(Knight.PositionChangers[i][0], Knight.PositionChangers[i][1]);
                if (newKnightPos.Row >= 0 && newKnightPos.Row <= 7 && newKnightPos.Column >= 0 && newKnightPos.Column <= 7)
                    if (state.GetBoard()[newKnightPos.Row, newKnightPos.Column] != null)
                    {
                        if (!(state.GetBoard()[newKnightPos.Row, newKnightPos.Column] is Ghost))
                        {
                            if (state.GetBoard()[newKnightPos.Row, newKnightPos.Column].IsWhite != this.IsWhite)
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

        private bool checkCapture(Position destenation, GameState state)
        {
            if (state.GetBoard()[destenation.Row, destenation.Column] != null)
                if (this is King)
                {
                    if (state.GetBoard()[destenation.Row, destenation.Column] is King)
                        return false;
                    else if (((King)this).isUnderCheckCheck)
                    {
                        return true;
                    }
                    else
                    {
                        return capture(destenation, state);
                    }
                }
                else
                {
                    return capture(destenation, state);
                }
            else
                return true;
        }

        private bool capture(Position destenation, GameState state)
        {
            if (state.GetBoard()[destenation.Row, destenation.Column] is Ghost && this is Pawn)
            {
                state.RemovePawn(((Ghost)state.GetBoard()[destenation.Row, destenation.Column]).PawnSoul);
                state.noCaptures = 0;
                state.ResetRepeats();
                if (this.IsWhite)
                    state.blackPieceCount--;
                else
                    state.whitePieceCount--;
                return true;
            }
            else if (!(state.GetBoard()[destenation.Row, destenation.Column] is Ghost))
            {
                state.noCaptures = 0;
                state.ResetRepeats();
                if (this.IsWhite)
                    state.blackPieceCount--;
                else
                    state.whitePieceCount--;
                return true;
            }
            else
                return true;
        }

        public virtual Piece GetCopy()
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
    }
}
