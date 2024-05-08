using System.Collections.Generic;

namespace Assets.src {

    public class Pawn : GenericPiece {

        private bool moved;

        public Pawn(Colour colour, Tile tile) : base(colour, tile) {
            this.colour = colour;
            this.tile = tile;
            this.moved = true;
        }

        public override int getValue() => (int) PieceValue.PAWN;

        public override List<Tile> possibleMoves() {
            return new List<Tile>();
        }

        public override void move() {
            this.moved = true;
        }

    }

}