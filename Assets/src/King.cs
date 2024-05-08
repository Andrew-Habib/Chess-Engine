﻿using System.Collections.Generic;

namespace Assets.src {

    public class King : GenericPiece {

        private bool castlePrivelege;
        private bool kingSideCastle;
        private bool QueenSideCastle;
        private bool inCheck;
        private bool checkmated;
        private bool stalemated;

        public King(Colour colour, Tile tile) : base(colour, tile) {
            this.colour = colour;
            this.tile = tile;

            this.castlePrivelege = true;
            this.kingSideCastle = false;
            this.QueenSideCastle = false;
            this.inCheck = false;
            this.checkmated = false;
            this.stalemated = false;
    }

        public override int getValue() => (int) PieceValue.KING;

        public override List<Tile> possibleMoves() {
            return new List<Tile>();
        }

        public override void move() {
            this.castlePrivelege = false;
        }

    }

}