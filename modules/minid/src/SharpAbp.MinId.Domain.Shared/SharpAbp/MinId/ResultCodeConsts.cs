namespace SharpAbp.MinId
{
    public class ResultCodeConsts
    {
        /// <summary>
        /// Normal
        /// </summary>
        public static int Normal = 1;

        /// <summary>
        /// Need load nextId
        /// </summary>
        public static int Loading = 2;

        /// <summary>
        /// More than maxId
        /// </summary>
        public static int Over = 4;
    }
}
