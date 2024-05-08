using System.Collections.Generic;
public interface ChessPiece{
    Colour colour();
    PieceValue value();
    List<Tile> possibleMoves(); 
}