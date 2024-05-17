﻿using UnityEngine;
using System.Collections.Generic;
// TODO - Pinned piece or in check

namespace Assets.src {

    static class MoveGenerator {

        public static List<int[]> generatePawnMoves(ChessPiece piece, ChessPiece[,] board, bool whiteTurn) {

            List<int[]> moves = new List<int[]>();

            int row = piece.getRow();
            int col = piece.getColumn();
            int dir = whiteTurn ? 1 : -1;

            // Moving Forward - 1 Square (Maybe 2)
            if (ChessTools.emptyTile(board, row + dir, col)) { 
                moves.Add(new int[] { row + dir, col });
                // If the pawn has not moved yet, allow the pawnt to move up a second square
                if (!((Pawn)piece).hasMoved() && ChessTools.emptyTile(board, row + 2 * dir, col))
                    moves.Add(new int[] { row + 2 * dir, col });
            }

            // Capturing Opposing Pieces Diagonally Forward
            if (ChessTools.inbounds(row + dir, col - 1) &&
                ChessTools.enemyAtDestination(piece, board, row + dir, col - 1))
                moves.Add(new int[] { row + dir, col - 1 }); 
            if (ChessTools.inbounds(row + dir, col + 1) &&
                ChessTools.enemyAtDestination(piece, board, row + dir, col + 1))
                moves.Add(new int[] { row + dir, col + 1 });

            // Capturing Opposing Pieces with enpassent left and right
            if (ChessTools.inbounds(row, col - 1) &&
                ChessTools.enemyAtDestination(piece, board, row, col - 1) && 
                ((Pawn)board[row, col - 1]).isCapturableByEnpassent()) {
                moves.Add(new int[] { row + dir, col - 1 });
            }
            if (ChessTools.inbounds(row, col + 1) &&
                ChessTools.enemyAtDestination(piece, board, row, col + 1) &&
                ((Pawn)board[row, col + 1]).isCapturableByEnpassent()) {
                moves.Add(new int[] { row + dir, col + 1 });
            }

            return moves;

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
                        moves.Add(new int[] { possibleRow, possibleCol });
                    }
                }

            }

            return moves;

        }

        public static List<int[]> generateSlidingMoves(ChessPiece piece, ChessPiece[,] board, bool whiteTurn, (int, int)[] dirs) {
            
            List<int[]> moves = new List<int[]>();

            int row = piece.getRow();
            int col = piece.getColumn();

            (int, int)[] directions = dirs;

            foreach (var dir in directions) {

                for (int i = 1; ChessTools.inbounds(row + i * dir.Item1, col + i * dir.Item2); i++) {

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

        public static List<int[]> generateBishopMoves(ChessPiece piece, ChessPiece[,] board, bool whiteTurn) {

            (int, int)[] bishopDirections =
            {
                (-1, -1),
                (-1, 1),
                (1, -1),
                (1, 1)
            };

            List<int[]> moves = generateSlidingMoves(piece, board, whiteTurn, bishopDirections);

            return moves;

        }

        public static List<int[]> generateRookMoves(ChessPiece piece, ChessPiece[,] board, bool whiteTurn) {

            (int, int)[] rookDirections =
            {
                (0, -1),
                (0, 1),
                (-1, 0),
                (1, 0)
            };

            List<int[]> moves = generateSlidingMoves(piece, board, whiteTurn, rookDirections);

            return moves;

        }

        public static List<int[]> generateQueenMoves(ChessPiece piece, ChessPiece[,] board, bool whiteTurn) {
            // Queen moves are combination of Rook and Bishop Moves - Union of Two Sets
            List<int[]> moves = new List<int[]>();

            moves.AddRange(generateBishopMoves(piece, board, whiteTurn));
            moves.AddRange(generateRookMoves(piece, board, whiteTurn));

            return moves;

        }

        public static List<int[]> generateKingMoves(ChessPiece piece, ChessPiece[,] board, bool whiteTurn) {

            List<int[]> moves = new List<int[]>();

            int row = piece.getRow();
            int col = piece.getColumn();

            (int, int)[] kingDirections =
            {
                (-1, 0), // Left
                (-1, -1), // Bottom-left
                (0, -1), // Down
                (1, -1), // Bottom-right
                (1, 0), // Right
                (1, 1), // Top-right
                (0, 1), // Up
                (-1, 1) // Top-left
            };

            foreach (var dir in kingDirections) {

                int newRow = row + dir.Item1;
                int newCol = col + dir.Item2;

                if (ChessTools.inbounds(newRow, newCol) &&
                    (ChessTools.emptyTile(board, newRow, newCol) || 
                    ChessTools.enemyAtDestination(piece, board, newRow, newCol))) {
                    moves.Add(new int[] { newRow, newCol });
                } 

            }

            

            return moves;

        }

    }
}