namespace Assets.src {

    public abstract class GenericPiece : ChessPiece {

        protected Colour colour;

        protected GenericPiece(Colour colour) {
            this.colour = colour;
        }

        public abstract PieceType getType();

        public abstract int getValue();

        public Colour getColour() {
            return colour;
        }

    }

}