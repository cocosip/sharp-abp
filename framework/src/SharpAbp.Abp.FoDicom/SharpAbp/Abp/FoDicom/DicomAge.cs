namespace SharpAbp.Abp.FoDicom
{
    public class DicomAge
    {
        public int Age { get; set; }

        public DicomAgeMode Mode { get; set; }

        public DicomAge()
        {

        }

        public DicomAge(int age, DicomAgeMode mode)
        {
            Age = age;
            Mode = mode;
        }

    }

    public enum DicomAgeMode
    {
        D = 1,
        W = 2,
        M = 4,
        Y = 8
    }


}
