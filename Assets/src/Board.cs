using UnityEngine;
using System.Collections.Generic;

namespace Assets.src {
    class Board {

        private readonly ChessPiece[,] tiles;
        private bool isWhiteTurn;
        private bool[] gameResult;
        private readonly King whiteKing;
        private readonly King blackKing;
        private readonly Dictionary<int, int> hashPosWhiteTurn;
        private readonly Dictionary<int, int> hashPosBlackTurn;

        public Board() {

            tiles = new ChessPiece[8, 8];
            isWhiteTurn = true;
            gameResult = new bool[] { false, false };
            whiteKing = new King(Colour.LIGHT);
            blackKing = new King(Colour.DARK);
            hashPosWhiteTurn = new Dictionary<int, int>();
            hashPosBlackTurn = new Dictionary<int, int>();

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

        public ChessPiece getPieceAt(int row, int col) => tiles[row, col];

        public ChessPiece[,] getChessPieces() => tiles;

        public bool whiteTurn() => isWhiteTurn;

        public King getWhiteKing() => whiteKing;

        public King getBlackKing() => blackKing;

        public List<int[]> generateLegalMoves(int row, int col) {
            return MoveGenerator.generateMovesAbstract(tiles, row, col, isWhiteTurn, false);
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

            switch (ChessTools.getPieceType(tiles, rowPiece, colPiece)) {
                case PieceType.PAWN:
                    AfterMoveStateManager.updatePawnState(tiles, isWhiteTurn, rowPiece, colPiece, rowDest, colDest);
                    break;
                case PieceType.ROOK:
                    AfterMoveStateManager.updateRookState(tiles, rowPiece, colPiece);
                    break;
                case PieceType.KING:
                    AfterMoveStateManager.updateKingState(tiles, rowPiece, colPiece, rowDest, colDest);
                    break;
            }

            tiles[rowDest, colDest] = tiles[rowPiece, colPiece];
            tiles[rowPiece, colPiece] = null;
            gameResult = AfterMoveStateManager.updateBoardGeneral(tiles, isWhiteTurn, GetHashCode(), hashPosWhiteTurn, hashPosBlackTurn);
            isWhiteTurn = !isWhiteTurn;

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