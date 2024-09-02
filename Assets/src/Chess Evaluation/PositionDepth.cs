using System.Collections.Generic;
using System.IO;

namespace Assets.src {
    public class PositionDepth {

        public static List<int[,]> depth3Positions(Board board) {

            Board board_d0 = board; // current position
            List<int[,]> depth3Positions = new();
            bool whiteTurn = board.whiteTurn();

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
                        depth3Positions.Add(ChessTools.ConvertTo2DNumericalBoard(board_d3));  
                    }
                    depth3Positions.Add(new int[,] { { 2 } } ); // Dividers where all pos in the same division have the same parent depth 2
                }
                depth3Positions.Add(new int[,] { { 1 } }); // Dividers where all pos in the same division have the same parent depth 1
            }

            sendToTxt(depth3Positions, whiteTurn);

            return depth3Positions;

        }

        private static void sendToTxt(List<int[,]> depth3Positions, bool whiteTurn) {
            using (StreamWriter writer = new StreamWriter("data.txt")) {
                if (whiteTurn) {
                    writer.Write(10);
                } else {
                    writer.Write(-10);
                }
                writer.WriteLine();

                foreach (var array in depth3Positions) {
                    int rows = array.GetLength(0);
                    int cols = array.GetLength(1);

                    for (int i = 0; i < rows; i++) {
                        for (int j = 0; j < cols; j++) {
                            writer.Write(array[i, j]);
                            if (j < cols - 1) {
                                writer.Write(",");
                            }
                        }
                        writer.WriteLine();
                    }
                    writer.Write("|\n");
                }
            }
        }

    }
}
