using System.Collections.Generic;

namespace Assets.src {

    public class Pawn : GenericPiece {

        public Pawn(Colour colour, int row, int column) : base(colour, row, column) {
        }

        public override PieceType getType() => PieceType.PAWN;

        public override int getValue() => (int) PieceType.PAWN;

    }

}