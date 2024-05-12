using System;
using System.Collections.Generic;

namespace Assets.src {
    class Board {

        private ChessPiece[,] tiles;
        private Player pWhite;
        private Player pBlack;
        private bool isWhiteTurn;

        public Board(Player p1, Player p2) {

            if(!(p1.isTurn() && !p2.isTurn())) {
                throw new ArgumentException("Invalid entry! Player 1 must be LIGHT and Player 2 must be black.");
            }

            this.tiles = new ChessPiece[8, 8];
            this.pWhite = p1;
            this.pBlack = p2;
            this.isWhiteTurn = this.pWhite.isTurn();

        }

        public void initChessBoard() {

            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {

                    if (i == 0) {
                        if (j == 0 || j == 7) {
                            this.tiles[i, j] = new Rook(Colour.LIGHT, i, j);
                        } else if (j == 1 || j == 6) {
                            this.tiles[i, j] = new Knight(Colour.LIGHT, i, j);
                        } else if (j == 2 || j == 5) {
                            this.tiles[i, j] = new Bishop(Colour.LIGHT, i, j);
                        } else if (j == 3) {
                            this.tiles[i, j] = new Queen(Colour.LIGHT, i, j);
                        } else if (j == 4) {
                            this.tiles[i, j] = new King(Colour.LIGHT, i, j);
                        } else {
                            this.tiles[i, j] = null;
                        }
                    } else if (i == 1) {
                        this.tiles[i, j] = new Pawn(Colour.LIGHT, i, j);
                    } else if (i == 6) {
                        this.tiles[i, j] = new Pawn(Colour.DARK, i, j);
                    } else if (i == 7) {
                        if (j == 0 || j == 7) {
                            this.tiles[i, j] = new Rook(Colour.DARK, i, j);
                        } else if (j == 1 || j == 6) {
                            this.tiles[i, j] = new Knight(Colour.DARK, i, j);
                        } else if (j == 2 || j == 5) {
                            this.tiles[i, j] = new Bishop(Colour.DARK, i, j);
                        } else if (j == 3) {
                            this.tiles[i, j] = new Queen(Colour.DARK, i, j);
                        } else if (j == 4) {
                            this.tiles[i, j] = new King(Colour.DARK, i, j);
                        } else {
                            this.tiles[i, j] = null;
                        }
                    } else {
                        tiles[i, j] = null;
                    }

                }
            }

        }

        public ChessPiece getPieceAt(int row, int col) {
            return this.tiles[row, col];
        }

        public List<int[]> generateLegalMoves(int row, int col) {

            if (this.getPieceAt(row, col) != null) {

                ChessPiece pieceSelected = this.getPieceAt(row, col);

                if ((this.isWhiteTurn && pieceSelected.getColour() == Colour.LIGHT) ^
                    (!this.isWhiteTurn && pieceSelected.getColour() == Colour.DARK)) {
                    switch (pieceSelected.getType()) {
                        case PieceType.PAWN:
                            return MoveGenerator.generatePawnMoves(pieceSelected, this.tiles, this.isWhiteTurn); 
                        case PieceType.KNIGHT:
                            return MoveGenerator.generateKnightMoves(pieceSelected, this.tiles, this.isWhiteTurn); 
                        case PieceType.BISHOP:
                            return MoveGenerator.generateBishopMoves(pieceSelected, this.tiles, this.isWhiteTurn); 
                        case PieceType.ROOK:
                            return MoveGenerator.generateRookMoves(pieceSelected, this.tiles, this.isWhiteTurn); 
                        case PieceType.QUEEN:
                            return MoveGenerator.generateQueenMoves(pieceSelected, this.tiles, this.isWhiteTurn); 
                        case PieceType.KING:
                            return MoveGenerator.generateKingMoves(pieceSelected, this.tiles, this.isWhiteTurn); 
                        default:
                            return new List<int[]>();
                    }
                }

            }

            return new List<int[]>();

        }

        public void move(int rowPiece, int colPiece, int rowDest, int colDest) {
            this.isWhiteTurn = !this.isWhiteTurn;
        }

    }
}
