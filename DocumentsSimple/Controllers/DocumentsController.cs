using DocumentsSimple.Application;
using DocumentsSimple.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentsSimple.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly DocumentsService _documents;

        public DocumentsController(DocumentsService documents)
        {
            _documents = documents;
        }

        [HttpGet]
        public async Task<ActionResult<Document[]>> Get()
        {
            var documents = await _documents.GetAll();

            return documents.ToArray();
        }

        [HttpPost]
        public async Task<ActionResult<Document>> Upload(IFormFile file)
        {
            if (file == null)
                return BadRequest();
            var downloadLocation = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/api/Download";

            var uploadedDocument = await _documents.Upload(file, downloadLocation);

            return uploadedDocument;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _documents.Delete(id);

            return NoContent();
        }
    }
}
