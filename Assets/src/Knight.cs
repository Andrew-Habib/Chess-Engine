using System.Collections.Generic;

namespace Assets.src {

    public class Knight : GenericPiece {

        public Knight(Colour colour, Tile tile) : base(colour, tile) {
            this.colour = colour;
            this.tile = tile;
        }

        public override int getValue() => (int) PieceValue.KNIGHT;

        public override List<Tile> possibleMoves() {
            return new List<Tile>();
        }

        public override void move() {
        }

    }

}