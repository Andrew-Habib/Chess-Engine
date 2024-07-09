namespace Assets.src {

    public class Pawn : GenericPiece {

        private bool moved;
        private bool capturableEnpassent;

        public Pawn(Colour colour) : base(colour) {
            moved = false;
            capturableEnpassent = false;
        }

        public override PieceType getType() => PieceType.PAWN;

        public override int getValue() => (int) PieceType.PAWN;

        public void markAsMoved() {
            moved = true;
        }

        public void setCapturableByEnpassent(bool capturable) {
            capturableEnpassent = capturable;
        }

        public bool hasMoved() {
            return moved;
        }

        public bool isCapturableByEnpassent() {
            return capturableEnpassent;
        }

    }

}