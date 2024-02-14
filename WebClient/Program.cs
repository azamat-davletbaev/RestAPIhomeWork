using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebClient
{
    static class Program
    {
        private static string customersURL = "https://localhost:5001/customers";
        private static string customersRandomURL = "https://localhost:5001/customers/random";

        static void Main(string[] args)
        {
            preview();

            HttpClient client = new HttpClient();

            string line = string.Empty;
            while ((line = Console.ReadLine().Trim().ToLower()) != "exit")
            {
                if (line == "new")
                {
                    Console.WriteLine("Введите Id =>");
                    var id = Console.ReadLine();

                    Console.WriteLine("Введите FirstName =>"); 
                    var firstName = Console.ReadLine();

                    Console.WriteLine("Введите LastName =>");
                    var LastName = Console.ReadLine();

                    var flag = CreateCustomer(client, new Customer
                    {
                        Id = Convert.ToInt64(id),
                        Firstname = firstName,
                        Lastname = LastName
                    }); ;

                    if (flag == null)                    
                        Console.WriteLine("Ошибка создания клиента!");                    
                    else if (flag == true)
                        Console.WriteLine($"Клиент {id} {firstName} {LastName} добавлен в БД!");
                    else if (flag == false)
                        Console.WriteLine($"Клиент с Id {id} уже существует в БД!");

                    preview();
                }
                if (line == "random")
                {
                    var randomCustomer = RandomCustomer(client);

                    if (randomCustomer != null && randomCustomer is Customer customer)
                        Console.WriteLine($"Создан новый клиент=> Id:{customer.Id}, Firstname:{customer.Firstname}, Lastname:{customer.Lastname}");
                    else
                        Console.WriteLine("Ошибка создания нового клиента!");

                    preview();
                }
                else if (line.StartsWith("id "))
                {
                    line = line.Trim().Replace("id", "");
                    long id = 0;
                    try
                    {
                        id = Convert.ToInt64(line);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"не правильный id..");                        
                        continue;
                    }                    

                    HttpResponseMessage response = client.GetAsync($"{customersURL}/{id}").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var tmp = response.Content.ReadFromJsonAsync<Customer>().Result;
                        Console.WriteLine($"Найден в БД: {tmp.Id} {tmp.Firstname} {tmp.Lastname}");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        Console.WriteLine($"Клиент с id {id} не найден в БД..");
                    }

                    preview();
                }
                else if (line == "all")
                {
                    HttpResponseMessage response = client.GetAsync(customersURL).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var data = response.Content.ReadFromJsonAsync<List<Customer>>().Result;

                        Console.WriteLine($"Найдено в БД:");

                        data.ForEach((tmp) =>
                        {
                            Console.WriteLine($"{tmp.Id} {tmp.Firstname} {tmp.Lastname}");
                        });
                    }
                    preview();
                }
            }
        }

        private static Customer RandomCustomer(HttpClient client)
        {
            var customerCreateRequest = new CustomerCreateRequest
            {                
                Firstname = "Firstname",
                Lastname = "Lastname"
            };
                        
            string json = JsonConvert.SerializeObject(customerCreateRequest);
            StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");            
            HttpResponseMessage response = client.PostAsync(customersRandomURL, httpContent).Result;

            if (response.IsSuccessStatusCode && response.Content.ReadFromJsonAsync<Customer>().Result is Customer customer)            
                return customer;            
            else            
                return null;                            
        }
        private static bool? CreateCustomer(HttpClient client, Customer customerRequest)
        {            
            string json = JsonConvert.SerializeObject(customerRequest);
            StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(customersURL, httpContent).Result;

            if (response.IsSuccessStatusCode)
                return true;
            else if (response.StatusCode == HttpStatusCode.Forbidden)
                return false;
            else
                return null;
        }

        private static void preview()
        {
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("Введите 'Exit' чтобы завершить приложение");
            Console.WriteLine("Введите 'ID клиента' чтобы получить данные по клиенту, (ID 10 -> вернуть данные по клиенту с ID = 10)");
            Console.WriteLine("Введите 'New' для создания нового клиента");
            Console.WriteLine("-----------------------------------------");
        }
    }
}