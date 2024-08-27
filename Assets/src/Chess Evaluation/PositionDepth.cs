using System.Collections.Generic;
using UnityEngine;

namespace Assets.src {
    public class PositionDepth {
        public static List<int> depth3Positions() {

            Board board_d0 = new(); // current position
            board_d0.initChessBoard();
            Dictionary<int, int> hashListPos = new();
            bool whiteTurn = true;
            int total = 0;

            List<int[]> moves_d0 = MoveGenerator.allPlayerMoves(board_d0.getChessPieces(), whiteTurn, false);

            foreach (int[] move0 in moves_d0) {
                Board board_d1 = (Board)board_d0.Clone();
                board_d1.move(move0[0], move0[1], move0[2], move0[3]);

                List<int[]> moves_d1 = MoveGenerator.allPlayerMoves(board_d1.getChessPieces(), !whiteTurn, false);

                foreach (int[] move1 in moves_d1) {
                    Board board_d2 = (Board)board_d1.Clone();
                    board_d2.move(move1[0], move1[1], move1[2], move1[3]);

                    List<int[]> moves_d2 = MoveGenerator.allPlayerMoves(board_d2.getChessPieces(), whiteTurn, false);
                    
                    foreach (int[] move2 in moves_d2) {
                        Board board_d3 = (Board)board_d2.Clone();
                        board_d3.move(move2[0], move2[1], move2[2], move2[3]);
                        List<int[]> moves_d3 = MoveGenerator.allPlayerMoves(board_d3.getChessPieces(), whiteTurn, false);
                        total++;
                        
                    }
                }
                Debug.Log(total);
            }

            return new List<int>();

        }
    }
}
