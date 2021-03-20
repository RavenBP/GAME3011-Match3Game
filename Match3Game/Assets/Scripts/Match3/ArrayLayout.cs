using UnityEngine;
using System.Collections;

// Original Script created by Sumeet Khobare - https://www.youtube.com/c/GameDevFreakss

[System.Serializable]
public class ArrayLayout  
{
	[System.Serializable]
	public struct rowData
	{
		public bool[] row;
	}

    public Grid grid;
    public rowData[] rows = new rowData[14];
}
