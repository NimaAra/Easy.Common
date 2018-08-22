namespace Easy.Common.XAML.Sample.Messages
{
    public abstract class MessageBase
    {
        /// <summary>
        /// Gets the sender of this message.
        /// </summary>
        public object Sender { get; }

        protected MessageBase(object sender)
        {
            Sender = sender;
        }
    }
}