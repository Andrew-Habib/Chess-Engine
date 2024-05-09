using UnityEngine;

public class BoardGeneratorInit : MonoBehaviour {

    public Sprite tileSprite;

    // Start is called before the first frame update
    void Start(){
        generateBoard();
    }

    void generateBoard() {
        bool isWhite = false;

        for (int row = 0; row < 8; row++) {
            for (int col = 0; col < 8; col++) {

                GameObject tile = new GameObject("Tile_" + row + "_" + col);

                SpriteRenderer renderer = tile.AddComponent<SpriteRenderer>();
                renderer.sprite = tileSprite;

                renderer.color = isWhite ? Color.white : Color.black;
                tile.transform.localPosition = new Vector2(-4f + row, -4f + col);

                isWhite = !isWhite;

            }
            isWhite = !isWhite;
        }
    }

}
