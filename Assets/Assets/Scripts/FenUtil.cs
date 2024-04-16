using System.Collections.Generic;
using UnityEngine;

public class FenUtil : MonoBehaviour {
    public static List<(string pieceName, int x, int y)> FenToPosition(string fen, int maxFile) {
        List<(string pieceName, int x, int y)> position = new();
        var files = fen.Split('/');
        for (var y = 0; y < files.Length; y++) {
            var x = maxFile - 1;
            for (var i = 0; i < files[y].Length; i++) {
                if (char.IsNumber(files[y][i])) {
                    x -= int.Parse(files[y][i].ToString());
                } else {
                    switch (files[y][i]) {
                        case 'a': position.Add(("player", x, y)); break;
                        case 'P': position.Add(("pawn", x, y)); break;
                        case 'R': position.Add(("rook", x, y)); break;
                        case 'N': position.Add(("knight", x, y)); break;
                        case 'B': position.Add(("bishop", x, y)); break;
                        case 'Q': position.Add(("queen", x, y)); break;
                        case 'K': position.Add(("king", x, y)); break;
                    }
                    x--;
                }
            }

        }
        return position;
    }

    public static string PositionToFen(ChessPiece[,] position) {
        var fen = "";
        for(int y = 0; y < position.GetLength(1); y++) {
            fen += "/";
            var fieldsWithoutPiece = 0;
            for(var x = position.GetLength(0)-1; x >= 0; x--) {
                if (position[x, y] is null) fieldsWithoutPiece++;
                else {
                    if (fieldsWithoutPiece > 0) fen += fieldsWithoutPiece;
                    switch (position[x, y].name) {
                        case "player": fen += 'a'; break;
                        case "pawn": fen += 'P'; break;
                        case "rook": fen += 'R'; break;
                        case "knight": fen += 'N'; break;
                        case "bishop": fen += 'B'; break;
                        case "queen": fen += 'Q'; break;
                        case "king": fen += 'K'; break;
                    }
                    fieldsWithoutPiece = 0;
                }
            }
            if (fieldsWithoutPiece > 0) fen += fieldsWithoutPiece;
            
        }
        return fen[1..];
    }
}
