using System;

namespace Assets.src {
    static class AfterMoveStateManager {

        public static void updateBoardGeneral(ChessPiece[,] tiles, bool isWhiteTurn) {

            for (int row = 0; row < tiles.GetLength(0); row++) { // Enpassent resets for opposition after move is made
                for (int col = 0; col < tiles.GetLength(1); col++) {
                    if (tiles[row, col] != null && tiles[row, col].getType() == PieceType.PAWN) { 
                        if ((isWhiteTurn && tiles[row, col].getColour() == Colour.DARK) ||
                            (!isWhiteTurn && tiles[row, col].getColour() == Colour.LIGHT)) {
                            ((Pawn)tiles[row, col]).setCapturableByEnpassent(false);
                        }
                    }
                }
            }

        }

        public static void updatePawnState(ChessPiece[,] tiles, bool isWhiteTurn, int rowPiece, int colPiece, int rowDest, int colDest) {

            ((Pawn) tiles[rowPiece, colPiece]).markAsMoved();

            if (Math.Abs(rowDest - rowPiece) == 2) {
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

        public static void updateRookState() {

        }

        public static void updateKingState() {

        }

    }
}
