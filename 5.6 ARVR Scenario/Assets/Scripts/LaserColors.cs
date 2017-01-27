using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserColors {

    private static Color red = Color.red;
    private static Color blue = Color.blue;
    private static Color green = Color.green;
    public static Color Green { get { return green; } }
    public static Color Red { get { return red; } }
    public static Color Blue {  get { return blue; } }

    public static Color[] LaserColor = new Color[] { red, green, blue };

}
