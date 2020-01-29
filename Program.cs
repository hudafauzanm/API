using System;
using Todos;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http;
using McMaster.Extensions.CommandLineUtils;
using System.Collections.Generic;
using System.Linq;

namespace RestApiTest
{
    class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var client = new HttpClient();
            
            var root = new CommandLineApplication()
            {
                Name = "#9 Get screenshots from a list of file",
                Description = "Get screenshots from a list of file",
                ShortVersionGetter = () => "1.0.0",
            };

            root.Command("list",app => 
            {
                app.Description = "Get screenshots from a list of file";
                
                app.OnExecuteAsync(async cancellationToken => 
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get,"http://localhost:3000/posts");
                    HttpResponseMessage response = await client.SendAsync(request);
                    var json = await response.Content.ReadAsStringAsync();
                    
                    var list = JsonSerializer.Deserialize<List<Todo>>(json);
                    Console.WriteLine("TO DO LIST OF MINE");
                    foreach(var x in list)
                    {
                        Console.WriteLine(x.id+"."+" | "+x.activity+" | "+ x.status);
                    }
                });
            });

            root.Command("add",app => 
            {
                app.Description = "Get screenshots from a list of file";

                var text = app.Argument("Text","Masukkan Text");
                
                app.OnExecuteAsync(async cancellationToken => 
                {
                    var add = new Todo()
                    {
                        activity = text.Value,
                    };

                    var data = JsonSerializer.Serialize(add);
                    var hasil = new StringContent(data,Encoding.UTF8,"application/json");
                    var response = await client.PostAsync("http://localhost:3000/posts",hasil);
                });
            });

            root.Command("clear",app => 
            {
                app.Description = "Get screenshots from a list of file";
                
                
                app.OnExecuteAsync(async cancellationToken => 
                {
                    Prompt.GetYesNo("Yakin kah?",false);
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get,"http://localhost:3000/posts");
                    HttpResponseMessage response = await client.SendAsync(request);
                    var json = await response.Content.ReadAsStringAsync();
                    
                    var list = JsonSerializer.Deserialize<List<Todo>>(json);
                    var x = from l in list select l.id;
                    foreach(var y in x)
                    {
                        var responses = await client.DeleteAsync($"http://localhost:3000/posts/{y}");
                    }
                });
            });

            root.Command("update",app => 
            {
                app.Description = "Get screenshots from a list of file";

                var text = app.Argument("Text","Masukkan Text",true);
                app.OnExecuteAsync(async cancellationToken => 
                {
                    var add = "{" + "\"activity\":" + $"\"{text.Values[1]}\"" + "}";
                    var hasil = new StringContent(add,Encoding.UTF8,"application/json");
                    var responses = await client.PatchAsync($"http://localhost:3000/posts/{text.Values[0]}",hasil);
                });
            });

            root.Command("delete",app => 
            {
                app.Description = "Get screenshots from a list of file";
                var text = app.Argument("Text","Masukkan Text");
                app.OnExecuteAsync(async cancellationToken => 
                {   
                    var responses = await client.DeleteAsync($"http://localhost:3000/posts/{text.Value}");
                });
            });

            root.Command("done",app => 
            {
                app.Description = "Get screenshots from a list of file";

                var text = app.Argument("Text","Masukkan Text");
                app.OnExecuteAsync(async cancellationToken => 
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get,"http://localhost:3000/posts");
                    HttpResponseMessage response = await client.SendAsync(request);
                    var json = await response.Content.ReadAsStringAsync();
                    
                    var add = "{" + "\"status\":" + "\"Done\"" + "}";
                    var hasil = new StringContent(add,Encoding.UTF8,"application/json");
                    var responses = await client.PatchAsync($"http://localhost:3000/posts/{text.Value}",hasil);
                });
            });

            return root.Execute(args);
        }
    }
}
