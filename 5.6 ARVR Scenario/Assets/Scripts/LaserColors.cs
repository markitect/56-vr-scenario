using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserColors {

    public static Color[] LaserColor = new Color[] { Color.red, Color.yellow, Color.blue };

	public static LayerMask[] LaserLayerMasks = new LayerMask[] { LayerMask.NameToLayer("Red") , LayerMask.NameToLayer("Yellow"), LayerMask.NameToLayer("Blue")};

	public static int GetIndexFromColor(Color color)
	{
		for (var i = 0; i < LaserColor.Length; ++i)
		{
			if (LaserColor[i] == color)
			{
				return i;
			}
		}

		throw new ArgumentException("Color does not exist in possible laser color array.");
	}

}
