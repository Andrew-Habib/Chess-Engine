namespace Assets.src {

    public class King : GenericPiece {

        private bool castlePrivelege;
        private bool inCheck;
        private bool checkmated;
        private bool stalemated;

        public King(Colour colour) : base(colour) {
            this.castlePrivelege = true;
            this.inCheck = false;
            this.checkmated = false;
            this.stalemated = false;
        }

        public override PieceType getType() => PieceType.KING;

        public override int getValue() => (int) PieceType.KING;

        public void resetKingStates() {
            this.castlePrivelege = true;
            this.inCheck = false;
            this.checkmated = false;
            this.stalemated = false;
        }

        public void revokeCastling() => this.castlePrivelege = false;

        public bool canCastle() => this.castlePrivelege;

        public void unCheckKing() => this.inCheck = false;

        public void checkKing() => this.inCheck = true;

        public bool isInCheck() => this.inCheck;

        public void mateKing() {
            if (this.inCheck) {
                this.checkmated = true;
            } else {
                this.stalemated = true;
            }
        }

        public bool isCheckMated() => this.checkmated;

        public bool isStaleMated() => this.stalemated;

    }

}