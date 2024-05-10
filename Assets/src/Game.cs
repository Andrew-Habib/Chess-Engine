namespace Assets.src {
    class Game {

        private Player player1;
        private Player player2;
        private Board board;

        public Game() {
            this.player1 = new Player(Colour.LIGHT);
            this.player2 = new Player(Colour.DARK);
            this.board = new Board(this.player1, this.player2);
        }



    }
}
