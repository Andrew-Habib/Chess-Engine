using System;

namespace Assets.src {

    public class King : GenericPiece, ICloneable {

        private bool castlePrivilege;
        private bool inCheck;
        private bool checkmated;
        private bool stalemated;

        public King(Colour colour) : base(colour) {
            castlePrivilege = true;
            inCheck = false;
            checkmated = false;
            stalemated = false;
        }

        public override object Clone() {
            return new King(colour) {
                castlePrivilege = castlePrivilege,
                inCheck = inCheck,
                checkmated = checkmated,
                stalemated = stalemated
            };
        }

        public override PieceType getType() => PieceType.KING;

        public override int getValue() => (int) PieceType.KING;

        public void resetKingStates() {
            castlePrivilege = true;
            inCheck = false;
            checkmated = false;
            stalemated = false;
        }

        public void revokeCastling() => castlePrivilege = false;

        public bool canCastle() => castlePrivilege;

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