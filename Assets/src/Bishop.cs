using System.Collections.Generic;

namespace Assets.src {

    public class Bishop : GenericPiece {

        public Bishop(Colour colour) : base(colour) {
            this.colour = colour;
        }

        public override int getValue() => (int) PieceValue.BISHOP;

        public override List<Tile> possibleMoves() {
            return new List<Tile>();
        }

    }

}