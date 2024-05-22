using UnityEngine;

public class PieceDragger : MonoBehaviour {

    private bool dragging;
    private Vector3 offset;
    private Vector3 original_pos;

    void Start() {
        original_pos = transform.position;
    }

    void OnMouseDown() {
        dragging = true;
        offset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
    }

    void Update() {
        original_pos = transform.position;
        if (dragging) {
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - offset;
            newPosition.z = original_pos.z;
            transform.position = newPosition;
        }
    }

    void OnMouseUp() {
        dragging = false;
        transform.position = original_pos;
    }

}
