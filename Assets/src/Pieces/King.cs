namespace Assets.src {

    public class King : GenericPiece {

        private bool castlePrivelege;
        private bool inCheck;
        private bool checkmated;
        private bool stalemated;

        public King(Colour colour) : base(colour) {
            castlePrivelege = true;
            inCheck = false;
            checkmated = false;
            stalemated = false;
        }

        public override PieceType getType() => PieceType.KING;

        public override int getValue() => (int) PieceType.KING;

        public void resetKingStates() {
            castlePrivelege = true;
            inCheck = false;
            checkmated = false;
            stalemated = false;
        }

        public void revokeCastling() => castlePrivelege = false;

        public bool canCastle() => castlePrivelege;

        public void unCheckKing() => inCheck = false;

        public void checkKing() => inCheck = true;

        public bool isInCheck() => inCheck;

        public void mateKing() {
            if (inCheck) {
                checkmated = true;
            } else {
                stalemated = true;
            }
        }

        public bool isCheckMated() => checkmated;

        public bool isStaleMated() => stalemated;

    }

}