namespace Assets.src {

    public abstract class GenericPiece : ChessPiece {

        protected Colour colour;
        protected int row;
        protected int column;
        protected string coord;

        protected GenericPiece(Colour colour, int r, int c) {
            this.colour = colour;
            this.row = r;
            this.column = c;
            this.coord = ((char)('a' + this.column)).ToString() + (this.row + 1).ToString();
        }

        public abstract PieceType getType();

        public abstract int getValue();

        public Colour getColour() {
            return this.colour;
        }

        public int getRow() {
            return this.row;
        }

        public int getColumn() {
            return this.column;
        }

        public string getCoord() {
            return this.coord;
        }

        public void setLocation(int r, int c) {
            this.row = r;
            this.column = c;
            this.coord = ((char)('a' + this.column)).ToString() + (this.row + 1).ToString();
        }

    }

}