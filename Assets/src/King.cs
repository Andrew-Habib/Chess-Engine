namespace Assets.src {

    public class King : GenericPiece {

        private bool castlePrivelege;
        private bool inCheck;
        private bool checkmated;
        private bool stalemated;

        public King(Colour colour, int row, int column) : base(colour, row, column) {
            this.castlePrivelege = true;
            this.inCheck = false;
            this.checkmated = false;
            this.stalemated = false;
        }

        public override PieceType getType() => PieceType.KING;

        public override int getValue() => (int) PieceType.KING;

    }

}