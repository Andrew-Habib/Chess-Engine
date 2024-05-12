using System.Collections.Generic;

namespace Assets.src {

    public class Rook : GenericPiece {

        public Rook(Colour colour, int row, int column) : base(colour, row, column) {
        }

        public override PieceType getType() => PieceType.ROOK;

        public override int getValue() => (int) PieceType.ROOK;

    }

}