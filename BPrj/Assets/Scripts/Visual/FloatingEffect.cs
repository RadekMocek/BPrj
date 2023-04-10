using System.Collections;
using UnityEngine;

public class FloatingEffect : MonoBehaviour
{
    // == Floating effect =======================
    private Vector2 initialPosition;
    private Vector2 currentPosition;
    private int floatingOffset;
    private int floatingAddition;

    private IEnumerator Floating()
    {
        while (true) {
            if (floatingOffset == 0 || floatingOffset == 10) floatingAddition *= -1;
            floatingOffset += floatingAddition;
            currentPosition.Set(initialPosition.x, initialPosition.y + floatingOffset * 0.02f);
            this.transform.position = currentPosition;
            yield return new WaitForSeconds(0.075f);
        }
    }

    // == MonoBehaviour functions ===============
    private void Start()
    {
        initialPosition = this.transform.position;
        floatingOffset = 0;
        floatingAddition = -1;
        StartCoroutine(Floating());
    }
}
