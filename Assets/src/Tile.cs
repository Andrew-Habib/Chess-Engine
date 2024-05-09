namespace Assets.src {
    public class Tile {

        private int x;
        private int y;
        private ChessPiece piece;
        private string coord;

        public Tile(int x, int y, ChessPiece piece) {
            this.x = x;
            this.y = y;
            this.piece = piece;
            this.coord = ((char)('a' + this.x)).ToString() + (this.y + 1).ToString();
        }

        public int getX() {
            return this.x;
        }

        public int getY() {
            return this.y;
        }

        public ChessPiece getPiece() {
            return this.piece;
        }

        public string getCoord() {
            return this.coord;
        }

    }
}