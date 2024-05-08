using System.Collections.Generic;

namespace Assets.src {

    public class Rook : GenericPiece {

        public Rook(Colour colour, Tile tile) : base(colour, tile) {
            this.colour = colour;
            this.tile = tile;
        }

        public override int getValue() => (int) PieceValue.ROOK;

        public override List<Tile> possibleMoves() {
            return new List<Tile>();
        }

        public override void move() {
        }

    }

}