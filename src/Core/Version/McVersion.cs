namespace MeloncherCore.Version
{
    class McVersion
    {
        public McVersion(string Name, string Type, string ProfileName)
        {
            this.Name = Name;
            this.Type = Type;
            this.ProfileName = ProfileName;
        }

        public string Name { get; set; }
        public string Type { get; set; }
        public string ProfileName { get; set; }
    }
}
