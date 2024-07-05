using System.Collections.Generic;

namespace Assets.src {

    static class MoveGenerator {

        public static List<int[]> generateMovesAbstract(ChessPiece[,] board, int row, int col, bool whiteTurn, bool genDangerMode) {

            ChessPiece piece = board[row, col];
            List<int[]> moves = new List<int[]>();

            if (piece != null && ChessTools.isCurrentPlayerPiece(board, row, col, whiteTurn)) {
                moves = piece.getType() switch {
                    PieceType.PAWN => generatePawnMoves(board, row, col, whiteTurn),
                    PieceType.KNIGHT => generateKnightMoves(board, row, col),
                    PieceType.BISHOP => generateBishopMoves(board, row, col),
                    PieceType.ROOK => generateRookMoves(board, row, col),
                    PieceType.QUEEN => generateQueenMoves(board, row, col),
                    PieceType.KING => generateKingMoves(board, row, col),
                    _ => new List<int[]>(),
                };
                if (!genDangerMode) {
                    adjustGeneratedMoves(board, row, col, moves, whiteTurn);
                }
            }

            return moves;

        }

        private static void adjustGeneratedMoves(ChessPiece[,] board, int row, int col, List<int[]> moves, bool whiteTurn) {

            List<int[]> dangerZones = new List<int[]>();

            for (int i = moves.Count - 1; i >= 0; i--) {
                int possible_row = moves[i][0];
                int possible_col = moves[i][1];
                ChessPiece takenPiece = board[possible_row, possible_col];
                board[possible_row, possible_col] = board[row, col];
                board[row, col] = null;
                dangerZones.Clear();
                for (int r = 0; r < board.GetLength(0); r++) {
                    for (int c = 0; c < board.GetLength(1); c++) {
                        if (ChessTools.isCurrentPlayerPiece(board, r, c, !whiteTurn)) { // Handle current player pieces
                            dangerZones.AddRange(generateMovesAbstract(board, r, c, !whiteTurn, true));
                        }
                    }
                }
                foreach (int[] zone in dangerZones) {
                    if (ChessTools.getPieceType(board, zone[0], zone[1]) == PieceType.KING) {
                        ((King)board[zone[0], zone[1]]).checkKing();
                        moves.RemoveAt(i);
                    }
                }
                board[row, col] = board[possible_row, possible_col];
                board[possible_row, possible_col] = takenPiece;
            }
        }

        private static List<int[]> generatePawnMoves(ChessPiece[,] board, int row, int col, bool whiteTurn) {

            ChessPiece piece = board[row, col];
            List<int[]> moves = new List<int[]>();

            int dir = whiteTurn ? 1 : -1;

            // Moving Forward - 1 Square (Maybe 2)
            if (ChessTools.emptyTile(board, row + dir, col)) { 
                moves.Add(new int[] { row + dir, col });
                // If the pawn has not moved yet, allow the pawnt to move up a second square
                if (!((Pawn)piece).hasMoved() && ChessTools.emptyTile(board, row + 2 * dir, col))
                    moves.Add(new int[] { row + 2 * dir, col });
            }

            // Capturing Opposing Pieces Diagonally Forward
            if (ChessTools.inbounds(row + dir, col - 1) && ChessTools.enemyAtDestination(piece, board, row + dir, col - 1))
                moves.Add(new int[] { row + dir, col - 1 }); 
            if (ChessTools.inbounds(row + dir, col + 1) && ChessTools.enemyAtDestination(piece, board, row + dir, col + 1))
                moves.Add(new int[] { row + dir, col + 1 });

            // Capturing Opposing Pieces with enpassent left and right
            if (ChessTools.inbounds(row, col - 1) && ChessTools.enemyAtDestination(piece, board, row, col - 1) && 
                ChessTools.getPieceType(board, row, col - 1) == PieceType.PAWN && ((Pawn)board[row, col - 1]).isCapturableByEnpassent()) {
                moves.Add(new int[] { row + dir, col - 1 });
            }
            if (ChessTools.inbounds(row, col + 1) && ChessTools.enemyAtDestination(piece, board, row, col + 1) &&
                ChessTools.getPieceType(board, row, col + 1) == PieceType.PAWN && ((Pawn)board[row, col + 1]).isCapturableByEnpassent()) {
                moves.Add(new int[] { row + dir, col + 1 });
            }

            return moves;

        }

        private static List<int[]> generateKnightMoves(ChessPiece[,] board, int row, int col) {

            ChessPiece piece = board[row, col];
            List<int[]> moves = new List<int[]>();

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

        private static List<int[]> generateSlidingMoves(ChessPiece[,] board, int row, int col, (int, int)[] dirs) {

            ChessPiece piece = board[row, col];
            List<int[]> moves = new List<int[]>();

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

        private static List<int[]> generateBishopMoves(ChessPiece[,] board, int row, int col) {

            (int, int)[] bishopDirections =
            {
                (-1, -1),
                (-1, 1),
                (1, -1),
                (1, 1)
            };

            List<int[]> moves = generateSlidingMoves(board, row, col, bishopDirections);

            return moves;

        }

        private static List<int[]> generateRookMoves(ChessPiece[,] board, int row, int col) {

            (int, int)[] rookDirections =
            {
                (0, -1),
                (0, 1),
                (-1, 0),
                (1, 0)
            };

            List<int[]> moves = generateSlidingMoves(board, row, col, rookDirections);

            return moves;

        }

        private static List<int[]> generateQueenMoves(ChessPiece[,] board, int row, int col) {
            // Queen moves are combination of Rook and Bishop Moves - Union of Two Sets
            List<int[]> moves = new List<int[]>();

            moves.AddRange(generateBishopMoves(board, row, col));
            moves.AddRange(generateRookMoves(board, row, col));

            return moves;

        }

        private static List<int[]> generateKingMoves(ChessPiece[,] board, int row, int col) {

            ChessPiece piece = board[row, col];
            List<int[]> moves = new List<int[]>();

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

                if (ChessTools.inbounds(newRow, newCol) && (ChessTools.emptyTile(board, newRow, newCol) || 
                    ChessTools.enemyAtDestination(piece, board, newRow, newCol))) {
                    moves.Add(new int[] { newRow, newCol });
                } 

            }

            if (((King)piece).canCastle() && !((King)piece).isInCheck()) {

                // Check for ability to queen-side castle
                // King can castle, Rook is present, Rook has not moved, and all squares between the Queen's rook and the king are empty
                if ((ChessTools.getPieceType(board, row, 0) == PieceType.ROOK && 
                    !((Rook) board[row, 0]).hasMoved() && board[row, 1] == null && board[row, 2] == null && board[row, 3] == null)) {
                    moves.Add(new int[] { row, 2 });
                }

                // Check for ability to king-side castle
                // King can castle, Rook is present, Rook has not moved, and all squares between the King's rook and the king are empty
                if ((ChessTools.getPieceType(board, row, 7) == PieceType.ROOK &&
                    !((Rook)board[row, 7]).hasMoved() && board[row, 5] == null && board[row, 6] == null)) {
                    moves.Add(new int[] { row, 6 });
                }

            }

            return moves;

        }

    }
}