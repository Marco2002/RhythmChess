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
        
        var file = Resources.Load<TextAsset>("LevelData/"+levelName);
        var fileData = file.text;
        var lines = fileData.Split('\n');
        var levelData = lines[0].Split(",");
        var moveData = lines.Skip(1).ToArray();

        // fen
        fen = levelData[0];

        // maxRank and maxFile
        maxRank = int.Parse(levelData[1]);
        maxFile = int.Parse(levelData[2]);

        // disabledFields
        disabledFields = new List<(int x, int y)>();
        var disabledFieldsString = levelData[3];
        foreach (var field in disabledFieldsString.Split(' ')) {
            if (field != "") disabledFields.Add(FieldStringToVector2(field));
        }

        // flagRegion
        flagRegion = new List<(int x, int y)>();
        var flagRegionString = levelData[4];
        foreach (var field in flagRegionString.Split(' ')) {
            flagRegion.Add(FieldStringToVector2(field));
        }

        // moveMatrix
        moveMatrix = new Dictionary<string, ((int x, int y) from, (int x, int y) to)>();
        foreach (var data in moveData) {
            var position = data.Split(",")[0];
            var move = data.Split(",")[1];
            var from = FieldStringToVector2(move[..2]);
            var to = FieldStringToVector2(move[2..]);
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
        var x = maxFile - (fieldData[0] - 96);
        var y = maxRank - int.Parse(fieldData[1].ToString());
        return (x, y);
    }
}
