using System;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace SaluteOnline.ChatService.DAL
{
    public class ChatRepository : IChatRepository
    {
        private readonly IMongoDatabase _mongoDb;

        public ChatRepository(IConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            _mongoDb = new MongoClient(configuration.GetValue<string>("MongoSettings:Path"))
                   .GetDatabase(configuration.GetValue<string>("MongoSettings:DB"));
        }
    }
}
