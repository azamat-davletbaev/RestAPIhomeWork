using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Http = System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace WebApi.Controllers
{
    [Route("customers")]
    public class CustomerController : Controller
    {
        private static List<Customer> customers = new List<Customer>();

        [HttpGet("{id:long}")]
        public IActionResult GetCustomerAsync([FromRoute] long id)
        {
            var tmp = customers.Where(x => x.Id == id).FirstOrDefault();

            if (tmp == null)                
                return new NotFoundObjectResult(HttpStatusCode.NotFound);
            else
                return new ObjectResult(tmp);
        }

        [HttpGet]
        public Task<List<Customer>> GetAllCustomersAsync()
        {            
            return Task.FromResult(customers);
        }

        [HttpPost, Route("random")]
        public Task<Customer> CreateRandomCustomerAsync([FromBody] CustomerCreateRequest customerRequest)
        {
            var id = new Random().Next(0, 100);

            var customer = new Customer
            {
                Id = id,
                Firstname = $"{customerRequest.Firstname}({id})",
                Lastname = $"{customerRequest.Lastname}({id})"
            };
            customers.Add(customer);

            return Task.FromResult(customer);
        }

        [HttpPost]
        public IActionResult CreateCustomer([FromBody] Customer customer)
        {
            if (customers.Where(x => x.Id == customer.Id).Any())            
                return StatusCode(StatusCodes.Status403Forbidden);                                                            
            else
            {
                customers.Add(customer);
                return Ok();
            }
        }
    }
}