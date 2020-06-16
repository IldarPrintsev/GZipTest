using Ninject;
using System;
using System.Linq;

namespace GZipTest.Infrastructure
{
    /// <summary>
    ///     Represents the <see cref="NinjectContainer"/> wrapper.
    /// </summary>
    public class NinjectWrapper
    {
        private readonly IKernel _kernel;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NinjectWrapper"/> class.
        /// </summary>
        public NinjectWrapper()
        {
            this._kernel = new StandardKernel();
            this._kernel.Load(AppDomain.CurrentDomain.GetAssemblies());
        }

        /// <summary>
        ///     Gets an implementation of an interface.
        /// </summary>
        public T Get<T, K>(K param)
        {
            return _kernel.GetAll<T>(new SimpleParameter<K>(param)).FirstOrDefault();
        }
    }
}
