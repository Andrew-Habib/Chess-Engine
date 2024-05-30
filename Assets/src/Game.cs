using UnityEngine;
using System.Linq;
using System.Collections.Generic;

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

                List<RaycastHit2D> hitList = (Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), 
                                                    Vector2.zero)).ToList();
                
                int dest_col = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x);
                int dest_row = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y);

                if (hitList.Any(hit => hit.collider.CompareTag("MoveCollider"))) {

                    Destroy(hitList.First().collider.gameObject);
                    this.board.move(piece_row, piece_col, dest_row, dest_col);
                    this.pieceSelected.transform.position = new Vector3(dest_col, dest_row, -1f);
                    this.removeAllMoveSprites();
                    this.resetPieceSelection();

                } else if (hitList.Any(hit => hit.collider.CompareTag("WhitePiece")) ||
                    hitList.Any(hit => hit.collider.CompareTag("BlackPiece"))) {

                    this.removeAllMoveSprites();
                    this.resetPieceSelection();
                    this.pieceSelected = hitList.First().collider.gameObject;
                    this.piece_col = dest_col;
                    this.piece_row = dest_row;
                    this.generateMoveSprites();

                } else {

                    this.removeAllMoveSprites();
                    this.resetPieceSelection();

                }
            }

        }

        void generateMoveSprites() {

            foreach (var move in this.board.generateLegalMoves(piece_row, piece_col)) {
                
                GameObject imageObject = new GameObject("moveGen");
                SpriteRenderer spriteRenderer = imageObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = moveGenerate;

                imageObject.transform.position = new Vector3((float) move[1], (float) move[0], -0.5f);
                imageObject.transform.localScale = new Vector3(0.15f, 0.15f, 0);
                imageObject.tag = "Move";

                GameObject objectCollider = new GameObject("moveCollider");

                objectCollider.transform.position = new Vector3((float)move[1], (float)move[0], -0.5f);
                objectCollider.transform.localScale = new Vector3(1f, 1f, 0);
                objectCollider.tag = "MoveCollider";

                BoxCollider2D collider = objectCollider.AddComponent<BoxCollider2D>();
                collider.isTrigger = true;

            }

        }

        void removeAllMoveSprites() {
            foreach (GameObject moveObject in GameObject.FindGameObjectsWithTag("Move")) {
                Destroy(moveObject);
            }
            foreach (GameObject collider in GameObject.FindGameObjectsWithTag("MoveCollider")) {
                Destroy(collider);
            }
        }

        void resetPieceSelection() {
            this.pieceSelected = null;
            this.piece_row = -1;
            this.piece_col = -1;
        }

    }
}