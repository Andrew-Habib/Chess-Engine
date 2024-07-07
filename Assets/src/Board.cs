using UnityEngine;
using System.Collections.Generic;

namespace Assets.src {
    class Board {

        private ChessPiece[,] tiles;
        private bool isWhiteTurn;
        private King whiteKing;
        private King blackKing;

        public Board() {

            this.tiles = new ChessPiece[8, 8];
            this.isWhiteTurn = true;
            this.whiteKing = new King(Colour.LIGHT);
            this.blackKing = new King(Colour.DARK);

        }

        public void initChessBoard() {

            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {

                    if (i == 0) {
                        if (j == 0 || j == 7) {
                            this.tiles[i, j] = new Rook(Colour.LIGHT);
                        } else if (j == 1 || j == 6) {
                            this.tiles[i, j] = new Knight(Colour.LIGHT);
                        } else if (j == 2 || j == 5) {
                            this.tiles[i, j] = new Bishop(Colour.LIGHT);
                        } else if (j == 3) {
                            this.tiles[i, j] = new Queen(Colour.LIGHT);
                        } else if (j == 4) {
                            this.tiles[i, j] = this.whiteKing;
                        } else {
                            this.tiles[i, j] = null;
                        }
                    } else if (i == 1) {
                        this.tiles[i, j] = new Pawn(Colour.LIGHT);
                    } else if (i == 6) {
                        this.tiles[i, j] = new Pawn(Colour.DARK);
                    } else if (i == 7) {
                        if (j == 0 || j == 7) {
                            this.tiles[i, j] = new Rook(Colour.DARK);
                        } else if (j == 1 || j == 6) {
                            this.tiles[i, j] = new Knight(Colour.DARK);
                        } else if (j == 2 || j == 5) {
                            this.tiles[i, j] = new Bishop(Colour.DARK);
                        } else if (j == 3) {
                            this.tiles[i, j] = new Queen(Colour.DARK);
                        } else if (j == 4) {
                            this.tiles[i, j] = this.blackKing;
                        } else {
                            this.tiles[i, j] = null;
                        }
                    } else {
                        tiles[i, j] = null;
                    }

                }
            }

        }

        public ChessPiece getPieceAt(int row, int col) => this.tiles[row, col];

        public ChessPiece[,] getChessPieces() => this.tiles;

        public bool whiteTurn() => this.isWhiteTurn;

        public King getWhiteKing() => this.whiteKing;

        public King getBlackKing() => this.blackKing;

        public List<int[]> generateLegalMoves(int row, int col) {

            if (this.getPieceAt(row, col) != null) {
                return MoveGenerator.generateMovesAbstract(tiles, row, col, isWhiteTurn, false);
            }

            return new List<int[]>();

        }

        public bool move(int rowPiece, int colPiece, int rowDest, int colDest) {

            List<int[]> legalMoves = generateLegalMoves(rowPiece, colPiece);

            foreach (int[] possible in legalMoves) {
                if (rowDest == possible[0] && colDest == possible[1]) {
                    this.confirmMoveBoardStateManager(rowPiece, colPiece, rowDest, colDest);                    
                    return true;
                }
            }

            return false;

        }

        private void confirmMoveBoardStateManager(int rowPiece, int colPiece, int rowDest, int colDest) {

            switch (this.tiles[rowPiece, colPiece].getType()) {
                case PieceType.PAWN:
                    AfterMoveStateManager.updatePawnState(this.tiles, this.isWhiteTurn, rowPiece, colPiece, rowDest, colDest);
                    break;
                case PieceType.ROOK:
                    AfterMoveStateManager.updateRookState(this.tiles, rowPiece, colPiece);
                    break;
                case PieceType.KING:
                    AfterMoveStateManager.updateKingState(this.tiles, rowPiece, colPiece, rowDest, colDest);
                    break;
            }

            this.tiles[rowDest, colDest] = this.tiles[rowPiece, colPiece];
            this.tiles[rowPiece, colPiece] = null;
            AfterMoveStateManager.updateBoardGeneral(this.tiles, this.isWhiteTurn);
            this.isWhiteTurn = !this.isWhiteTurn;

        }

        public void PrintBoardState() {
            for (int row = 0; row < this.tiles.GetLength(0); row++) {
                string rowState = "";
                for (int col = 0; col < this.tiles.GetLength(1); col++) {
                    if (this.tiles[row, col] != null) {
                        rowState += this.tiles[row, col].getType() + " ";
                    } else {
                        rowState += "Empty ";
                    }
                }
                Debug.Log("Row " + row + ": " + rowState);
            }
        }

    }
}