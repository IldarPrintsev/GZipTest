using GZipTest.Archiver;
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
                        var prm = r.Parameters.OfType<SimpleParameter<string>>().SingleOrDefault();
                        return string.Equals(prm.Value, "compress");
                    }
                    );

                this.Bind<IArchiver>().To<GZipDecompressor>()
                    .When(r =>
                    {
                        var prm = r.Parameters.OfType<SimpleParameter<string>>().SingleOrDefault();
                        return string.Equals(prm.Value, "decompress");
                    }
                    );
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
