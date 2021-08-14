namespace cangulo.cicd.abstractons.Models
{
    public class ReleaseNumber
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Patch { get; set; }
        public override string ToString() => $"{Major}.{Minor}.{Patch}";
    }
}
