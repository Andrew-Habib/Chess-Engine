﻿namespace Assets.src {
    class Player {

        private Colour colour;
        private bool turn;

        public Player(Colour col) {
            this.colour = col;
            this.turn = (this.colour == Colour.LIGHT);
        }

        public void switchTurn() {
            this.turn = !this.turn;
        }

        public bool isTurn() {
            return this.turn;
        }

        public void move(string coord) {

        }

    }
}
