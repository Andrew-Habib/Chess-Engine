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

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;

            GenericPiece other = (GenericPiece)obj;
            return colour == other.colour;
        }

        public abstract object Clone();

        public override int GetHashCode() {
            return colour.GetHashCode();
        }

    }

}