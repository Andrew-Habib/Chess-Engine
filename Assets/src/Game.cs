using UnityEngine;

namespace Assets.src {
    public class Game : MonoBehaviour {

        public GameObject whitePawn0;
        public GameObject whitePawn1;
        public GameObject whitePawn2;
        public GameObject whitePawn3;
        public GameObject whitePawn4;
        public GameObject whitePawn5;
        public GameObject whitePawn6;
        public GameObject whitePawn7;
        public GameObject whiteRook0;
        public GameObject whiteRook7;
        public GameObject whiteKnight1;
        public GameObject whiteKnight6;
        public GameObject whiteBishop2;
        public GameObject whiteBishop5;
        public GameObject whiteQueen;
        public GameObject whiteKing;

        public GameObject blackPawn0;
        public GameObject blackPawn1;
        public GameObject blackPawn2;
        public GameObject blackPawn3;
        public GameObject blackPawn4;
        public GameObject blackPawn5;
        public GameObject blackPawn6;
        public GameObject blackPawn7;
        public GameObject blackRook0;
        public GameObject blackRook7;
        public GameObject blackKnight1;
        public GameObject blackKnight6;
        public GameObject blackBishop2;
        public GameObject blackBishop5;
        public GameObject blackQueen;
        public GameObject blackKing;

        private Player player1;
        private Player player2;
        private Board board;

        // Start is called before the first frame update
        void Start() {

            this.player1 = new Player(Colour.LIGHT);
            this.player2 = new Player(Colour.DARK);
            this.board = new Board(this.player1, this.player2);
            this.board.initChessBoard();
            
            // Debugging - Checks Move Generators on the Unity Environment
            for (int i = 0; i < this.board.generateLegalMoves(0, 3).Count; i++) {
                int[] array = this.board.generateLegalMoves(0, 3)[i];
                Debug.Log($"Array {i + 1}: " + array[0] + " " + array[1]);
            }

        }

        // Update is called once per frame
        void Update() {

        }

    }
}