using System;

namespace Assets.src {
    static class ChessTools {

        public static string gridToChessCoord(int row, int column) {
            return ((char)('a' + column)).ToString() + (row + 1).ToString();
        }

        public static (int, int) chessCoordToGrid(string coord) {
            if (coord == null || coord.Length != 2) {
                throw new ArgumentException("Invalid chess coordinate");
            }
            return (coord[0] - 'a', int.Parse(coord[1].ToString()) - 1);
        }

        public static int[,] ConvertTo2DNumericalBoard(Board board) {
            int[,] boardNumArray = new int[8, 8];
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    ChessPiece piece = board.getPieceAt(i, j);
                    boardNumArray[i, j] = piece == null ? 0 : piece switch {
                        Pawn => piece.getColour() == Colour.LIGHT ? 1 : -1,
                        Knight => piece.getColour() == Colour.LIGHT ? 3 : -3,
                        Bishop => piece.getColour() == Colour.LIGHT ? 4 : -4,
                        Rook => piece.getColour() == Colour.LIGHT ? 5 : -5,
                        Queen => piece.getColour() == Colour.LIGHT ? 9 : -9,
                        King => piece.getColour() == Colour.LIGHT ? 1000 : -1000,
                        _ => 0
                    };
                }
            }
            return boardNumArray;
        }

        public static bool inbounds(int r, int c) {
            return (r >= 0 && r <= 7 && c >= 0 && c <= 7);
        }

        public static bool emptyTile(ChessPiece[,] board, int r, int c) {
            return board[r, c] == null;
        }

        public static PieceType? getPieceType(ChessPiece[,] board, int r, int c) {
            if (!inbounds(r, c) || board[r, c] == null) {
                return null;
            } else {
                return board[r, c].getType();
            }
        }

        public static bool isCurrentPlayerPiece(ChessPiece[,] board, int r, int c, bool isWhiteTurn) {
            if (!inbounds(r, c) || board[r, c] == null) {
                return false;
            } else {
                return (isWhiteTurn && board[r, c].getColour() == Colour.LIGHT) ||
                        (!isWhiteTurn && board[r, c].getColour() == Colour.DARK);
            }
        }

        public static bool teamAtDestination(ChessPiece piece, ChessPiece[,] board, int r, int c) {
            return piece != null && !emptyTile(board, r, c) && piece.getColour() == board[r, c].getColour();
        }

        public static bool enemyAtDestination(ChessPiece piece, ChessPiece[,] board, int r, int c) {
            return piece != null && !emptyTile(board, r, c) && piece.getColour() != board[r, c].getColour();
        }

    }
}

// TODO - Implement Try catches for static methods
