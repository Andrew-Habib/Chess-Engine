using System.Collections.Generic;

namespace Assets.src {

    static class MoveGenerator {

        public static List<int[]> generatePawnMoves(ChessPiece piece, ChessPiece[,] board, bool whiteTurn) {
            List<int[]> moves = new List<int[]>();
            return new List<int[]>();
        }

        public static List<int[]> generateKnightMoves(ChessPiece piece, ChessPiece[,] board, bool whiteTurn) {

            List<int[]> moves = new List<int[]>();

            int row = piece.getRow();
            int col = piece.getColumn();

            (int, int)[] knightHopCombos =
            {
                (1, 2),
                (2, 1),
                (2, -1),
                (1, -2),
                (-1, -2),
                (-2, -1),
                (-2, 1),
                (-1, 2)
            };

            foreach (var possible in knightHopCombos) { // Check all knight hops

                int possibleRow = piece.getRow() + possible.Item1;
                int possibleCol = piece.getColumn() + possible.Item2;

                if (Board.inbounds(possibleRow, possibleCol)) { // Check if knight hop in bounds of chess board
                    // Check if the possible hop location does not contain a team piece - VALID square condition
                    if (!(piece.getColour().Equals(board[row + possibleRow, col + possibleCol].getColour()))) {
                        moves.Add(new int[] {row + possibleRow, col + possibleCol});
                    }
                }

            }

            return moves;

        }

        public static List<int[]> generateBishopMoves(ChessPiece piece, ChessPiece[,] board, bool whiteTurn) {
            List<int[]> moves = new List<int[]>();



            return moves;
        }

        public static List<int[]> generateRookMoves(ChessPiece piece, ChessPiece[,] board, bool whiteTurn) {
            List<int[]> moves = new List<int[]>();
            return moves;
        }

        public static List<int[]> generateQueenMoves(ChessPiece piece, ChessPiece[,] board, bool whiteTurn) {
            List<int[]> moves = new List<int[]>(); // Queen moves are combination of Rook and Bishop Moves
            return moves;
        }

        public static List<int[]> generateKingMoves(ChessPiece piece, ChessPiece[,] board, bool whiteTurn) {
            List<int[]> moves = new List<int[]>();
            return moves;
        }

    }
}