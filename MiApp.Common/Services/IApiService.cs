using MiApp.Common.Models;
using System.Threading.Tasks;

namespace MiApp.Common.Services
{
    public interface IApiService
    {
        Task<Response> GetListAsync<T>(
            string urlBase,
            string servicePrefix,
            string controller);

        Task<Response> GetListAsync<T>(
           string urlBase,
           string servicePrefix,
           string controller,
           string tokenType,
           string accessToken);

        Task<ResponseT<object>> GetListTAsync<T>(
            string urlBase,
            string servicePrefix,
            string controller);

        Task<ResponseT<object>> PostAsync<T>(
           string urlBase,
           string servicePrefix,
           string controller,
           T model,
           string tokenType,
           string accessToken);

        Task<Response> DeleteAsync(
           string urlBase,
           string servicePrefix,
           string controller,
           int id,
           string tokenType,
           string accessToken);

        Task<Response> PutAsync<T>(
            string urlBase,
            string servicePrefix,
            string controller,
            T model,
            string tokenType,
            string accessToken);

        Task<ResponseT<object>> PutAsync<T>(
            string urlBase,
            string servicePrefix,
            string controller,
            int id,
            T model,
            string tokenType,
            string accessToken);



        bool CheckConnection();

        Task<Response> GetTokenAsync(string urlBase, string servicePrefix, string controller, TokenRequest request);

        Task<Response> GetUserByEmail(string urlBase, string servicePrefix, string controller, string tokenType, string accessToken, EmailRequest request);

        Task<Response> RecoverPasswordAsync(string urlBase, string servicePrefix, string controller, EmailRequest emailRequest);

        Task<Response> RegisterUserAsync(string urlBase, string servicePrefix, string controller, UserRequest userRequest);

        

        Task<Response> ChangePasswordAsync(string urlBase, string servicePrefix, string controller, ChangePasswordRequest changePasswordRequest, string tokenType, string accessToken);

    }
}
