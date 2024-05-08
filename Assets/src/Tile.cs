public class Tile {

    private int x;
    private int y;
    private Colour colour; // Maybe Add Dimensions later
    private ChessPiece piece;
    private string coord;

    public Tile(int x, int y, Colour colour, ChessPiece piece) {
        this.x = x;
        this.y = y;
        this.colour = colour;
        this.piece = piece;
        this.coord = ((char)('a' + this.x)).ToString() + (this.y + 1).ToString();
    }

    public int getX() {
        return this.x;
    }

    public int getY() {
        return this.y;
    }

    public Colour getColour() {
        return this.colour;
    }

    public ChessPiece getPiece() {
        return this.piece;
    }

    public string getCoord() {
        return this.coord;
    }

}