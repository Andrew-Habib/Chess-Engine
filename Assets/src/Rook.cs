using System.Collections.Generic;

namespace Assets.src {

    public class Rook : GenericPiece {

        public Rook(Colour colour) : base(colour) {
            this.colour = colour;
        }

        public override int getValue() => (int) PieceValue.ROOK;

        public override List<Tile> possibleMoves() {
            return new List<Tile>();
        }

    }

}