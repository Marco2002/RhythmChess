using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessman : MonoBehaviour {
    public GameObject controller;

    private int x = -1;
    private int y = -1;

    public Sprite player, pawn, knight, bishop, rook, queen, king;

    public int GetX() { return x; }
    public void SetX(int x) { this.x = x; }

    public int GetY() { return y; }
    public void SetY(int y) { this.y = y; }

    public void Activate() {
        controller = GameObject.FindGameObjectWithTag("GameController");

        switch (this.name) {
            case "pawn": this.GetComponent<SpriteRenderer>().sprite = pawn; break;
            case "knight": this.GetComponent<SpriteRenderer>().sprite = knight; break;
            case "bishop": this.GetComponent<SpriteRenderer>().sprite = bishop; break;
            case "rook": this.GetComponent<SpriteRenderer>().sprite = rook; break;
            case "queen": this.GetComponent<SpriteRenderer>().sprite = queen; break;
            case "king": this.GetComponent<SpriteRenderer>().sprite = king; break;
            case "player": this.GetComponent<SpriteRenderer>().sprite = player; break;
        }
    }
}
