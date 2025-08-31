using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using TestMillion.Persistence.Context;
using TestMillion.Persistence.Repositories;
using TestMillion.Services.Implementations;
using TestMillion.Services.Models.Request;

namespace TestMillion.Tests;

public class OwnerServiceTests
{
    private AppDbContext _context = null!;
    private OwnerService _service = null!;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        var repository = new OwnerRepository(_context);
        var storage = new FakeFileStorageService();
        _service = new OwnerService(repository, storage, NullLogger<OwnerService>.Instance);
    }

    [Test]
    public async Task CreateAsync_ShouldReturnOwner()
    {
        var request = new OwnerRequest
        {
            Name = "John",
            Address = "123 St",
            Photo = null,
            Birthday = null
        };

        var result = await _service.CreateAsync(request, null);

        Assert.That(result.Id, Is.GreaterThan(0));
        Assert.That(result.Name, Is.EqualTo("John"));
    }
}
