using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class LevelReader : MonoBehaviour {

    private const byte END_OF_FIELD_SEQUENCE = 15;
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
    private List<(int x, int y)> flagRegion;
    private List<(int x, int y)> disabledFields;
    private Dictionary<byte[], ((int x, int y) from, (int x, int y) to)> moveMatrix;

    private static (int x, int y) ReadField(byte[] data, int start) {
        return (data[start], data[start + 1]);
    }
    private static (List<(int x, int y)> fields, int newIndex) ReadFields(byte[] data, int start) {
        List<(int x, int y)> fields = new();
        int i;
        for (i = start; data[i] != END_OF_FIELD_SEQUENCE; i += 2) {
            fields.Add(ReadField(data, i));
        }
        return (fields, i+1);
    }

    private (List<(string pieceName, int x, int y)> position, int newIndex) ReadFen(byte[] data, int start) {
        List<(string pieceName, int x, int y)> position = new();
        var currentX = maxFile - 1;
        var currentY = 0;
        int i;
        for (i = start; data[i] != END_OF_FEN; i++) {
            if (currentX <= 0) {
                currentX = maxFile - 1;
                currentY++;
            }
            if (data[i] <= 8) {
                currentX -= data[i];
            } else {
                position.Add((FenValueMap[data[i]], currentX, currentY));
                currentX--;
            }
        }
        return (position, i+1);
    }

    private byte[] PositionToFen(ChessPiece[,] position) {
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
        return fen.ToArray();
    }
    public void ReadLevelCsv(string levelName) {
        var data = File.ReadAllBytes("Assets/Assets/Resources/LevelData/Easy/" + levelName + ".rcl");
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

        // moveMatrix
        moveMatrix = new Dictionary<byte[], ((int x, int y) from, (int x, int y) to)>(new ByteArrayComparer());
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
            var from = ReadField(formatedData, currentIndex);
            var to = ReadField(formatedData, currentIndex + 2);
            currentIndex += 4;
            moveMatrix.Add(position, (from, to));
        }
    }

    public List<(string pieceName, int x, int y)> GetStartingPosition() { return startingPosition; }
    public int GetMaxRank() { return maxRank; }
    public int GetMaxFile() { return maxFile; }
    public List<(int x, int y)> GetFlagRegion() { return flagRegion; }
    public List<(int x, int y)> GetDisabledFields() { return disabledFields; }

    public ((int x, int y) from, (int x, int y) to) GetBestMove(ChessPiece[,] position) {
        var fen = PositionToFen(position);
        return moveMatrix.TryGetValue(fen, out var move) ? move : ((0, 0), (0, 0));
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
