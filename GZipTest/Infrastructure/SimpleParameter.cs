using Ninject.Parameters;

namespace GZipTest.Infrastructure
{
    /// <summary>
    ///     Modifies an activation process in some way.
    /// </summary>
    public class SimpleParameter<T> : Parameter
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SimpleParameter"/> class.
        /// </summary>
        public SimpleParameter(T value) : this(DefName, value) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SimpleParameter"/> class.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        public SimpleParameter(string name, T value)
            : base(name, value, false)
        {
            Value = value;
        }

        /// <summary>
        ///     Gets the name of the parameter.
        /// </summary>
        static string DefName
        {
            get { return typeof(T).Name; }
        }

        /// <summary>
        ///     Gets the value of the parameter.
        /// </summary>
        public T Value { get; private set; }
    }
}
