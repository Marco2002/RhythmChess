using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

using Field = System.ValueTuple<int, int>;
using Move = System.ValueTuple<System.ValueTuple<int, int>, System.ValueTuple<int, int>>;
public class LevelReader : MonoBehaviour {

    private const byte END_OF_SEQUENCE = 15;
    private const byte END_OF_FEN = 0;

    private static readonly Dictionary<byte, string> FenValueMap = new() {
        { 10, "player" },
        { 11, "pawn"},
        { 12, "knight"},
        { 13, "bishop"},
        { 14, "rook"},
        { 15, "queen"},
    };

    private List<(string pieceName, int x, int y)> startingPosition;
    private int maxRank, maxFile;
    private List<Field> flagRegion;
    private List<Field> disabledFields;
    private List<Move> solution;
    private Dictionary<string, Move> moveMatrix;
    public static readonly Move INVALID_MOVE = ((-1, -1), (-1, -1));

    private static Field ReadField(byte[] data, int start) {
        return (data[start], data[start + 1]);
    }

    private static Move ReadMove(byte[] data, int start) {
        var from = ReadField(data, start);
        var to = ReadField(data, start + 2);
        return (from, to);
    }
    
    private static (List<Field> fields, int newIndex) ReadFields(byte[] data, int start) {
        List<Field> fields = new();
        int i;
        for (i = start; data[i] != END_OF_SEQUENCE; i += 2) {
            fields.Add(ReadField(data, i));
        }
        return (fields, i+1);
    }

    private static (List<Move> moves, int newIndex) ReadMoves(byte[] data, int start) {
        List<Move> moves = new();
        int i;
        for (i = start; data[i] != END_OF_SEQUENCE; i += 4) {
            moves.Add(ReadMove(data, i));
        }

        return (moves, i+1);
    }

    private (List<(string pieceName, int x, int y)> position, int newIndex) ReadFen(byte[] data, int start) {
        List<(string pieceName, int x, int y)> position = new();
        var currentX = maxFile - 1;
        var currentY = 0;
        int i;
        for (i = start; data[i] != END_OF_FEN; i++) {
            if (data[i] <= 8) {
                currentX -= data[i];
            } else {
                position.Add((FenValueMap[data[i]], currentX, currentY));
                currentX--;
            }
            if (currentX < 0) {
                currentX = maxFile - 1;
                currentY++;
            }
        }
        return (position, i+1);
    }

    private static string PositionToFen(ChessPiece[,] position) {
        var fen = new List<byte>();
        for(var y = 0; y < position.GetLength(1); y++) {
            byte fieldsWithoutPiece = 0;
            for(var x = position.GetLength(0)-1; x >= 0; x--) {
                if (position[x, y] is null) fieldsWithoutPiece++;
                else {
                    if (fieldsWithoutPiece > 0) fen.Add(fieldsWithoutPiece);
                    switch (position[x, y].name) {
                        case "player": fen.Add(10); break;
                        case "pawn": fen.Add(11); break;
                        case "knight": fen.Add(12); break;
                        case "bishop": fen.Add(13); break;
                        case "rook": fen.Add(14); break;
                        case "queen": fen.Add(15); break;
                    }
                    fieldsWithoutPiece = 0;
                }
            }
            if (fieldsWithoutPiece > 0) fen.Add(fieldsWithoutPiece);
            
        }
        fen.Add(0);
        return BitConverter.ToString(fen.ToArray());
    }
    public void ReadLevelCsv(string levelName) {
        var path = Path.Combine(Application.streamingAssetsPath, "LevelData/" + levelName + ".rcl");
        byte[] data;
        if (Application.platform == RuntimePlatform.Android) {
            using var www = UnityWebRequest.Get(path);
            www.SendWebRequest();
            while (!www.isDone) { }

            data = www.downloadHandler.data;
        } else {
            data = File.ReadAllBytes(path);
        }
        var formatedData = new byte[data.Length * 2];
        int currentIndex;

        for (var i = 0; i < data.Length; i++) {
            formatedData[2*i] = (byte)(data[i] >> 4); // 4 higher bits of data[i]
            formatedData[2*i+1] = (byte)(data[i] & 0x0F); // 4 lower bits of data[i]
        }

        // maxRank and maxFile
        maxRank = formatedData[0];
        maxFile = formatedData[1];
        
        // fen
        (startingPosition, currentIndex) = ReadFen(formatedData, 2);

        // flagRegion
        (flagRegion, currentIndex) = ReadFields(formatedData, currentIndex);
        
        // disabledFields
        (disabledFields, currentIndex) = ReadFields(formatedData, currentIndex);
        
        // solution
        (solution, currentIndex) = ReadMoves(formatedData, currentIndex);

        // moveMatrix
        moveMatrix = new Dictionary<string, Move>();
        while (currentIndex < formatedData.Length) {
            var positionStart = currentIndex;
            while (formatedData[currentIndex] != END_OF_FEN) {
                currentIndex++;
            }
            currentIndex++;
            if(currentIndex >= formatedData.Length) return;
            var positionEnd = currentIndex;
            var position = new byte[positionEnd-positionStart];
            Array.Copy(formatedData, positionStart, position, 0, positionEnd - positionStart);
            var move = ReadMove(formatedData, currentIndex);
            currentIndex += 4;
            moveMatrix.Add(BitConverter.ToString(position), move);
        }
    }

    public List<(string pieceName, int x, int y)> GetStartingPosition() { return startingPosition; }
    public int GetMaxRank() { return maxRank; }
    public int GetMaxFile() { return maxFile; }
    public List<Field> GetFlagRegion() { return flagRegion; }
    public List<Field> GetDisabledFields() { return disabledFields; }

    public List<Move> GetSolution() { return solution; }

    public Move GetBestMove(ChessPiece[,] position) {
        var fen = PositionToFen(position);
        return moveMatrix.GetValueOrDefault(fen, INVALID_MOVE);
    }
}

public class ByteArrayComparer : IEqualityComparer<byte[]> {
    public bool Equals(byte[] left, byte[] right) {
        if ( left == null || right == null ) {
            return left == right;
        }
        return left.SequenceEqual(right);
    }
    public int GetHashCode(byte[] key) {
        if (key == null)
            throw new ArgumentNullException("key");
        return key.Sum(b => b);
    }
}
