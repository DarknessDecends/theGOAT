using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;

/// <summary>
/// Hierarchy Organizer v3.0
/// By Isaiah Kelly 
/// @IsaiahKelly on Twitter
/// isaiah@dangerofdeath.com
/// </summary>

public static class Sorting
{
	static void SortChildren(GameObject gameObject)
	{
		// Get selected transforms.
		Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();
		
		int newIndex = GetLowestIndex (transforms);

		// Sort the transforms by number.
		foreach (Transform t in transforms.Cast<Transform>().OrderBy(t => findInt(t.name)))
		{
			if (t != t.root)
			{
				t.SetSiblingIndex (newIndex);
				newIndex ++;
			}
		}

		Debug.Log ("Children Sorted.");
	}
	
	static int GetLowestIndex (Transform[] t) 
	{
		int lowestIndex = 9999;
		int index;
		
		for (int i = 0; i < t.Length; i++) 
		{
			index = t[i].GetSiblingIndex();
			
			if (index < lowestIndex) 
			{
				lowestIndex = index;
			}
		}
		
		return lowestIndex;
	}

	static int findInt(string name) {
		int number;
		int.TryParse(Regex.Match(name, @"\d+").Value, out number);
		return number;
	}

}
