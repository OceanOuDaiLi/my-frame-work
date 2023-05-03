namespace FTX
{
	public static class RenderParameter
	{
		public static readonly string depthTest = "_ZTest";
		public static readonly string depthWrite = "_ZWrite";
		public static readonly string cullMode = "_CullMode";

		public static readonly int [] renderQueue = new int []
		{
			2000, // Default
			2020, // Alpha Decals
			2050, // Glass
			2450, // Alpha Test
			3000, // Transparent
			3500, // Skybox
		};
	}
}