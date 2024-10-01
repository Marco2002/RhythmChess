using UnityEngine;

public abstract class ColorScheme {
    public static Color primary = new Color32(163, 235, 254, 255);
    public static Color primaryOffset = new Color32(143, 219, 240, 255);
    public static Color secondary = new Color32(216, 88, 122, 255);
    public static Color secondaryOffset = new Color32(196, 72, 105, 255);
    public static Color fieldPrimary = new Color32(6,123,194, 255);
    public static Color fieldPrimaryOffset = new Color32(242, 239, 229, 255);
    public static Color fieldSecondary = new Color32(100,97,160, 255);
    public static Color fieldSecondaryOffset = new Color32(242, 239, 229, 255);
    public static Color fieldFlag = new Color32(41, 189, 67, 255);
    public static Color fieldFlagOffset = new Color32(41, 189, 67, 255);
}

public enum Coloring { Primary, Secondary }