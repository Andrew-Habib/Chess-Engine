using UnityEngine;

namespace Assets.src {
    public class Game : MonoBehaviour {

        private Player player1;
        private Player player2;
        private Board board;

        // Start is called before the first frame update
        void Start() {
            this.player1 = new Player(Colour.LIGHT);
            this.player2 = new Player(Colour.DARK);
            this.board = new Board(this.player1, this.player2);
            this.board.initChessBoard();
            Debug.Log(this.board.generateLegalMoves(0, 1));
            
            // Access all items using for loop
            for (int i = 0; i < this.board.generateLegalMoves(0, 1).Count; i++) {
                int[] array = this.board.generateLegalMoves(0, 1)[i];
                Debug.Log($"Array {i + 1}: ");

                for (int j = 0; j < array.Length; j++) {
                    Debug.Log(array[j] + " ");
                }
                Debug.Log("");
            }
        }

        // Update is called once per frame
        void Update() {

        }
    }
}