using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Kryz.UnityDI.Tests.Editor
{
	public static class PackagePath
	{
		public static readonly string Path = PackageInfo.FindForAssembly(typeof(PackagePath).Assembly).assetPath;
	}
}