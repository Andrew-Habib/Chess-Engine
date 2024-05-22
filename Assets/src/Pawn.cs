namespace Assets.src {

    public class Pawn : GenericPiece {

        private bool moved;
        private bool capturableEnpassent;

        public Pawn(Colour colour) : base(colour) {
            this.moved = false;
            this.capturableEnpassent = false;
        }

        public override PieceType getType() => PieceType.PAWN;

        public override int getValue() => (int) PieceType.PAWN;

        public bool hasMoved() {
            return this.moved;
        }

        public bool isCapturableByEnpassent() {
            return this.capturableEnpassent;
        }

    }

}