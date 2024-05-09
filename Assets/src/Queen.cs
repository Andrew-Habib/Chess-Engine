using System.Collections.Generic;

namespace Assets.src {

    public class Queen : GenericPiece {

        public Queen(Colour colour) : base(colour) {
            this.colour = colour;
        }

        public override int getValue() => (int) PieceValue.QUEEN;

        public override List<Tile> possibleMoves() {
            return new List<Tile>();
        }

    }

}