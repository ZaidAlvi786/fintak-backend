using FintrakBanking.ViewModels.Credit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FailedPostingManager
{
    public class Sender
    {
        private HttpClient httpClient;
        private readonly Reader reader;
        public Sender(Reader _reader)
        {
            this.httpClient = new HttpClient();
            reader = _reader;
        }
        public void Send(List<FailedTransactionsViewModel> transactions, Reader reader)
        {
            foreach (var trans in transactions)
            {
                Post(trans);
            }
        }
        

        private async void Post(FailedTransactionsViewModel transaction)
        {
            var json = JsonConvert.SerializeObject(transaction);
            this.httpClient.DefaultRequestHeaders.Clear();
           
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
          
            var content = new StringContent(transaction.requestBody, Encoding.UTF8, "application/json");
            var responseString = await this.httpClient.PostAsync(transaction.destination, content);
            if((int)responseString.StatusCode > 199 && (int)responseString.StatusCode < 300)
            {
                UpdateRecord(transaction.transactionId, true);

            }
            else
            {
                 var result = responseString.Content.ReadAsStringAsync();
                UpdateRecord(transaction.transactionId, false);
            }
            
           

           
        }

        private async void UpdateRecord(int failedTransactionId, Boolean status)
        {
            await reader.Update(failedTransactionId, status);
        }
    }
}
