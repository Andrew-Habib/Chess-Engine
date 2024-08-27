using System.Collections.Generic;

namespace Assets.src {
    public class PositionDepth {
        public List<int> depth3Positions() {
            Board board = new();
            board.initChessBoard();
            bool whiteTurn = true;

            MoveGenerator.allPlayerMoves(board.getChessPieces(), whiteTurn, false); // d0 player moves
            Board board2 = (Board)board.Clone();


            return new List<int>();
        }
    }
}
