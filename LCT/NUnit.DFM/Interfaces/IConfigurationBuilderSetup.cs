using Microsoft.Extensions.Configuration;

namespace NUnit.DFM.Interfaces
{
    public interface IConfigurationBuilderSetup
    { 
        IConfigurationRoot Create();
    }
}
