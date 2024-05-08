using System.Collections.Generic;

namespace Assets.src {

    public abstract class GenericPiece : ChessPiece {

        protected Colour colour;
        protected Tile tile;

        protected GenericPiece(Colour colour, Tile tile) {
            this.colour = colour;
            this.tile = tile;
        }

        public abstract int getValue();

        public Colour getColour() {
            return this.colour;
        }

        public Tile getTile() {
            return this.tile;
        }
        
        public abstract List<Tile> possibleMoves();

        public abstract void move();

    }

}