using System.Collections.Generic;

namespace Assets.src {

    public class Pawn : GenericPiece {

        public Pawn(Colour colour) : base(colour) {
            this.colour = colour;
        }

        public override int getValue() => (int) PieceValue.PAWN;

        public override List<Tile> possibleMoves() {
            return new List<Tile>();
        }

    }

}