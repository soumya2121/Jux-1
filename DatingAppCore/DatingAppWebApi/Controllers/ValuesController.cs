using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingAppDal.Context;
using DatingAppDal.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingAppWebApi.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        DatingAppDbContext _context;
        public ValuesController(DatingAppDbContext context)
        {
            _context = context;
        }
        // GET api/values
        [HttpGet]
        
        public async Task<ActionResult> Get()
        {
            var values=await  _context.Values.ToListAsync<Value>();
            return Ok(values);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
