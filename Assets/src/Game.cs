using UnityEngine;

namespace Assets.src {
    public class Game : MonoBehaviour {

        private Board board;
        
        public Sprite moveGenerate;

        private GameObject pieceSelected = null;
        private int piece_row = -1;
        private int piece_col = -1;

        // Start is called before the first frame update
        void Start() {

            this.board = new Board();
            this.board.initChessBoard();

            // Debugging - Checks Move Generators on the Unity Environment
            for (int i = 0; i < this.board.generateLegalMoves(0, 1).Count; i++) {
                int[] array = this.board.generateLegalMoves(0, 1)[i];
                Debug.Log($"Array {i + 1}: " + array[0] + " " + array[1]);
            }

        }

        void Update() {

            if (Input.GetMouseButtonDown(0)) {

                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

                if (hit.collider != null && hit.collider.CompareTag("Piece")) { // Piece Selected

                    this.pieceSelected = hit.collider.gameObject;
                    Vector2 objectPosition = pieceSelected.transform.position;
                    this.piece_col = Mathf.RoundToInt(objectPosition.x);
                    this.piece_row = Mathf.RoundToInt(objectPosition.y);
                    Debug.Log($"Hit object: {hit.collider.gameObject.name} at position ({piece_row}, {piece_col})");
                    this.generateMoveSprites(piece_row, piece_col);

                } else if (hit.collider != null && hit.collider.CompareTag("Move")) { // Generated Move Selected

                    Vector2 dest = hit.collider.gameObject.transform.position;
                    int dest_col = Mathf.RoundToInt(dest.x);
                    int dest_row = Mathf.RoundToInt(dest.y);
                    this.board.move(piece_row, piece_col, dest_row, dest_col);
                    this.pieceSelected.transform.position = new Vector3(dest_col, dest_row, -1f);
                    this.removeAllMoveSprites();
                    this.resetPieceSelection();
                    this.board.DebugBoardState();

                } else { // Click-Off Selected Piece
                    this.removeAllMoveSprites();
                    this.resetPieceSelection();
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

                BoxCollider2D collider = imageObject.AddComponent<BoxCollider2D>();
                collider.isTrigger = true;

            }
        }

        void removeAllMoveSprites() {
            GameObject[] moveGenObjects = GameObject.FindGameObjectsWithTag("Move");
            foreach (GameObject moveObject in moveGenObjects) {
                Destroy(moveObject);
            }
        }

        void resetPieceSelection() {
            this.pieceSelected = null;
            this.piece_row = -1;
            this.piece_col = -1;
        }

    }
}