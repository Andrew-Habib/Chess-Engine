namespace Assets.src {

    public class Rook : GenericPiece {

        private bool moved;

        public Rook(Colour colour) : base(colour) {
            moved = false;
        }

        public override object Clone() {
            return new Rook(colour) {
                moved = moved
            };
        }

        public override PieceType getType() => PieceType.ROOK;

        public override int getValue() => (int) PieceType.ROOK;

        public void markAsMoved() {
            moved = true;
        }

        public bool hasMoved() {
            return moved;
        }

    }

}