using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LaserColorInfo
{
	public Color LaserColor;
	public LayerMask LaserLayer;
}

public static class LaserData {
	public static readonly Dictionary<LaserType, LaserColorInfo> Lasers = new Dictionary<LaserType, LaserColorInfo>()
	{
		{ LaserType.Red, new LaserColorInfo () {LaserColor = Color.red, LaserLayer = LayerMask.NameToLayer("Red") } },
		{LaserType.Yellow, new LaserColorInfo() {LaserColor = Color.yellow, LaserLayer = LayerMask.NameToLayer("Yellow") } },
		{LaserType.Blues, new LaserColorInfo() {LaserColor = Color.blue, LaserLayer = LayerMask.NameToLayer("Blue") } }
	};
}