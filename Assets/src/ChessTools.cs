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

        public static bool teamAtDestination(ChessPiece piece, ChessPiece[,] board, int r, int c) {
            return (piece.getColour() == board[r, c].getColour());
        }

        public static bool enemyAtDestination(ChessPiece piece, ChessPiece[,] board, int r, int c) {
            return !(teamAtDestination(piece, board, r, c));
        }

    }
}
