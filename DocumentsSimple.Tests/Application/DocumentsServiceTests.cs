using DocumentsSimple.Application;
using DocumentsSimple.Entities;
using DocumentsSimple.Exceptions;
using DocumentsSimple.Infrastructure;
using DocumentsSimple.Tests.Stubs;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentsSimple.Tests.Application
{
    public class DocumentsServiceTests
    {
        DocumentsService _documentsService;
        IDocumentsStorage _docStorageStub;
        Mock<IBlobStorageAdapter> _blobMock;

        [SetUp]
        public void Setup()
        {
            _blobMock = new Mock<IBlobStorageAdapter>();
            _docStorageStub = new DocumentsStorageAdapterStub();
            _documentsService = new DocumentsService(_blobMock.Object, _docStorageStub);
        }

        [Test]
        public async Task GetAll_Should_ReturnExistingDocuments()
        {
            //Arrange
            string[] newDocumentIds = new[]
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString()
            };
            await _docStorageStub.Add(new Document
            {
                Id = newDocumentIds[0]
            });
            await _docStorageStub.Add(new Document
            {
                Id = newDocumentIds[1]
            });

            //Act
            var documents = await _documentsService.GetAll();

            //Assert

            Assert.AreEqual(newDocumentIds.Length, documents.Count());
            Assert.AreEqual(1, documents.Where(x => x.Id == newDocumentIds[0]).Count());
        }

        [Test]
        public async Task GetAll_Should_ReturnEmptyCollectionWhenNoDocumentsAreAvailable()
        {
            //Arrange
            _documentsService = new DocumentsService(_blobMock.Object, _docStorageStub);

            //Act
            var documents = await _documentsService.GetAll();

            //Assert
            Assert.AreEqual(0, documents.Count());
        }

        [Test]
        public async Task Upload_Should_AddNewDocument()
        {
            //Arrange
            _documentsService = new DocumentsService(_blobMock.Object, _docStorageStub);
            var formFile = new Mock<IFormFile>();
            formFile.Setup(x => x.ContentType).Returns("application/pdf");
            formFile.Setup(x => x.FileName).Returns("testFileName.pdf");
            formFile.Setup(x => x.Length).Returns(2);
            var testLocation = "testLocation";

            //Act
            Document doc = await _documentsService.Upload(formFile.Object, testLocation);
            var actual = await _documentsService.Get(doc.Id);

            //Assert
            Assert.NotNull(actual);
            Assert.AreEqual(doc.Name, actual.Name);
            Assert.AreEqual(doc.Location, actual.Location);
            Assert.AreEqual(doc.Size, actual.Size);
        }

        [Test]
        public void Upload_Should_Throw_OnNonPdfDoc()
        {
            //Arrange
            _documentsService = new DocumentsService(_blobMock.Object, _docStorageStub);
            var formFile = new Mock<IFormFile>();
            formFile.Setup(x => x.ContentType).Returns("application/json");

            //Act& Assert
            HttpStatusCodeException ex = Assert.ThrowsAsync<HttpStatusCodeException>(async () => await _documentsService.Upload(formFile.Object, ""));
            Assert.AreEqual("file should be in pdf format", ex.Message);
        }

        [Test]
        public void Upload_Should_Throw_OnNonTooLagreFile()
        {
            //Arrange
            _documentsService = new DocumentsService(_blobMock.Object, _docStorageStub);
            var formFile = new Mock<IFormFile>();
            formFile.Setup(x => x.ContentType).Returns("application/pdf");
            formFile.Setup(x => x.FileName).Returns("testFileName.pdf");
            formFile.Setup(x => x.Length).Returns(25000000);

            //Act& Assert
            HttpStatusCodeException ex = Assert.ThrowsAsync<HttpStatusCodeException>(async () => await _documentsService.Upload(formFile.Object, ""));
            Assert.AreEqual("file size is too large", ex.Message);
        }
    }
}
