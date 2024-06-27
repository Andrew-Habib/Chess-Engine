namespace Assets.src {
    static class ChessTools {

        public static string gridToChessCoord(int row, int column) {
            return ((char)('a' + column)).ToString() + (row + 1).ToString();
        }

        public static bool inbounds(int r, int c) {
            return (r >= 0 && r <= 7 && c >= 0 && c <= 7);
        }

        public static bool emptyTile(ChessPiece[,] board, int r, int c) {
            return board[r, c] == null;
        }

        public static PieceType? getPieceType(ChessPiece[,] board, int r, int c) {
            if (board[r, c] == null) {
                return null;
            } else {
                return board[r, c].getType();
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
