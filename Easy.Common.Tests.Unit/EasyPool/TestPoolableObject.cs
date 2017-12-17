namespace Easy.Common.Tests.Unit.EasyPool
{
    using Easy.Common;
    using Easy.Common.Interfaces;

    internal class TestPoolableObject : IPoolableObject
    {
        protected string BackingText = "Default";
        public string Text
        {
            get { return BackingText; }
            set { BackingText = value; }
        }

        protected int BackingNumber = 666;
        public int Number
        {
            get { return BackingNumber; }
            set { BackingNumber = value; }
        }

        protected IObjectPool Pool;

        /// <summary>
        /// Sets the <see cref="IObjectPool"/> for the <see cref="IPoolableObject"/>.
        /// </summary>
        /// <param name="pool">The object pool which stores the <see cref="IPoolableObject"/>.</param>
        public void SetPoolManager(IObjectPool pool)
        {
            Ensure.NotNull(pool, nameof(pool));
            Pool = pool;
        }

        /// <summary>
        /// Resets and returns the <typeparamref name="{T}"/> back to the <see cref="Pool"/>.
        /// </summary>
        public virtual void Dispose()
        {
            BackingText = "Default";
            BackingNumber = 666;
            Pool.Return(this);
        }
    }

    internal sealed class TestNonResetPoolableObject : TestPoolableObject
    {
        public override void Dispose()
        {
            Pool.Return(this);
        }
    }
}