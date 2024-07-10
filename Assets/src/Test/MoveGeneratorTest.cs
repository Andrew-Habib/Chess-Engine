//using NUnit.Framework;
//using System.Collections.Generic;

//// Perform a test to check if number of moves generated correlates to Shannon's calculation

//namespace Assets.src {
//    [TestFixture]
//    class MoveGeneratorTest {

//        private Board board;
//        private List<int[]> moves;

//        [SetUp]
//        public void SetUp() {
//            board = new();
//            board.initChessBoard();
//        }

//        [Test]
//        public void positionsOneHalfMove() {
//            moves = MoveGenerator.allPlayerMoves(board.getChessPieces(), true, false);
//            Assert.Equals(2, 3);
//        }

//        [Test]
//        public void positionsTwoHalfMoves() {
//            //for (int r = 0; r < 8; r++) {
//            //    for (int c = 0; c < 8; c++) {
//            //        List<int[]> firstMoves = MoveGenerator.generateMovesAbstract(board.getChessPieces(), r, c, true, false);
//            //        foreach (int[] move in firstMoves) {
//            //            break;  
//            //        }
//            //    }
//            //}
//        }

//    }
//}
