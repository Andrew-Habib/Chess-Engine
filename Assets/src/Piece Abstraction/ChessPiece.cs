using System;

namespace Assets.src {
    public interface ChessPiece : ICloneable {
        PieceType getType();
        int getValue();
        Colour getColour();
        bool Equals(object obj);
        new object Clone();
        int GetHashCode();
    }
}