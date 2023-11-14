using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CsvLevelReader : MonoBehaviour {

    private string fen;
    private int maxRank, maxFile;
    private List<(int x, int y)> flagRegion;
    private List<(int x, int y)> disabledFields;
    private Dictionary<string, ((int x, int y) from, (int x, int y) to)> moveMatrix;
    
    public void ReadLevelCsv(string levelName) {
        
        TextAsset file = Resources.Load<TextAsset>("LevelData/"+levelName);
        string fileData = file.text;
        string[] lines = fileData.Split('\n');
        string[] levelData = lines[0].Split(",");
        string[] moveData = lines.Skip(1).ToArray();

        // fen
        fen = levelData[0];
        Debug.Log("Loaded level. Fen: " + fen);

        // maxRank and maxFile
        maxRank = int.Parse(levelData[1]);
        maxFile = int.Parse(levelData[2]);

        // diabledFields
        disabledFields = new List<(int x, int y)>();
        string disabledFieldsString = levelData[3];
        foreach (string field in disabledFieldsString.Split(' ')) {
            if (field != "") disabledFields.Add(FieldStringToVector2(field));
        }

        // flagRegion
        flagRegion = new List<(int x, int y)>();
        string flagRegionString = levelData[4];
        foreach (string field in flagRegionString.Split(' ')) {
            flagRegion.Add(FieldStringToVector2(field));
        }

        // moveMatrix
        moveMatrix = new Dictionary<string, ((int x, int y) from, (int x, int y) to)>();
        foreach (string data in moveData) {
            string position = data.Split(",")[0];
            string move = data.Split(",")[1];
            (int x, int y) from = FieldStringToVector2(move[..2]);
            (int x, int y) to = FieldStringToVector2(move[2..]);
            moveMatrix.Add(position, (from, to));
        }
    }

    public string GetFen() { return fen; }
    public int GetMaxRank() { return maxRank; }
    public int GetMaxFile() { return maxFile; }
    public List<(int x, int y)> GetFlagRegion() { return flagRegion; }
    public List<(int x, int y)> GetDisabledFields() { return disabledFields; }
    public Dictionary<string, ((int x, int y) from, (int x, int y) to)> GetMoveMatrix() { return moveMatrix; }

    private (int x, int y) FieldStringToVector2(string fieldData) {
        // field is inversed as perspective is from black
        int x = maxFile - ((int)fieldData[0] - 96);
        int y = maxRank - int.Parse(fieldData[1].ToString());
        return (x, y);
    }
}
