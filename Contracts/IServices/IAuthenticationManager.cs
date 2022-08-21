using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.DataTransferObjects;

namespace Contracts.IServices
{
   public interface IAuthenticationManager
    {

        Task<bool> ValidateUserAsync(UserForAuthenticationDto userForAuth);
        Task<string> CreateTokenAsync();


    }
}
