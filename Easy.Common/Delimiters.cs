namespace Easy.Common
{
    /// <summary>
    /// Contains most commonly used delimiters as <see cref="T:char[]"/>.
    /// </summary>
    public static class Delimiters
    {
        /// <summary>
        /// Gets a delimiter as <c>','</c>.
        /// </summary>
        public static char[] Comma { get; } = { ',' };

        /// <summary>
        /// Gets a delimiter as <c>'.'</c>.
        /// </summary>
        public static char[] Dot { get; } = { '.' };

        /// <summary>
        /// Gets a delimiter as <c>';'</c>.
        /// </summary>
        public static char[] SemiColon { get; } = { ';' };

        /// <summary>
        /// Gets a delimiter as <c>':'</c>.
        /// </summary>
        public static char[] Colon { get; } = { ':' };

        /// <summary>
        /// Gets a delimiter as <c>' '</c>.
        /// </summary>
        public static char[] Space { get; } = { ' ' };

        /// <summary>
        /// Gets a delimiter as <c>'\t'</c>.
        /// </summary>
        public static char[] Tab { get; } = { '\t' };

        /// <summary>
        /// Gets a delimiter as <c>'|'</c>.
        /// </summary>
        public static char[] Pipe { get; } = { '|' };
    }
}