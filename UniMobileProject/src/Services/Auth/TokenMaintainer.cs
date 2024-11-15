using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniMobileProject.src.Models.ServiceModels.AuthModels;
using UniMobileProject.src.Services.Database;
using UniMobileProject.src.Services.Database.Models;

namespace UniMobileProject.src.Services.Auth
{
    public class TokenMaintainer
    {
        DatabaseService _dbService;
        public TokenMaintainer(string testDbPath)
        {
            _dbService = new DatabaseService(testDbPath);
        }
        public TokenMaintainer()
        {
            _dbService = new DatabaseService();
        }

        public async Task<bool> SetToken(SuccessfulAuth authData)
        {
            var token = await _dbService.GetToken();
            bool success;
            if (token.Id == 0)
            {
                success = await _dbService.AddToken(new Database.Models.Token(authData));
            }
            else
            {
                success = await _dbService.UpdateToken(token);
            }
            return success;
        }

        public async Task<Token?> GetToken()
        {
            var token = await _dbService.GetToken();
            return token;
        }
        // TODO
        /*
        Set Token (
         if exists - update
         if does not - create
        )
         */
    }
}
