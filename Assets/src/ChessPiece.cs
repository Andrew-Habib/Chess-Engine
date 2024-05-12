namespace Assets.src {
    public interface ChessPiece {
        PieceType getType();
        int getValue();
        Colour getColour();
        int getRow();
        int getColumn();
        string getCoord();
        void setLocation(int r, int c);
    }
}