using UnityEngine;
using System.Collections.Generic;
using System;

namespace Assets.src {

    [Serializable]
    public class Board {

        private ChessPiece[,] tiles;
        private bool isWhiteTurn;
        private bool[] gameResult;
        private King whiteKing;
        private King blackKing;
        private int movesInARowNoCapture;
        private Dictionary<int, int> hashPosWhiteTurn;
        private Dictionary<int, int> hashPosBlackTurn;
        private Stack<Stack<Tuple<int, int, ChessPiece>>> oldPiecePositions;

        public Board() {

            tiles = new ChessPiece[8, 8];
            isWhiteTurn = true;
            gameResult = new bool[] { false, false };
            whiteKing = new King(Colour.LIGHT);
            blackKing = new King(Colour.DARK);
            movesInARowNoCapture = 0;
            hashPosWhiteTurn = new Dictionary<int, int>();
            hashPosBlackTurn = new Dictionary<int, int>();
            oldPiecePositions = new();

        }

        public void initChessBoard() {

            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {

                    if (i == 0) {
                        if (j == 0 || j == 7) {
                            tiles[i, j] = new Rook(Colour.LIGHT);
                        } else if (j == 1 || j == 6) {
                            tiles[i, j] = new Knight(Colour.LIGHT);
                        } else if (j == 2 || j == 5) {
                            tiles[i, j] = new Bishop(Colour.LIGHT);
                        } else if (j == 3) {
                            tiles[i, j] = new Queen(Colour.LIGHT);
                        } else if (j == 4) {
                            tiles[i, j] = whiteKing;
                        } else {
                            tiles[i, j] = null;
                        }
                    } else if (i == 1) {
                        tiles[i, j] = new Pawn(Colour.LIGHT);
                    } else if (i == 6) {
                        tiles[i, j] = new Pawn(Colour.DARK);
                    } else if (i == 7) {
                        if (j == 0 || j == 7) {
                            tiles[i, j] = new Rook(Colour.DARK);
                        } else if (j == 1 || j == 6) {
                            tiles[i, j] = new Knight(Colour.DARK);
                        } else if (j == 2 || j == 5) {
                            tiles[i, j] = new Bishop(Colour.DARK);
                        } else if (j == 3) {
                            tiles[i, j] = new Queen(Colour.DARK);
                        } else if (j == 4) {
                            tiles[i, j] = blackKing;
                        } else {
                            tiles[i, j] = null;
                        }
                    } else {
                        tiles[i, j] = null;
                    }

                }
            }

        }

