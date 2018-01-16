namespace Easy.Common.Extensions
{
    using System.Text;

    /// <summary>
    /// Extensions for <see cref="StringBuilder"/>.
    /// </summary>
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// Appends the given <paramref name="text"/> <paramref name="count"/> times.
        /// </summary>
        public static StringBuilder AppendMultiple(this StringBuilder builder, string text, uint count)
        {
            Ensure.NotNull(builder, nameof(builder));
            Ensure.NotNull(text, nameof(text));

            for (var i = 0; i < count; i++)
            {
                builder.Append(text);
            }
            return builder;
        }
        
        /// <summary>
        /// Appends the given <paramref name="character"/> <paramref name="count"/> times.
        /// </summary>
        public static StringBuilder AppendMultiple(this StringBuilder builder, char character, uint count)
        {
            Ensure.NotNull(builder, nameof(builder));
            
            for (var i = 0; i < count; i++)
            {
                builder.Append(character);
            }
            return builder;
        }

        /// <summary>
        /// Appends space <paramref name="count"/> times.
        /// </summary>
        /// <returns></returns>
        public static StringBuilder AppendSpace(this StringBuilder builder, uint count)
            => builder.AppendMultiple(' ', count);
    }
}