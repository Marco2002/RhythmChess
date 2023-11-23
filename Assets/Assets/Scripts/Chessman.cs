using UnityEngine;

public class Chessman : MonoBehaviour {

    private int x = -1;
    private int y = -1;


    public int GetX() { return x; }
    public void SetX(int x) { this.x = x; }

    public int GetY() { return y; }
    public void SetY(int y) { this.y = y; }

}
