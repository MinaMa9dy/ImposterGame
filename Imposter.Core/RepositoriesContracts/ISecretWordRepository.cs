using Imposter.Core.Domain.Entities;
using Imposter.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imposter.Core.RepositoriesContracts
{
    public interface ISecretWordRepository
    {
        Task<SecretWord> AddSecretWord(string Text, CategoryOptions categoryOptions);
        Task<SecretWord> PickRandomSecretWord();
        Task<SecretWord> GetSecretWord(Guid SecretWordId);
    }
}
