using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Assets.src {
    public class Game : MonoBehaviour {

        private Board board;
        
        public Sprite moveGenerate;
        public Sprite whiteQueen;
        public Sprite blackQueen;

        private GameObject pieceSelected = null;
        private int piece_row = -1;
        private int piece_col = -1;

        // Start is called before the first frame update
        void Start() {

            board = new Board();
            board.initChessBoard();

        }

        void Update() {

            if (Input.GetMouseButtonDown(0)) {

                List<RaycastHit2D> hitList = (Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), 
                                                    Vector2.zero)).ToList();
                
                int dest_col = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x);
                int dest_row = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y);

                if (hitList.Any(hit => hit.collider.CompareTag("MoveCollider"))) {

                    Collider2D[] colliders = new Collider2D[0];
                    Collider2D targetCollider = new();
                    SpriteRenderer spriteRenderer = pieceSelected.GetComponent<SpriteRenderer>();

                    switch(ChessTools.getPieceType(board.getChessPieces(), piece_row, piece_col)) {

                        case PieceType.PAWN:

                            if (dest_row == 0) { // Black Pawn Promotes to Queen

                                spriteRenderer.sprite = blackQueen;

                            } else if (dest_row == 7) { // White Pawn Promotes to Queen

                                spriteRenderer.sprite = whiteQueen;

                            } else if (Math.Abs(piece_col - dest_col) == 1) { // Enpassent

                                if (board.whiteTurn() && !hitList.Any(hit => hit.collider.CompareTag("BlackPiece"))) {
                                    colliders = Physics2D.OverlapBoxAll(new Vector2(dest_col, dest_row - 1), new Vector2(1, 1), 0);
                                    targetCollider = colliders.FirstOrDefault(c => c.CompareTag("BlackPiece"));
                                    if (targetCollider != null) Destroy(targetCollider.gameObject);
                                } else if (!board.whiteTurn() && !hitList.Any(hit => hit.collider.CompareTag("WhitePiece"))) {
                                    colliders = Physics2D.OverlapBoxAll(new Vector2(dest_col, dest_row + 1), new Vector2(1, 1), 0);
                                    targetCollider = colliders.FirstOrDefault(c => c.CompareTag("WhitePiece"));
                                    if (targetCollider != null) Destroy(targetCollider.gameObject);
                                }

                            }
                            break;

                        case PieceType.KING:

                            if (piece_col - dest_col == 2 && 
                                ChessTools.getPieceType(board.getChessPieces(), dest_row, 0) == PieceType.ROOK) { // Queen-side castle

                                colliders = Physics2D.OverlapBoxAll(new Vector2(0, dest_row), new Vector2(1, 1), 0);
                                targetCollider = colliders.FirstOrDefault(c => c.CompareTag("WhitePiece") || c.CompareTag("BlackPiece"));
                                if (targetCollider != null) targetCollider.transform.position = new Vector2(3, piece_row); // Rook to Queen square

                            } else if (piece_col - dest_col == -2 && 
                                ChessTools.getPieceType(board.getChessPieces(), dest_row, 7) == PieceType.ROOK) { // King-side castle

                                colliders = Physics2D.OverlapBoxAll(new Vector2(7, dest_row), new Vector2(1, 1), 0);
                                targetCollider = colliders.FirstOrDefault(c => c.CompareTag("WhitePiece") || c.CompareTag("BlackPiece"));
                                if (targetCollider != null) targetCollider.transform.position = new Vector2(5, piece_row); // Rook to King's bishop square

                            }
                            break;

                    }

                     

                    Destroy(hitList.First().collider.gameObject);
                    board.move(piece_row, piece_col, dest_row, dest_col);
                    pieceSelected.transform.position = new Vector3(dest_col, dest_row, -1f);
                    removeAllMoveSprites();
                    resetPieceSelection();
                    Debug.Log(board.getResult());

                } else if (hitList.Any(hit => hit.collider.CompareTag("WhitePiece")) ||
                    hitList.Any(hit => hit.collider.CompareTag("BlackPiece"))) {

                    removeAllMoveSprites();
                    resetPieceSelection();
                    pieceSelected = hitList.First().collider.gameObject;
                    piece_col = dest_col;
                    piece_row = dest_row;
                    generateMoveSprites();

                } else {

                    removeAllMoveSprites();
                    resetPieceSelection();

                }
            }

        }

        void generateMoveSprites() {

            foreach (var move in board.generateLegalMoves(piece_row, piece_col)) {
                
                GameObject imageObject = new("moveGen");
                SpriteRenderer spriteRenderer = imageObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = moveGenerate;

                imageObject.transform.position = new Vector3((float) move[1], (float) move[0], -0.5f);
                imageObject.transform.localScale = new Vector3(0.15f, 0.15f, 0);
                imageObject.tag = "Move";

                GameObject objectCollider = new("moveCollider");

                objectCollider.transform.position = new Vector3((float)move[1], (float)move[0], -0.5f);
                objectCollider.transform.localScale = new Vector3(1f, 1f, 0f);
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
            pieceSelected = null;
            piece_row = -1;
            piece_col = -1;
        }

    }
}