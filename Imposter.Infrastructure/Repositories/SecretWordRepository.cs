using Imposter.Core.Domain.Entities;
using Imposter.Core.Domain.Enums;
using Imposter.Core.RepositoriesContracts;
using Imposter.Infrastructure.Dbcontext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imposter.Infrastructure.Repositories
{
    public class SecretWordRepository : ISecretWordRepository
    {
        private readonly AppDbContext _appDbContext;
        public SecretWordRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<SecretWord> AddSecretWord(string Text, CategoryOptions categoryOptions)
        {
            var secretWord = new SecretWord
            {
                SecretWordId = Guid.NewGuid(),
                Text = Text,
                Category = categoryOptions
            };
            await _appDbContext.secretWords.AddAsync(secretWord);
            await _appDbContext.SaveChangesAsync();
            return secretWord;
        }
        public int GetCount()
        {
            return _appDbContext.secretWords.Count();
        }
        public async Task<SecretWord> PickRandomSecretWord()
        {
            var random = new Random();
            int value = random.Next(0, GetCount());
            return await _appDbContext.secretWords.Skip(value).FirstAsync();
        }
        public async Task<SecretWord> GetSecretWord(Guid SecretWordId)
        {
            return await _appDbContext.secretWords.FirstAsync(sw => sw.SecretWordId == SecretWordId);
        }
    }
}
