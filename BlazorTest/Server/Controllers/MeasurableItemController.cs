using BlazorTest.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorTest.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MeasurableItemController
    {

        [HttpGet]
        public IEnumerable<MeasurableItem> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 1000).Select(index => new MeasurableItem
            {
                Text = LoremNET.Lorem.Paragraph(3, 8, 3, 30),
                Id = index.ToString()
            }).ToArray();
        }
    }
}
