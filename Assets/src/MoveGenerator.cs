using UnityEngine;
using System.Collections.Generic;
// TODO - Pinned piece or in check

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

                int possibleRow = row + possible.Item1;
                int possibleCol = col + possible.Item2;

                if (ChessTools.inbounds(possibleRow, possibleCol)) { // Check if knight hop in bounds of chess board
                    // Check if the possible hop location does not contain a team piece - VALID square condition
                    if (ChessTools.emptyTile(board, possibleRow, possibleCol) || 
                        ChessTools.enemyAtDestination(piece, board, possibleRow, possibleCol)) {
                        moves.Add(new int[] {possibleRow, possibleCol});
                    }
                }

            }

            return moves;

        }

        public static List<int[]> generateBishopMoves(ChessPiece piece, ChessPiece[,] board, bool whiteTurn) {
            
            List<int[]> moves = new List<int[]>();

            int row = piece.getRow();
            int col = piece.getColumn();

            (int, int)[] bishopDirections =
            {
                (-1, -1),
                (-1, 1),
                (1, -1),
                (1, 1)
            };

            foreach (var dir in bishopDirections) {
                
                for (int i = 1; ChessTools.inbounds(row + i * dir.Item1, col + i * dir.Item2); i++) { // Top Right Checks

                    int newRow = row + i * dir.Item1;
                    int newCol = col + i * dir.Item2;

                    if (ChessTools.emptyTile(board, newRow, newCol)) {
                        moves.Add(new int[] { newRow, newCol });
                    } else if (ChessTools.enemyAtDestination(piece, board, newRow, newCol)) {
                        moves.Add(new int[] { newRow, newCol });
                        break;
                    } else {
                        break;
                    }

                }

            }

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