namespace Assets.src {

    public class Rook : GenericPiece {

        public Rook(Colour colour) : base(colour) {
        }

        public override PieceType getType() => PieceType.ROOK;

        public override int getValue() => (int) PieceType.ROOK;

    }

}