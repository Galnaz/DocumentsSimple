using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentsSimple.Entities;

namespace DocumentsSimple.Infrastructure
{
    public interface IDocumentsStorage
    {
        Task Add(Document item);
        Task Delete(string id);
        Task<Document> Get(string id);
        Task<IEnumerable<Document>> GetAll();
        Task Update(string id, Document item);
    }
}