namespace Assets.src {
    class Board {

        private Tile[,] tiles;
        private Player pWhite;
        private Player pBlack;

        public Board(Player p1, Player p2) {
            this.tiles = new Tile[8, 8];
            this.pWhite = p1;
            this.pBlack = p2;
            this.pWhite.initPlayer(Colour.LIGHT);
            this.pBlack.initPlayer(Colour.DARK);
        }

        public void initChessBoard() {

            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {

                    if (i == 0) {
                        if (j == 0 || j == 7) {
                            tiles[i, j] = new Tile(i, j, new Rook(Colour.DARK));
                        } else if (j == 1 || j == 6) {
                            tiles[i, j] = new Tile(i, j, new Knight(Colour.DARK));
                        } else if (j == 2 || j == 5) {
                            tiles[i, j] = new Tile(i, j, new Bishop(Colour.DARK));
                        } else if (j == 3) {
                            tiles[i, j] = new Tile(i, j, new Queen(Colour.DARK));
                        } else if (j == 4) {
                            tiles[i, j] = new Tile(i, j, new King(Colour.DARK));
                        } else {
                            tiles[i, j] = new Tile(i, j, null);
                        }
                    } else if (i == 1) {
                        tiles[i, j] = new Tile(i, j, new Pawn(Colour.DARK));
                    } else if (i == 6) {
                        tiles[i, j] = new Tile(i, j, new Pawn(Colour.LIGHT));
                    } else if (i == 7) {
                        if (j == 0 || j == 7) {
                            tiles[i, j] = new Tile(i, j, new Pawn(Colour.LIGHT));
                        } else if (j == 1 || j == 6) {
                            tiles[i, j] = new Tile(i, j, new Knight(Colour.LIGHT));
                        } else if (j == 2 || j == 5) {
                            tiles[i, j] = new Tile(i, j, new Bishop(Colour.LIGHT));
                        } else if (j == 3) {
                            tiles[i, j] = new Tile(i, j, new Queen(Colour.LIGHT));
                        } else if (j == 4) {
                            tiles[i, j] = new Tile(i, j, new King(Colour.LIGHT));
                        } else {
                            tiles[i, j] = new Tile(i, j, null);
                        }
                    } else {
                        tiles[i, j] = new Tile(i, j, null);
                    }

                }
            }

        }

        public Tile getTileAt(int r, int c) {
            return this.tiles[r, c];
        }

        public void generatePawnMoves(int row, int col) {
            // White pawn goes forward and black pawn goes backwards
            int dir_pawn = (this.pWhite.isTurn()) ? 1 : -1;

        }

    }
}
