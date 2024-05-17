using UnityEngine;

namespace Assets.src {
    public class Game : MonoBehaviour {

        private Player player1;
        private Player player2;
        private Board board;
        
        public Sprite moveGenerate;

        // Start is called before the first frame update
        void Start() {

            this.player1 = new Player(Colour.LIGHT);
            this.player2 = new Player(Colour.DARK);

            this.board = new Board(this.player1, this.player2);
            this.board.initChessBoard();

            // Debugging - Checks Move Generators on the Unity Environment
            for (int i = 0; i < this.board.generateLegalMoves(0, 1).Count; i++) {
                int[] array = this.board.generateLegalMoves(0, 1)[i];
                Debug.Log($"Array {i + 1}: " + array[0] + " " + array[1]);
            }

        }

        // Update is called once per frame
        void Update() {
            if (Input.GetMouseButtonDown(0)) {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

                if (hit.collider != null && hit.collider.CompareTag("Piece")) {
                    Vector2 objectPosition = hit.collider.gameObject.transform.position;
                    int col = Mathf.RoundToInt(objectPosition.x);
                    int row = Mathf.RoundToInt(objectPosition.y);
                    Debug.Log($"Hit object: {hit.collider.gameObject.name} at position ({row}, {col})");
                    generateMoveSprites(row, col);
                }

            }
        }

        void generateMoveSprites(int row, int col) {

            GameObject[] objectsToDelete = GameObject.FindGameObjectsWithTag("Move");
            foreach (GameObject obj in objectsToDelete) {
                Destroy(obj);
            }

            foreach (var move in this.board.generateLegalMoves(row, col)) {
                
                GameObject imageObject = new GameObject("moveGen");
                SpriteRenderer spriteRenderer = imageObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = moveGenerate;

                imageObject.transform.position = new Vector3((float) move[1], (float) move[0], -0.5f);

                imageObject.transform.localScale = new Vector3(0.15f, 0.15f, 0);

                imageObject.tag = "Move";

            }
        }

    }
}