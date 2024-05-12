using System.Collections.Generic;

namespace Assets.src {

    public class Knight : GenericPiece {

        public Knight(Colour colour, int row, int column) : base(colour, row, column) {
            this.colour = colour;
        }

        public override PieceType getType() => PieceType.KNIGHT;

        public override int getValue() => (int) PieceType.KNIGHT;

    }

}