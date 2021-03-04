using System;
using System.Collections.Generic;

[Serializable]
public class TutorialData
{
	public StoredValue<List<int>> Passed;

	public TutorialData()
	{
		Passed = new StoredValue<List<int>>();
	}
}