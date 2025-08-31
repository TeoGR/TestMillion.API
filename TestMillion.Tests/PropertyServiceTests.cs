using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TestMillion.Domain.Entities;
using TestMillion.Persistence.Context;
using TestMillion.Persistence.Repositories;
using TestMillion.Services.Implementations;
using TestMillion.Services.Models.Request;

namespace TestMillion.Tests;

public class PropertyServiceTests
{
    private AppDbContext _context = null!;
    private PropertyService _service = null!;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        var ownerRepo = new OwnerRepository(_context);
        var propertyRepo = new PropertyRepository(_context);
        var imageRepo = new PropertyImageRepository(_context);
        var traceRepo = new PropertyTraceRepository(_context);
        var storage = new FakeFileStorageService();
        _service = new PropertyService(propertyRepo, ownerRepo, storage, imageRepo, traceRepo, NullLogger<PropertyService>.Instance);
        ownerRepo.CreateAsync(new Owner { Name = "Owner", Address = "Addr" }).Wait();
    }

    [Test]
    public async Task ChangePriceAsync_ShouldUpdatePrice()
    {
        var request = new PropertyRequest
        {
            Name = "House",
            Address = "Street",
            Price = 100m,
            CodeInternal = "ABC123",
            Year = 2020,
            IdOwner = 1
        };
        var created = await _service.CreateAsync(request, null);

        var updated = await _service.ChangePriceAsync(created.Id, 150m);

        Assert.That(updated.Price, Is.EqualTo(150m));
        Assert.That(_context.PropertyTraces.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task AddImageAsync_ShouldPersistImage()
    {
        var request = new PropertyRequest
        {
            Name = "House",
            Address = "Street",
            Price = 100m,
            CodeInternal = "XYZ789",
            Year = 2020,
            IdOwner = 1
        };
        var created = await _service.CreateAsync(request, null);

        var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        var file = new FormFile(stream, 0, stream.Length, "file", "test.jpg");

        var image = await _service.AddImageAsync(created.Id, file, true);

        Assert.That(image.Id, Is.GreaterThan(0));
        Assert.That(_context.PropertyImages.Count(), Is.EqualTo(1));
        Assert.That(_context.PropertyTraces.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task CreateAsync_WithImage_ShouldPersistTraceAndImage()
    {
        var request = new PropertyRequest
        {
            Name = "House2",
            Address = "Street",
            Price = 200m,
            CodeInternal = "LMN456",
            Year = 2021,
            IdOwner = 1,
            Image = "http://img"
        };

        var created = await _service.CreateAsync(request, null);

        Assert.That(created.Id, Is.GreaterThan(0));
        Assert.That(_context.PropertyTraces.Count(), Is.EqualTo(1));
        Assert.That(_context.PropertyImages.Count(), Is.EqualTo(1));
        var img = _context.PropertyImages.Single();
        Assert.That(img.PropertyId, Is.EqualTo(created.Id));
        Assert.That(img.ImageUrl, Is.EqualTo("http://img"));
    }

    [Test]
    public async Task GetImagesAsync_ShouldReturnImages()
    {
        var request = new PropertyRequest
        {
            Name = "House3",
            Address = "Street",
            Price = 300m,
            CodeInternal = "IMG123",
            Year = 2022,
            IdOwner = 1,
            Image = "http://img"
        };

        var created = await _service.CreateAsync(request, null);
        var images = await _service.GetImagesAsync(created.Id);

        Assert.That(images.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task GetTracesAsync_ShouldReturnTraces()
    {
        var request = new PropertyRequest
        {
            Name = "House4",
            Address = "Street",
            Price = 400m,
            CodeInternal = "TRC123",
            Year = 2022,
            IdOwner = 1
        };

        var created = await _service.CreateAsync(request, null);
        await _service.ChangePriceAsync(created.Id, 450m);

        var traces = await _service.GetTracesAsync(created.Id);

        Assert.That(traces.Count(), Is.EqualTo(2));
    }
}
