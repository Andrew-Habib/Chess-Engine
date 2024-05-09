using System.Collections.Generic;

namespace Assets.src {

    public class Knight : GenericPiece {

        public Knight(Colour colour) : base(colour) {
            this.colour = colour;
        }

        public override int getValue() => (int) PieceValue.KNIGHT;

        public override List<Tile> possibleMoves() {
            return new List<Tile>();
        }

    }

}