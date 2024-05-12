namespace Assets.src {

    public class Queen : GenericPiece {

        public Queen(Colour colour, int row, int column) : base(colour, row, column) {
        }

        public override PieceType getType() => PieceType.QUEEN;

        public override int getValue() => (int) PieceType.QUEEN;

    }

}