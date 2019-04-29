using System.Threading.Tasks;

namespace Stormancer.Plugins
{
    public interface IAuthenticationProvider
    {
        void Initialize();

        Task<AuthParameters> GetAuthArgs();
    }
}