        public Board Clone() {

            Board clonedBoard = new() {
                isWhiteTurn = isWhiteTurn,
                gameResult = gameResult, // Deep copy of array
                whiteKing = (King)whiteKing.Clone(), // Assuming King implements ICloneable
                blackKing = (King)blackKing.Clone(), // Assuming King implements ICloneable
                movesInARowNoCapture = movesInARowNoCapture,
                hashPosWhiteTurn = new Dictionary<int, int>(hashPosWhiteTurn),
                hashPosBlackTurn = new Dictionary<int, int>(hashPosBlackTurn)
            };

            clonedBoard.tiles = new ChessPiece[8, 8];
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    if (tiles[i, j] != null) {
                        clonedBoard.tiles[i, j] = (ChessPiece)tiles[i, j].Clone();
                    }
                }
            }
            return clonedBoard;

        }

        public ChessPiece getPieceAt(int row, int col) => tiles[row, col];

        public ChessPiece[,] getChessPieces() => tiles;

        public bool whiteTurn() => isWhiteTurn;

        public King getWhiteKing() => whiteKing;

        public King getBlackKing() => blackKing;

        public List<int[]> generateLegalMoves(int row, int col) {
            return MoveGenerator.generateMovesAbstract(tiles, row, col, isWhiteTurn, false, gameResult);
        }

        public bool move(int rowPiece, int colPiece, int rowDest, int colDest) {

            if (gameResult.Length == 2 && (gameResult[0] == true || gameResult[1] == true))
                return false;

            List<int[]> legalMoves = generateLegalMoves(rowPiece, colPiece);

            foreach (int[] possible in legalMoves) {
                if (rowDest == possible[0] && colDest == possible[1]) {
                    confirmMoveBoardStateManager(rowPiece, colPiece, rowDest, colDest);                    
                    return true;
                }
            }

            return false;

        }

        private void confirmMoveBoardStateManager(int rowPiece, int colPiece, int rowDest, int colDest) {


            Stack<Tuple<int, int, ChessPiece>> currPiecePos = new();

            switch (ChessTools.getPieceType(tiles, rowPiece, colPiece)) {
                case PieceType.PAWN:
                    AfterMoveStateManager.updatePawnState(tiles, isWhiteTurn, rowPiece, colPiece, rowDest, colDest, currPiecePos);
                    break;
                case PieceType.ROOK:
                    AfterMoveStateManager.updateRookState(tiles, rowPiece, colPiece, rowDest, colDest, currPiecePos);
                    break;
                case PieceType.KING:
                    AfterMoveStateManager.updateKingState(tiles, rowPiece, colPiece, rowDest, colDest, currPiecePos);
                    break;
            }

            movesInARowNoCapture = ChessTools.getPieceType(tiles, rowDest, colDest) == null ?
               movesInARowNoCapture + 1 : 0; // Increment for 50 move rule

            gameResult = AfterMoveStateManager.updateBoardGeneral(tiles, isWhiteTurn, rowPiece, colPiece, rowDest, colDest,
                GetHashCode(), hashPosWhiteTurn, hashPosBlackTurn, movesInARowNoCapture, currPiecePos);

            oldPiecePositions.Push(currPiecePos);

            isWhiteTurn = !isWhiteTurn;

        }

        public bool unmove() {

            if (oldPiecePositions.Count == 0) {
                return false;
            }

            isWhiteTurn = !isWhiteTurn;
            movesInARowNoCapture = 0; // TODO Needs fix
            gameResult = new bool[] { false, false };
            hashPosWhiteTurn = new Dictionary<int, int>();
            hashPosBlackTurn = new Dictionary<int, int>();

            Stack<Tuple<int, int, ChessPiece>> previousPiecePositions = oldPiecePositions.Pop();
            

            foreach (Tuple<int, int, ChessPiece> position in previousPiecePositions) {
                int row = position.Item1;
                int col = position.Item2;
                ChessPiece piece = (ChessPiece)position.Item3;

                Debug.Log($"Restoring piece at ({row}, {col}): {piece?.GetType()}");

                tiles[row, col] = piece != null ? (ChessPiece)piece.Clone() : null;
            }

            DebugBoardState();

            return true;

        }

        // Optional helper for debugging: prints the board state
        private void DebugBoardState() {
            string boardState = "";
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    boardState += tiles[i, j]?.GetType().Name ?? "null";
                    boardState += " ";
                }
                boardState += "\n";
            }
            Debug.Log(boardState);
        }

        public override int GetHashCode() {
            unchecked {
                int hash = 17;

                foreach (ChessPiece piece in tiles) {
                    if (piece != null) {
                        hash = hash * 31 + piece.GetHashCode();
                    } else {
                        hash = hash * 31;
                    }
                }

                return hash;
            }
        }

        public string getResult() {
            if (gameResult.Length != 2 || (gameResult[0] == false && gameResult[1] == false)) {
                return "Active Game!";
            } else if (gameResult[0] == true && gameResult[1] == false) {
                return "White wins!";
            } else if (gameResult[0] == false && gameResult[1] == true) {
                return "Black wins!";
            } else {
                return "White and Black Draw!";
            }
        }

        public void PrintBoardState() {
            for (int row = 0; row < tiles.GetLength(0); row++) {
                string rowState = "";
                for (int col = 0; col < tiles.GetLength(1); col++) {
                    if (tiles[row, col] != null) {
                        rowState += tiles[row, col].getType() + " ";
                    } else {
                        rowState += "Empty ";
                    }
                }
                Debug.Log("Row " + row + ": " + rowState);
            }
        }

    }
}