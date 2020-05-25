using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Serialization.Json;
using RestSharpDemo.Model;

namespace RestSharpDemo
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void TestMethod1()
        {
            var client = new RestClient("http://localhost:3000");
            var request = new RestRequest("posts/{postid}", Method.GET);
            request.AddUrlSegment("postid", 2);

            var response = client.Execute<Posts>(request);

            var deserialize = new JsonDeserializer();
            var output = deserialize.Deserialize<Dictionary<string, string>>(response);
            var result = output["author"];
            Assert.That(result, Is.EqualTo("K Karthic"), "Author is Not correct!");

            //JObject obs = JObject.Parse(response.Content);
            //Assert.That(obs["author"].ToString, Is.EqualTo("K Karthic"), "Author is Not correct!");
        }

        [Test]
        public void PostWithAnonymousClass()
        {
            var client = new RestClient("http://localhost:3000");
            var request = new RestRequest("posts/{postid}/profile", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { name = "Raj" });

            request.AddUrlSegment("postid", 2);

            var response = client.Execute(request);

            var deserialize = new JsonDeserializer();
            var output = deserialize.Deserialize<System.Collections.Generic.Dictionary<string, string>>(response);
            var result = output["name"];
            Assert.That(result, Is.EqualTo("Raj"), "Name is Not correct!");
        }

        [Test]
        public void PostWithTypeClassBody()
        {
            var client = new RestClient("http://localhost:3000");
            var request = new RestRequest("posts", Method.POST);

            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new Posts() { id="17", author="Test", title="Restsharp demo" });

            var response = client.Execute<Posts>(request);

            Assert.That(response.Data.author, Is.EqualTo("Test"), "Name is Not correct!");
        }

        [Test]
        public void PostWithAsync()
        {
            var client = new RestClient("http://localhost:3000");
            var request = new RestRequest("posts", Method.POST);

            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new Posts() { id = "18", author = "Test", title = "Restsharp demo" });

            //var response = client.Execute<Posts>(request);

            var response = ExecuteAsyncRequest<Posts>(client, request).GetAwaiter().GetResult();

            Assert.That(response.Data.author, Is.EqualTo("Test"), "Name is Not correct!");
        }

        private async Task<IRestResponse<T>> ExecuteAsyncRequest<T>(RestClient client, IRestRequest request) where T: class, new()
        {
            var taskCompletionSource = new TaskCompletionSource<IRestResponse<T>>();
           client.ExecuteAsync<T>(request, restResponse =>
             {
                 if (restResponse.ErrorException != null)
                 {
                     const string message = "Error retrieving response.";
                     throw new ApplicationException(message, restResponse.ErrorException);
                 }
                 taskCompletionSource.SetResult(restResponse);
             });
            return await taskCompletionSource.Task;
        }
    }
}
