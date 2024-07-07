namespace Assets.src {

    public class Queen : GenericPiece {

        public Queen(Colour colour) : base(colour) {}

        public override PieceType getType() => PieceType.QUEEN;

        public override int getValue() => (int) PieceType.QUEEN;

    }

}