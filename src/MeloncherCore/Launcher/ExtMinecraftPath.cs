using CmlLib.Core;

namespace MeloncherCore.Launcher
{
    public class ExtMinecraftPath : MinecraftPath
    {
        public string Natives { get; set; }
        public string RootPath { get; set; }
        public string MinecraftPath { get; set; }

        public ExtMinecraftPath(string rootPath, string profileType, string profileName)
		{
		}
        public ExtMinecraftPath(string rootPath, string profilePath)
        {
            BasePath = NormalizePath(profilePath);
            RootPath = NormalizePath(rootPath + "/" + profilePath);
            MinecraftPath = NormalizePath(rootPath + "/minecraft");

            Library = NormalizePath(MinecraftPath + "/libraries");
            Versions = NormalizePath(MinecraftPath + "/versions");
            Resource = NormalizePath(MinecraftPath + "/resources");

            Runtime = NormalizePath(MinecraftPath + "/runtime");
            Assets = NormalizePath(MinecraftPath + "/assets");
            Natives = NormalizePath(MinecraftPath + "/natives");

            CreateDirs();
        }

        override public string GetNativePath(string id)
        {
            return NormalizePath($"{Natives}/{id}");
        }
    }
}
