using System.Collections.Generic;

namespace Assets.src {

    public class Queen : GenericPiece {

        public Queen(Colour colour, Tile tile) : base(colour, tile) {
            this.colour = colour;
            this.tile = tile;
        }

        public override int getValue() => (int) PieceValue.QUEEN;

        public override List<Tile> possibleMoves() {
            return new List<Tile>();
        }

        public override void move() {
        }

    }

}