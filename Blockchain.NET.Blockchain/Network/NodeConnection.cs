using Blockchain.NET.Core.Mining;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.NET.Blockchain.Network
{
    public class NodeConnection
    {
        public string NodeAddress { get; set; }

        public string BaseApiRoute { get; set; } = "api/v1/blockchain";

        public NodeConnection(string nodeAddress)
        {
            NodeAddress = nodeAddress;
            BaseApiRoute = NodeAddress + BaseApiRoute;
        }

        public async Task<bool> Health()
        {
            string apiRoute = BaseApiRoute + "/health";
            try
            {
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(apiRoute))
                {
                    return response.StatusCode == System.Net.HttpStatusCode.OK;
                }
            }
            catch (Exception exc)
            {
                return false;
            }
        }

        public async Task<Block> LastBlock()
        {
            string apiRoute = BaseApiRoute + "/lastblock";
            try
            {
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(apiRoute))
                using (HttpContent content = response.Content)
                {
                    return JsonConvert.DeserializeObject<Block>(await content.ReadAsStringAsync());
                }
            }
            catch (Exception exc)
            {
                return null;
            }
        }

        public async Task<int> LastBlockHeight()
        {
            try
            {
                string apiRoute = BaseApiRoute + "/lastblockheight";
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(apiRoute))
                using (HttpContent content = response.Content)
                {
                    int.TryParse(await content.ReadAsStringAsync(), out int result);
                    return result;
                }
            }
            catch (Exception exc)
            {
                return 0;
            }
        }

        public async Task<string> BlockchainHash(int blockHeight)
        {
            try
            {
                string apiRoute = BaseApiRoute + $"/getblockchainhash/{blockHeight}";
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(apiRoute))
                using (HttpContent content = response.Content)
                {
                    return await content.ReadAsStringAsync();
                }
            }
            catch (Exception exc)
            {
                return string.Empty;
            }
        }

        public async Task<List<string>> MempoolHashes()
        {
            try
            {
                string apiRoute = BaseApiRoute + "/mempoolhashes";
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(apiRoute))
                using (HttpContent content = response.Content)
                {
                    return JsonConvert.DeserializeObject<List<string>>(await content.ReadAsStringAsync());
                }
            }
            catch (Exception exc)
            {
                return null;
            }
        }

        public async Task<List<string>> BlockHashes()
        {
            try
            {
                string apiRoute = BaseApiRoute + "/blockhashes";
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(apiRoute))
                using (HttpContent content = response.Content)
                {
                    return JsonConvert.DeserializeObject<List<string>>(await content.ReadAsStringAsync());
                }
            }
            catch (Exception exc)
            {
                return null;
            }
        }

        public async Task<Block> GetBlock(int blockHeight)
        {
            try
            {
                string apiRoute = BaseApiRoute + $"/getblock/{blockHeight}";
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(apiRoute))
                using (HttpContent content = response.Content)
                {
                    return JsonConvert.DeserializeObject<Block>(await content.ReadAsStringAsync());
                }
            }
            catch (Exception exc)
            {
                return null;
            }
        }

        public async Task<List<Block>> GetBlocks(List<int> blockHeights)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiRoute = BaseApiRoute + $"/getblocks";
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, apiRoute))
                    {
                        request.Content = new StringContent(JsonConvert.SerializeObject(blockHeights), Encoding.UTF8, "application/json");
                        var response = await client.SendAsync(request);
                        return JsonConvert.DeserializeObject<List<Block>>(await response.Content.ReadAsStringAsync());
                    }
                }
            }
            catch (Exception exc)
            {
                return null;
            }

        }

        public async Task<List<Transaction>> GetTransactions(List<string> hashes)
        {
            try
            {
                string apiRoute = BaseApiRoute + $"/gettransactions";
                using (HttpClient client = new HttpClient())
                {
                    var buffer = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(hashes));
                    var byteContent = new ByteArrayContent(buffer);
                    using (HttpResponseMessage response = await client.PostAsync(apiRoute, byteContent))
                    using (HttpContent content = response.Content)
                    {
                        return JsonConvert.DeserializeObject<List<Transaction>>(await content.ReadAsStringAsync());
                    }
                }

            }
            catch (Exception exc)
            {
                return null;
            }
        }

        public async Task<bool> PushBlock(Block block)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiRoute = BaseApiRoute + $"/pushblock";
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, apiRoute))
                    {
                        request.Content = new StringContent(block.ToJson(), Encoding.UTF8, "application/json");

                        var response = await client.SendAsync(request);
                        return response.StatusCode == System.Net.HttpStatusCode.OK;
                    }
                }
            }
            catch (Exception exc)
            {
                return false;
            }
        }

        public async Task<bool> PushTransaction(Transaction transaction)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiRoute = BaseApiRoute + $"/pushtransaction";
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, apiRoute))
                    {
                        request.Content = new StringContent(transaction.ToJson(), Encoding.UTF8, "application/json");

                        var response = await client.SendAsync(request);
                        return response.StatusCode == System.Net.HttpStatusCode.OK;
                    }
                }
            }
            catch (Exception exc)
            {
                return false;
            }
        }
    }
}
