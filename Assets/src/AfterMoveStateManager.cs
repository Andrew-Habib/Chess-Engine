﻿using System;
using System.Collections.Generic;
using UnityEngine;

// King checked - iterate through all opposing pieces moves then check if the king is at any of those locations
// King danger zone - iterate through all opposing pieces moves then 
// updateBoardGeneral handles enpassent resets and checks

namespace Assets.src {
    static class AfterMoveStateManager {

        public static void updateBoardGeneral(ChessPiece[,] tiles, bool isWhiteTurn) {

            List<int[]> currPlayerMoves = MoveGenerator.allPlayerMoves(tiles, isWhiteTurn, false);
            List<int[]> opposingMoves = MoveGenerator.allPlayerMoves(tiles, !isWhiteTurn, false);

            foreach (int[] zone in currPlayerMoves) { // Check for checks and checkmates
                if (ChessTools.getPieceType(tiles, zone[0], zone[1]) == PieceType.KING) {
                    ((King)tiles[zone[0], zone[1]]).checkKing();
                    Debug.Log("Check!");
                    if (opposingMoves.Count == 0) {
                        ((King)tiles[zone[0], zone[1]]).mateKing();
                        if (isWhiteTurn) {
                            Debug.Log("White wins the Game by Checkmate!");
                        } else {
                            Debug.Log("Black wins the Game by Checkmate!");
                        }   
                    }
                    return;
                }
            }

            for (int row = 0; row < tiles.GetLength(0); row++) {
                for (int col = 0; col < tiles.GetLength(1); col++) {

                    if (ChessTools.isCurrentPlayerPiece(tiles, row, col, isWhiteTurn)){
                        if (ChessTools.getPieceType(tiles, row, col) == PieceType.KING) { // Reset King Checks
                            ((King)tiles[row, col]).unCheckKing();
                        } 
                    }

                    if (ChessTools.isCurrentPlayerPiece(tiles, row, col, !isWhiteTurn)) {

                        if (ChessTools.getPieceType(tiles, row, col) == PieceType.KING) { 
                            if (opposingMoves.Count == 0) { // Can be made more efficient with King ref
                                ((King)tiles[row, col]).mateKing();
                                Debug.Log("The Game is a Draw to Stalemate!");
                                return;
                            }
                        } else if (ChessTools.getPieceType(tiles, row, col) == PieceType.PAWN) {
                            ((Pawn)tiles[row, col]).setCapturableByEnpassent(false);
                        }

                    }

                } 
            }

        }

        public static void updatePawnState(ChessPiece[,] tiles, bool isWhiteTurn, int rowPiece, int colPiece, int rowDest, int colDest) {

            ((Pawn) tiles[rowPiece, colPiece]).markAsMoved();

            if (Math.Abs(rowDest - rowPiece) == 2) { // Check for pawn moving 2 spaces up in one turn
                ((Pawn)tiles[rowPiece, colPiece]).setCapturableByEnpassent(true);
            }

            if (Math.Abs(colPiece - colDest) == 1 && tiles[rowPiece, colPiece].getType() == PieceType.PAWN 
                && tiles[rowDest, colDest] == null) { // Enpassent manouver deleting enemy piece (Only move that requires extra deletion)
                if (isWhiteTurn)
                    tiles[rowDest - 1, colDest] = null;
                else
                    tiles[rowDest + 1, colDest] = null;
            }

            if (rowDest == 0) tiles[rowPiece, colPiece] = new Queen(Colour.DARK);
            if (rowDest == 7) tiles[rowPiece, colPiece] = new Queen(Colour.LIGHT);

        }

        public static void updateRookState(ChessPiece[,] tiles, int rowPiece, int colPiece) {
            ((Rook)tiles[rowPiece, colPiece]).markAsMoved();
        }

        public static void updateKingState(ChessPiece[,] tiles, int rowPiece, int colPiece, int rowDest, int colDest) {

            ((King)tiles[rowPiece, colPiece]).revokeCastling();

            if (colPiece - colDest == 2 && ChessTools.getPieceType(tiles, rowDest, 0) == PieceType.ROOK) { // Check for Queen-side castling
                tiles[rowDest, 3] = tiles[rowPiece, 0];
                tiles[rowPiece, 0] = null;
            } else if (colPiece - colDest == -2 && ChessTools.getPieceType(tiles, rowDest, 7) == PieceType.ROOK) { // Check for King-side castling
                tiles[rowDest, 5] = tiles[rowPiece, 7];
                tiles[rowPiece, 7] = null;
            }

        }

    }
}
