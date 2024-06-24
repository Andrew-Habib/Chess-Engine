using System;

namespace Assets.src {
    static class AfterMoveStateManager {

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
