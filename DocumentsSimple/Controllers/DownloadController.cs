using DocumentsSimple.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentsSimple.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        private IBlobStorageAdapter _blob;
        private readonly IDocumentsStorage _documents;

        public DownloadController(IBlobStorageAdapter blobStorage, IDocumentsStorage documents)
        {
            _blob = blobStorage;
            _documents = documents;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var document = await _documents.Get(id);
            if (document == null)
                return NotFound();
            var downloadStream = await _blob.DownloadFile(id);
            if (downloadStream == null)
                return NotFound();

            return new FileStreamResult(downloadStream, "application/octet-stream")
            {
                FileDownloadName = $"{document.Name}"
            };
        }
    }
}
