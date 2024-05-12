using System.Collections.Generic;

namespace Assets.src {
    class Board {

        private ChessPiece[,] tiles;
        private Player pWhite;
        private Player pBlack;
        private bool whiteTurn;

        public Board(Player p1, Player p2) {
            this.tiles = new ChessPiece[8, 8];
            this.pWhite = p1;
            this.pBlack = p2;
            this.pWhite.initPlayer(Colour.LIGHT);
            this.pBlack.initPlayer(Colour.DARK);
            this.whiteTurn = this.pWhite.isTurn();
        }

        public void initChessBoard() {

            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {

                    if (i == 0) {
                        if (j == 0 || j == 7) {
                            tiles[i, j] = new Rook(Colour.DARK, i, j);
                        } else if (j == 1 || j == 6) {
                            tiles[i, j] = new Knight(Colour.DARK, i, j);
                        } else if (j == 2 || j == 5) {
                            tiles[i, j] = new Bishop(Colour.DARK, i, j);
                        } else if (j == 3) {
                            tiles[i, j] = new Queen(Colour.DARK, i, j);
                        } else if (j == 4) {
                            tiles[i, j] = new King(Colour.DARK, i, j);
                        } else {
                            tiles[i, j] = null;
                        }
                    } else if (i == 1) {
                        tiles[i, j] = new Pawn(Colour.DARK, i, j);
                    } else if (i == 6) {
                        tiles[i, j] = new Pawn(Colour.DARK, i, j);
                    } else if (i == 7) {
                        if (j == 0 || j == 7) {
                            tiles[i, j] = new Pawn(Colour.LIGHT, i, j);
                        } else if (j == 1 || j == 6) {
                            tiles[i, j] = new Knight(Colour.DARK, i, j);
                        } else if (j == 2 || j == 5) {
                            tiles[i, j] = new Bishop(Colour.LIGHT, i, j);
                        } else if (j == 3) {
                            tiles[i, j] = new Queen(Colour.LIGHT, i, j);
                        } else if (j == 4) {
                            tiles[i, j] = new King(Colour.LIGHT, i, j);
                        } else {
                            tiles[i, j] = null;
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

        public static bool inbounds(int r, int c) {
            return (r >= 0 && c <= 7);
        }

        public static bool sameTeam(ChessPiece piece, ChessPiece[,] board, int r, int c) {
            return (piece.getColour().Equals(board[r, c].getColour()));
        }

        public void generateLegalMoves(int row, int col) {

            if (this.getPieceAt(row, col) != null) {

                ChessPiece pieceSelected = this.getPieceAt(row, col);

                if ((this.whiteTurn && pieceSelected.getColour() == Colour.LIGHT) ^
                    (!this.whiteTurn && pieceSelected.getColour() == Colour.DARK)) {
                    switch (pieceSelected.getType()) {
                        case PieceType.PAWN:
                            List<int[]> pawnMoves = MoveGenerator.generatePawnMoves(pieceSelected, this.tiles, this.whiteTurn); 
                            break;
                        case PieceType.KNIGHT:
                            List<int[]> knightMoves = MoveGenerator.generateKnightMoves(pieceSelected, this.tiles, this.whiteTurn); 
                            break;
                        case PieceType.BISHOP:
                            List<int[]> bishopMoves = MoveGenerator.generateBishopMoves(pieceSelected, this.tiles, this.whiteTurn); 
                            break;
                        case PieceType.ROOK:
                            List<int[]> rookMoves = MoveGenerator.generateRookMoves(pieceSelected, this.tiles, this.whiteTurn); 
                            break;
                        case PieceType.QUEEN:
                            List<int[]> queenMoves = MoveGenerator.generateQueenMoves(pieceSelected, this.tiles, this.whiteTurn); 
                            break;
                        case PieceType.KING:
                            List<int[]> kingMoves = MoveGenerator.generateKingMoves(pieceSelected, this.tiles, this.whiteTurn); 
                            break;
                    }
                }

            }

        }

    }
}
