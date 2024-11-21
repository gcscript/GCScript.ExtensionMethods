using System.ComponentModel;
using System.Reflection;

namespace GCScript.ExtensionMethods;
public static class GCScriptEnumExtensions {
	public static string GetDescription(this Enum value) {
		var field = value.GetType().GetField(value.ToString());
		var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
		return attribute?.Description ?? value.ToString();
	}
}
