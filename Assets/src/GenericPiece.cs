using System.Collections.Generic;

namespace Assets.src {

    public abstract class GenericPiece : ChessPiece {

        protected Colour colour;

        protected GenericPiece(Colour colour) {
            this.colour = colour;
        }

        public abstract int getValue();

        public Colour getColour() {
            return this.colour;
        }
        
        public abstract List<Tile> possibleMoves();

    }

}