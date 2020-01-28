using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace MiniChess
{
    public class GameState
    {
        Piece[,] Board = new Piece[8, 8];
        string[] TurnLog = new string[0];
        Piece[,] LastTurn = new Piece[8, 8];
        King blacKing = null;
        King whiteKing = null;
        bool whiteTurn = true;
        bool gameWon = false;
        bool isDraw = false;
        int moveCount = 0;
        int repeatOfBoards = 0;
        public int blackPieceCount = 16;
        public int whitePieceCount = 16;
        public int noCaptures = 0;
        public int noPawnMoves = 0;
        string log = "";
        const string forfiet = "/ff";
        const string callForDraw = "/CallDraw";


        public GameState()

        {
            initialzieBoard();
            //enPassantTest();
            //checkTest();
            //castlingTest();
            //matTest();
            //check50Turns();
        }




        #region Game State Checks
        void MasterChecker()
        {
            checkMat();
            whiteKing.UpdateCheckState(this);
            blacKing.UpdateCheckState(this);
            checkStalemate();
            checkLeftMaterial();
            checkBoardRepeats();
            checkPawnAndCaptures();
        }

        bool kingUnderCheck()
        {
            if (whiteTurn)
                return whiteKing.UnderCheck;
            else
                return blacKing.UnderCheck;
        }

        void checkMat()
        {
            if (whiteKing.CheckForMate(this))
            {
                gameWon = true;
                Console.WriteLine("Congratulation!! \nWhite Player Won!");
            }
            else if (blacKing.CheckForMate(this))
            {
                gameWon = true;
                Console.WriteLine("Congratulation!! \nBlack Player Won!");
            }
        }

        void checkPromotion()
        {
            for (int i = 0; i < 8; i++)
            {
                if (Board[0, i] is Pawn)
                    promotionChoise(Board[0, i].IsWhite, new Position(0, i));
                if (Board[7, i] is Pawn)
                    promotionChoise(Board[7, i].IsWhite, new Position(7, i));
            }
        }

        void checkStalemate()
        {
            bool checkStalemate = true;
            foreach (Position kingPos in blacKing.MovePath.Paths)
                if (blacKing.ValidateOutOfMove(kingPos, this))
                    checkStalemate = false;
            if (checkStalemate)
            {
                isDraw = true;
                Console.WriteLine("The Black player is out of moves, this is a Draw!");
            }
            checkStalemate = true;
            foreach (Position kingPos in whiteKing.MovePath.Paths)
                if (whiteKing.ValidateOutOfMove(kingPos, this))
                    checkStalemate = false;
            if (checkStalemate)
            {
                isDraw = true;
                Console.WriteLine("The Black player is out of moves, this is a Draw!");
            }
        }

        void checkPawnAndCaptures()
        {
            if (noPawnMoves >= 50 && noCaptures >= 50)
            {
                isDraw = true;
                Console.WriteLine("After 50 turns with no captures or pawn movements, it is a Draw!!");
            }
        }

        void checkLeftMaterial()
        {
            if (checkLeftMaterialForPlayer(true) && checkLeftMaterialForPlayer(false))
            {
                isDraw = true;
                Console.WriteLine("Out of enough material to win, it is a Draw!!");
            }
        }

        bool checkLeftMaterialForPlayer(bool white)
        {
            int playerPieceCount = white ? whitePieceCount : blackPieceCount;
            if (playerPieceCount < 2)
            {
                foreach (Piece piece in Board)
                    if (piece is King && piece != null && piece.IsWhite == white)
                        return true;
            }
            else if (playerPieceCount < 3)
            {
                foreach (Piece piece in Board)
                    if (piece is Knight && piece != null && piece.IsWhite == white)
                        return true;
                    else if (piece is Bishop && piece != null && piece.IsWhite == white)
                        return true;
            }
            return false;
        }

        void checkBoardRepeats()
        {
            if (repeatOfBoards >= 3)
            {
                isDraw = true;
                Console.WriteLine("After 3 repeats of the same position the game is now a Draw!");
            }
        }

        public void compareBoards()
        {
            string currentBoard = boardToString();
            foreach (string board in TurnLog)
                if (board == currentBoard)
                    repeatOfBoards++;
            if (repeatOfBoards < 3)
                repeatOfBoards = 0;
        }
        #endregion

        #region Input Validation
        string getInput()
        {
            bool legalString = true;
            string userInput = "";
            do
            {
                Console.WriteLine("Please enter a valid move and press ENTER: ");
                userInput = Console.ReadLine();
                legalString = isLegalString(userInput);
                if (userInput != callForDraw && userInput != forfiet && legalString)
                {
                    userInput = processInput2Nums(userInput);
                    legalString = new Position(int.Parse(userInput[2].ToString()), int.Parse(userInput[3].ToString())).ValidateLegalPosion();
                    legalString = Board[int.Parse(userInput[0].ToString()), int.Parse(userInput[1].ToString())] != null;
                    if (legalString)
                        legalString = checkIfPlayersPiece(Board[int.Parse(userInput[0].ToString()), int.Parse(userInput[1].ToString())]);
                }
                else
                {
                    bool specialMove = iSpecialMove(userInput);
                    if (specialMove)
                    {
                        useSpecialMove(userInput);
                    }
                }
            } while (!legalString);
            return userInput;
        }

        string getInput(string pcInput)
        {
            bool legalString = true;
            string userInput = "";
            do
            {
                Console.WriteLine("Please enter a valid move and press ENTER: ");
                userInput = pcInput;
                legalString = isLegalString(userInput);
                if (userInput != callForDraw && userInput != forfiet && legalString)
                {
                    userInput = processInput2Nums(userInput);
                    legalString = new Position(int.Parse(userInput[2].ToString()), int.Parse(userInput[3].ToString())).ValidateLegalPosion();
                    legalString = Board[int.Parse(userInput[0].ToString()), int.Parse(userInput[1].ToString())] != null;
                    if (legalString)
                        legalString = checkIfPlayersPiece(Board[int.Parse(userInput[0].ToString()), int.Parse(userInput[1].ToString())]);
                }
                else
                {
                    bool specialMove = iSpecialMove(userInput);
                    if (specialMove)
                    {
                        useSpecialMove(userInput);
                    }
                }
            } while (!legalString);
            return userInput;
        }

        string processInput2Nums(string playerMove)
        {
            char[] legalChar = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
            string processedMove = "";
            foreach (char letter in playerMove.ToLower())
                if (isLegalChar(letter, legalChar))
                    processedMove += legalChar.ToList().IndexOf(letter).ToString();
                else
                    processedMove += (int.Parse(letter.ToString()) - 1).ToString();
            return processedMove;
        }

        bool isLegalChar(char inputLetter, char[] charArr)
        {
            if (charArr.Contains(inputLetter))
                return true;
            else
                return false;
        }

        bool isLegalString(string userInput)
        {
            string lowerInput = userInput.ToLower();
            if (userInput == forfiet)
                return true;
            if (userInput == callForDraw)
                return true;
            if (userInput.Length != 4)
                return false;
            char[] legalChar = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h'};
            char[] legalNumbers = new char[] { '1', '2', '3', '4', '5', '6', '7', '8' };
            for (int i = 0; i < 4; i++)
            {
                if (i == 0 || i == 2)
                {
                    if (!legalNumbers.Contains(lowerInput[i]))
                        return false;
                }
                else
                {
                    if (!legalChar.Contains(lowerInput[i]))
                        return false;
                }
            }
            return true;
        }

        bool checkIfPlayersPiece(Piece piece)
        {
            if (piece.IsWhite == whiteTurn)
                return true;
            return false;
        }

        void useSpecialMove(string playerMove)
        {
            if (playerMove == forfiet)
            {
                isDraw = true;
                Console.WriteLine("{0} Player has resigned!", whiteTurn ? "White" : "Black");
            }
            else
            {
                askDraw(whiteTurn);
            }
        }

        bool iSpecialMove(string playerMove)
        {
            if (playerMove == callForDraw || playerMove == forfiet)
                return true;
            else
                return false;
        }
        #endregion

        #region Game Functions
        public void Game()
        {
            do
            {
                drawBoard();
                noPawnMoves++;
                noCaptures++;
                GameTurn();
                drawBoard();
                MasterChecker();
            } while (!gameWon && !isDraw);
            Console.WriteLine("Game Over!");
        }

        void GameTurn()
        {
            bool legalMove = true;
            string playerMove = "";
            bool specialUsed = false;
            do
            {
                Console.WriteLine("This is {0} Player's Turn!", whiteTurn ? "White" : "Black");
                playerMove = getInput();
                bool specialMove = iSpecialMove(playerMove);
                if (specialMove)
                {
                    useSpecialMove(playerMove);
                    specialUsed = true;
                    break;
                }
                else
                {
                    if (!(legalMove = !kingUnderCheck()))
                    {
                        if (makeMove(playerMove))
                        {
                            updatePaths();
                            updateKingState();
                            legalMove = !kingUnderCheck();
                            if (!legalMove)
                            {
                                Board = GetCopy(LastTurn);
                                updatePaths();
                            }
                        }
                    }
                    else
                    {
                        if(legalMove = makeMove(playerMove))
                        {
                            updatePaths();
                            updateKingState();
                            if (!(legalMove = !kingUnderCheck()))
                            {
                                if (!legalMove)
                                {
                                    Board = GetCopy(LastTurn);
                                    updatePaths();
                                }
                            }
                        }              
                    }                       
                }
            } while (!legalMove); //Turn Validation
            if (!specialUsed)
            {
                updatePaths();
                checkPromotion();
            }
            moveCount++;
            log += string.Format("{0} Player: {1} \n", whiteTurn ? "White" : "Black", playerMove);
            ghostBusters();
            compareBoards();
            LastTurn = GetCopy(Board);
            addBoard();
            swtichTurn();
            drawBoard();
        }

        void updateKingState()
        {
            if (whiteTurn)
                whiteKing.UpdateCheckState(this);
            else
                blacKing.UpdateCheckState(this);
        }

        void swtichTurn() { whiteTurn = !whiteTurn; }

        void promotionChoise(bool white, Position piecePos)
        {
            Console.WriteLine("Your pawn has reached the end and can become another piece, would you like a Queen? \nPress y or n for another piece: ");
            char userChoise = Console.ReadLine().ToLower()[0];
            if (userChoise == 'y')
            {
                Queen queen = new Queen(white);
                queen.Position = piecePos;
                Board[piecePos.Row, piecePos.Column] = queen;
                updatePaths();
            }
            else
            {
                Console.WriteLine("Then please enter the key of your desired piece (r-Rook b-Bihsop k-Knight) and press ENTER: ");
                string userInput = Console.ReadLine().ToLower();
                userChoise = (userInput != null) ? userInput[0] : ' ';
                switch (userChoise)
                {
                    case 'r':
                        Rook rook = new Rook(white, 0);
                        rook.Position = piecePos;
                        Board[piecePos.Row, piecePos.Column] = rook;
                        updatePaths();
                        break;
                    case 'b':
                        Bishop bishop = new Bishop(white, 0);
                        bishop.Position = piecePos;
                        Board[piecePos.Row, piecePos.Column] = bishop;
                        updatePaths();
                        break;
                    case 'k':
                        Knight knight = new Knight(white, 0);
                        knight.Position = piecePos;
                        Board[piecePos.Row, piecePos.Column] = knight;
                        updatePaths();
                        break;
                    default:
                        promotionChoise(white, piecePos);
                        break;
                }
            }
        }

        bool makeMove(string playerMove)
        {
            Piece liftedPiece = getPieceByPosition(new Position(int.Parse(playerMove[0].ToString()), int.Parse(playerMove[1].ToString())));
            return liftedPiece.Move(new Position(int.Parse(playerMove[2].ToString()), int.Parse(playerMove[3].ToString())), this);
        }

        void askDraw(bool white)
        {
            Console.WriteLine("{0} Player has asked for a draw \nDo you agree? y\\n?", white ? "White" : "Black");
            char userChoise = Console.ReadLine().ToLower()[0];
            if (userChoise == 'y')
            {
                isDraw = true;
                Console.WriteLine("{0} Player has agreed for a draw. \nThis is a Draw!!", white ? "Black" : "White");
            }
            else
                Console.WriteLine("{0} Player has refused for a draw, Please continue.", white ? "Black" : "White");
        }

        #endregion

        #region Helper Functions
        Piece getPieceByPosition(Position position)
        {
            return Board[position.Row, position.Column];
        }

        void PrintLog()
        {
            Console.WriteLine(log);
        }

        void drawBoard()
        {
            Console.OutputEncoding = Encoding.Unicode;
            Console.Clear();
            Console.WriteLine("     A     B     C     D     E     F     G     H");
            Console.WriteLine();
            for (int i = 0; i < 8; i++)
            {
                string rowData = (i + 1).ToString() + " ";
                for (int j = 0; j < 8; j++)
                {
                    rowData += string.Format("  {0}  ", (Board[i, j] != null && !(Board[i, j] is Ghost)) ? Board[i, j].Piecesign.ToString() : " x");
                }
                rowData += " " + (i + 1);
                Console.WriteLine(rowData);
                Console.WriteLine();
            }
            Console.WriteLine("     A     B     C     D     E     F     G     H");
        }

        void initialzieBoard()
        {
            for (int i = 0; i < 8; i++)
            {
                Board[1, i] = new Pawn(false, i);
                Board[6, i] = new Pawn(true, i);
            }

            Board[0, 0] = new Rook(false, 0);
            Board[0, 1] = new Knight(false, 1);
            Board[0, 2] = new Bishop(false, 2);
            Board[0, 3] = new Queen(false);
            Board[0, 4] = new King(false);
            blacKing = (King)Board[0, 4];
            Board[0, 5] = new Bishop(false, 5);
            Board[0, 6] = new Knight(false, 6);
            Board[0, 7] = new Rook(false, 7);

            Board[7, 0] = new Rook(true, 0);
            Board[7, 1] = new Knight(true, 1);
            Board[7, 2] = new Bishop(true, 2);
            Board[7, 3] = new Queen(true);
            Board[7, 4] = new King(true);
            whiteKing = (King)Board[7, 4];
            Board[7, 5] = new Bishop(true, 5);
            Board[7, 6] = new Knight(true, 6);
            Board[7, 7] = new Rook(true, 7);

            updatePaths();
        }

        void updatePaths()
        {
            foreach (Piece piece in Board)
                if (piece != null)
                    piece.InitializePaths(this);
        }

        void ghostBusters()
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    if (Board[i, j] is Ghost && ((Ghost)Board[i, j]).PawnSoul.IsWhite != whiteTurn)
                        Board[i, j] = null;
        }

        string boardToString()
        {
            string boardString = "";
            foreach (Piece piece in Board)
                if (piece != null && !(piece is Ghost))
                    boardString += piece.ToString();
                else
                    boardString += "x";
            return boardString;
        }

        void addBoard()
        {
            List<string> logCopy = TurnLog.ToList();
            logCopy.Add(boardToString());
            TurnLog = logCopy.ToArray();
        }

        public Piece[,] GetCopy(Piece[,] origin)
        {
            Piece[,] boardCopy = new Piece[8, 8];
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    boardCopy[i, j] = (origin[i, j] != null) ? origin[i, j].GetCopy() : null;
                    if (origin[i, j] != null)
                    {
                        if (origin[i, j].ToString() == "BK")
                            blacKing = (King)origin[i, j];
                        else if (origin[i, j].ToString() == "WK")
                            whiteKing = (King)origin[i, j];
                    }
                }
            return boardCopy;
        }

        public void ResetRepeats()
        {
            repeatOfBoards = 0;
            ResetTurnLog();
        }

        public void RemovePawn(Pawn pawn)
        {
            Board[pawn.Position.Row, pawn.Position.Column] = null;
        }

        public void MovePiece(Position last, Position next, Piece piece)
        {
            Board[next.Row, next.Column] = piece;
            Board[last.Row, last.Column] = null;
            piece.Position = next;
        }

        public void ResetTurnLog()
        {
            TurnLog = new string[0];
        }
        
        public Piece[,] GetBoard() { return this.Board; }
        #endregion

        #region Tests
        void matTest()
        {
            moveCount = 5;
            Piece[,] matTest = new Piece[8, 8];
            matTest[0, 0] = new King(true);
            matTest[0, 0].Position = new Position(0, 0);
            whiteKing = (King)matTest[0, 0];
            matTest[2, 1] = new Queen(false);
            matTest[2, 1].Position = new Position(2, 1);
            matTest[0, 5] = new King(false);
            matTest[0, 5].Position = new Position(0, 5);
            blacKing = (King)matTest[0, 5];
            matTest[0, 4] = new Rook(false, 4);
            matTest[0, 4].Position = new Position(0, 4);
            matTest[7, 0] = new Rook(false, 0);
            matTest[7, 0].Position = new Position(7, 0);
            Board = matTest;
            whiteTurn = false;
            updatePaths();
        }

        void castlingTest()
        {
            Piece[,] castlingTest = new Piece[8, 8];
            castlingTest[0, 4] = new King(false);
            blacKing = (King)castlingTest[0, 4];
            castlingTest[0, 4].HasMoved = false;
            castlingTest[7, 4] = new King(true);
            whiteKing = (King)castlingTest[7, 4];
            castlingTest[0, 0] = new Rook(false, 0);
            castlingTest[0, 0].HasMoved = false;
            castlingTest[0, 7] = new Rook(false, 7);
            castlingTest[0, 7].HasMoved = false;
            castlingTest[7, 7] = new Rook(true, 7);
            castlingTest[7, 0] = new Rook(true, 0);
            whitePieceCount = 2;
            blackPieceCount = 2;
            Board = castlingTest;
            updatePaths();
        }

        void checkTest()
        {
            Piece[,] checkTest = new Piece[8, 8];
            checkTest[0, 4] = new King(false);
            blacKing = (King)checkTest[0, 4];
            checkTest[7, 4] = new King(true);
            whiteKing = (King)checkTest[7, 4];
            checkTest[1, 4] = new Rook(false, 0);
            checkTest[1, 4].Position = new Position(1, 4);
            checkTest[6, 7] = new Rook(true, 7);
            checkTest[6, 7].Position = new Position(6, 7);
            whitePieceCount = 2;
            blackPieceCount = 2;
            Board = checkTest;
            LastTurn = GetCopy(checkTest);
            updatePaths();
            updateKingState();
        }

        void enPassantTest()
        {
            Piece[,] enPassantTest = new Piece[8, 8];
            enPassantTest[0, 4] = new King(false);
            blacKing = (King)enPassantTest[0, 4];
            enPassantTest[7, 4] = new King(true);
            whiteKing = (King)enPassantTest[7, 4];
            enPassantTest[4, 1] = new Pawn(false, 1);
            enPassantTest[4, 1].Position = new Position(4, 1);
            enPassantTest[6, 0] = new Pawn(true, 0);
            whitePieceCount = 2;
            blackPieceCount = 2;
            Board = enPassantTest;
            LastTurn = GetCopy(enPassantTest);
            updatePaths();
            updateKingState();
        }
        
        void check50Turns()
        {
            noCaptures = 49;
            noPawnMoves = 49;
        }       
        
        /*string AutoChess()
        {
            char[] legalChar = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
            char[] legalNumbers = new char[] { '1', '2', '3', '4', '5', '6', '7', '8' };
            string pChoise = "";
            Piece[] validPieces = new Piece[16];
            for (int i = 0; i < 16; i++)
                foreach (Piece piece in GameState.Board)
                    if (piece != null && !(piece is Ghost) && piece.IsWhite == whiteTurn)
                        validPieces[i] = piece.GetCopy();

        }*/
        #endregion
    }
}
