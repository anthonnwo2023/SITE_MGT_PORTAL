using Project.V1.DLL.Services.Interfaces.FormSetup;
using Project.V1.Lib.Interfaces;

namespace Project.V1.Lib.Services;

public class Spectrum : GenericRepo<SpectrumViewModel>, ISpectrum, IDisposable
{
    public Spectrum(ApplicationDbContext context, ICLogger logger)
        : base(context, "", logger)
    {
    }
}
