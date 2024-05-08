using System.Collections.Generic;
namespace Assets.src {
    public interface ChessPiece {
        int getValue();
        Colour getColour();
        Tile getTile();
        List<Tile> possibleMoves();
        void move();
    }
}