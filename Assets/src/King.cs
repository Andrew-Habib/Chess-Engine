using System.Collections.Generic;

namespace Assets.src {

    public class King : GenericPiece {

        private bool castlePrivelege;
        private bool kingSideCastle;
        private bool QueenSideCastle;
        private bool inCheck;
        private bool checkmated;
        private bool stalemated;

        public King(Colour colour, int row, int column) : base(colour, row, column) {

        }

        public override PieceType getType() => PieceType.KING;

        public override int getValue() => (int) PieceType.KING;

    }

}