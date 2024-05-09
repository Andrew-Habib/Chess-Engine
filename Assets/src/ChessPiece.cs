using System.Collections.Generic;
namespace Assets.src {
    public interface ChessPiece {
        int getValue();
        Colour getColour();
        List<Tile> possibleMoves();
    }
}