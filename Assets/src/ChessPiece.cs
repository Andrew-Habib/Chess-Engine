namespace Assets.src {
    public interface ChessPiece {
        PieceType getType();
        int getValue();
        Colour getColour();
        bool Equals(object obj);
        int GetHashCode();
    }
}