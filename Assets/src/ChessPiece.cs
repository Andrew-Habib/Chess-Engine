namespace Assets.src {
    public interface ChessPiece {
        PieceType getType();
        int getValue();
        Colour getColour();
    }
}