using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DocumentStore.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DocumentStore.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly string _mongoDbConnectionString;// = "mongodb://localhost";

        public DocumentsController(IConfiguration con)
        {
            _mongoDbConnectionString = con["DocumentStore:MongoDb"];
        }

        // GET <DocumentsController>/B152713B-0BB7-4B32-86E0-3AD854A29B4F
        [HttpGet("{id}")]
        public async Task<ActionResult<Document>> Get(string id)
        {
            if (!IsGuidValid(id))
                return BadRequest("DocumentId must be GUID. For example: B152713B-0BB7-4B32-86E0-3AD854A29B4F");

            

            var doc = await GetDocument(id);
            if (doc == null) return NotFound();
            return doc;
        }

        // POST <DocumentsController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Document document)
        {
            if (!IsGuidValid(document.Id))
                return BadRequest("DocumentId must be GUID. For example: B152713B-0BB7-4B32-86E0-3AD854A29B4F");

            await PostDocument(document);

            return Ok();
        }

        private bool IsGuidValid(string guid)
        {
            var regex = new Regex(@"^[{(]?[0-9A-F]{8}[-]?(?:[0-9A-F]{4}[-]?){3}[0-9A-F]{12}[)}]?$",
                RegexOptions.IgnoreCase);
            return regex.Match(guid).Success;
        }

        private async Task<Document> GetDocument(string id)
        {
            var client = new MongoClient(_mongoDbConnectionString);
            var database = client.GetDatabase("document_store");
            //var collection = database.GetCollection<BsonDocument>("documents");
            var collection = database.GetCollection<Document>("documents");

            var filter = new BsonDocument("_id", id);
            using (var cursor = await collection.FindAsync(filter))
            {
                //while (await cursor.MoveNextAsync())
                if (await cursor.MoveNextAsync())
                {
                    var documents = cursor.Current;
                    return documents.FirstOrDefault();
                    //foreach (var doc in documents)
                    //{
                    //    var v = doc["data"].AsString;
                    //    return new Document {Id = id, Data = v};
                    //}
                }
                return null;
            }
        }

        private async Task PostDocument(Document doc)
        {
            var client = new MongoClient(_mongoDbConnectionString);
            var database = client.GetDatabase("document_store");
            var collection = database.GetCollection<Document>("documents");

            await collection.InsertOneAsync(doc);
        }
    }
}
