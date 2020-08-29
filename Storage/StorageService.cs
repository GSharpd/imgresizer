using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace imgresizer.Storage
{
    public class StorageService
    {
        private readonly CloudBlobClient _client;
        private readonly ILogger<StorageService> _logger;
        private readonly IConfiguration _configuration;

        public StorageService(ILogger<StorageService> logger, CloudBlobClient client, IConfiguration configuration)
        {
            _logger = logger;
            _client = client;
            _configuration = configuration;
        }
        public async Task<(bool, CloudBlockBlob)> TryGetFile(string name)
        {
            try
            {
                var container = _client.GetContainerReference(_configuration["StorageAccount:Container"]);
                var blob = container.GetBlockBlobReference(name);
                if (await blob.ExistsAsync())
                {
                    return (true, blob);
                }
            }
            catch(Exception error)
            {
                _logger.LogError(error, $"Could not retrieve file {name}");
            }

            return (false, null);
        }

        public async Task<byte[]> GetBlobBytes(CloudBlockBlob blob)
        {
            using(var memory = new MemoryStream())
            {
                await blob.DownloadToStreamAsync(memory);
                return memory.ToArray();
            }
        }
    }
}
