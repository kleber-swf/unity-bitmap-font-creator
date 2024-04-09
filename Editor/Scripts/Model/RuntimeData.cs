using UnityEngine;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	// [CreateAssetMenu(fileName = "RuntimeData", menuName = "RuntimeData", order = 0)]
	internal class RuntimeData : ScriptableObject
	{
		private const string DataPath = "RuntimeData";
		public ExecutionData Data;

		public static RuntimeData Load()
		{
			var data = Resources.Load<RuntimeData>(DataPath);
			data.Data ??= new ExecutionData();
			return data;
		}
	}
}
