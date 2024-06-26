﻿namespace Assets.src {

    public class Knight : GenericPiece {

        public Knight(Colour colour) : base(colour) {
            this.colour = colour;
        }

        public override PieceType getType() => PieceType.KNIGHT;

        public override int getValue() => (int) PieceType.KNIGHT;

    }

}