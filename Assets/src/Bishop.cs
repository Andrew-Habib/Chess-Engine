﻿using System.Collections.Generic;

namespace Assets.src {

    public class Bishop : GenericPiece {

        public Bishop(Colour colour) : base(colour) {
        }

        public override PieceType getType() => PieceType.BISHOP;

        public override int getValue() => (int) PieceType.BISHOP - 1;

    }

}