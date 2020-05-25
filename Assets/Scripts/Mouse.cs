using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    private void Update() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        LayerMask layer = LayerMask.GetMask("Clickable");
        //int layer = (1 << 8);

        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 100, layer);

        if (hit.transform == null) return;

        if (hit.transform.CompareTag("Block")) {

            Block block = hit.transform.gameObject.GetComponent<Block>();

            if (Input.GetMouseButton(0)) {
                block.MouseLeft();
            }else if (Input.GetMouseButtonUp(0)) {
                block.MouseLeftUp();
            }else if (Input.GetMouseButtonDown(1)) {
                block.MouseRightDown();
            }
        }
    }
}
