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

        public static void updatePawnState(ChessPiece[,] tiles, int rowPiece, int rowDest, int colDest) {

            ((Pawn) tiles[rowDest, colDest]).markAsMoved();

            if (Math.Abs(rowDest - rowPiece) == 2) {
                ((Pawn)tiles[rowDest, colDest]).setCapturableByEnpassent(true);
            }

            if (rowDest == 0) tiles[rowDest, colDest] = new Queen(Colour.DARK);
            if (rowDest == 7) tiles[rowDest, colDest] = new Queen(Colour.LIGHT);

        }

        public static void updateRookState() {

        }

        public static void updateKingState() {

        }

    }
}
