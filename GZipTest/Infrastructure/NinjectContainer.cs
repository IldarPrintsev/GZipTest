using GZipTest.Archiver;
using GZipTest.Common;
using GZipTest.Interfaces;
using GZipTest.Managers;
using Ninject.Modules;
using System;
using System.Linq;

namespace GZipTest.Infrastructure
{
    /// <summary>
    ///     Defines the bindings for the application.
    /// </summary>
    public class NinjectContainer : NinjectModule
    {
        /// <summary>
        ///     Loads the module into the kernel.
        /// </summary>
        public override void Load()
        {
            try
            {
                this.Bind<ISettingsManager>().To<SettingsManager>();

                this.Bind<IArchiveManager>().To<ArchiveManager>();

                this.Bind<IArchiver>().To<GZipCompressor>()
                    .When(r =>
                    {
                        var prm = r.Parameters.OfType<SimpleParameter<OperationType>>().SingleOrDefault();
                        return string.Equals(prm.Value, OperationType.Compress);
                    }
                    );

                this.Bind<IArchiver>().To<GZipDecompressor>()
                    .When(r =>
                    {
                        var prm = r.Parameters.OfType<SimpleParameter<OperationType>>().SingleOrDefault();
                        return string.Equals(prm.Value, OperationType.Decompress);
                    }
                    );
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurs during loading the module into the kernel: {ex.Message}", ex);
            }
        }
    }
}
