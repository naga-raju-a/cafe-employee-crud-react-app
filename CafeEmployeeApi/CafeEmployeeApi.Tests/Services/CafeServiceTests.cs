using NUnit.Framework;
using Moq;
using CafeEmployeeApi.Services;
using CafeEmployeeApi.Repositories;
using CafeEmployeeApi.Models;
using CafeEmployeeApi.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace CafeEmployeeApi.Tests.Services
{
    [TestFixture]
    public class CafeServiceTests
    {
        private Mock<ICafeRepository> _mockCafeRepository;
        private ICafeService _cafeService;

        [SetUp]
        public void Setup()
        {
            // Arrange: This setup runs before each test.
            // We create a mock of the repository to control its behavior.
            _mockCafeRepository = new Mock<ICafeRepository>();

            // We instantiate the service we are testing, injecting the mock repository.
            _cafeService = new CafeService(_mockCafeRepository.Object);
        }

        private List<Cafe> GetTestCafes()
        {
            // Helper method to create a list of cafes for testing.
            return new List<Cafe>
            {
                new Cafe { Id = Guid.NewGuid(), Name = "Cafe A", Location = "Downtown", Employees = new List<Employee> { new(), new() } }, // 2 employees
                new Cafe { Id = Guid.NewGuid(), Name = "Cafe B", Location = "Chinatown", Employees = new List<Employee> { new() } }, // 1 employee
                new Cafe { Id = Guid.NewGuid(), Name = "Cafe C", Location = "Downtown", Employees = new List<Employee> { new(), new(), new() } } // 3 employees
            };
        }

        [Test]
        public async Task GetCafesAsync_WhenNoLocationProvided_ReturnsAllCafesSortedByEmployeeCount()
        {
            // Arrange
            var testCafes = GetTestCafes();
            _mockCafeRepository.Setup(repo => repo.GetAllAsync(null)).ReturnsAsync(testCafes);

            // Act
            var result = (await _cafeService.GetCafesAsync(null)).ToList();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(3));
            // Check for correct sorting (Cafe C with 3 employees should be first)
            Assert.That(result[0].Name, Is.EqualTo("Cafe C"));
            Assert.That(result[0].EmployeesCount, Is.EqualTo(3));
            Assert.That(result[1].Name, Is.EqualTo("Cafe A"));
            Assert.That(result[1].EmployeesCount, Is.EqualTo(2));
            Assert.That(result[2].Name, Is.EqualTo("Cafe B"));
            Assert.That(result[2].EmployeesCount, Is.EqualTo(1));
        }

        [Test]
        public async Task GetCafesAsync_WhenLocationIsProvided_ReturnsFilteredAndSortedCafes()
        {
            // Arrange
            var testCafes = GetTestCafes();
            var downtownCafes = testCafes.Where(c => c.Location == "Downtown").ToList();
            _mockCafeRepository.Setup(repo => repo.GetAllAsync("Downtown")).ReturnsAsync(downtownCafes);

            // Act
            var result = (await _cafeService.GetCafesAsync("Downtown")).ToList();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            // Check that only "Downtown" cafes are returned
            Assert.That(result.All(c => c.Location == "Downtown"), Is.True);
            // Check sorting within the filtered list (Cafe C should be first)
            Assert.That(result[0].Name, Is.EqualTo("Cafe C"));
        }

        [Test]
        public async Task CreateCafeAsync_WhenCalled_AddsCafeAndSaves()
        {
            // Arrange
            var newCafeDto = new CreateOrUpdateCafeDto("New Cafe", "Description", null, "New Location");
            var cafe = new Cafe { Id = Guid.NewGuid(), Name = newCafeDto.Name };

            // When AddAsync is called, we can set up a callback to simulate DB behavior or just ensure it returns the object
            _mockCafeRepository.Setup(repo => repo.AddAsync(It.IsAny<Cafe>())).ReturnsAsync(cafe);

            // Act
            var result = await _cafeService.CreateCafeAsync(newCafeDto);

            // Assert
            Assert.That(result.CafeDto, Is.Not.Null);
            Assert.That(result.CafeDto.Name, Is.EqualTo(newCafeDto.Name));

            // Verify that the repository's AddAsync and SaveChangesAsync methods were called exactly once.
            _mockCafeRepository.Verify(repo => repo.AddAsync(It.Is<Cafe>(c => c.Name == newCafeDto.Name)), Times.Once);
            _mockCafeRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task UpdateCafeAsync_WithValidId_UpdatesCafeAndSaves()
        {
            // Arrange
            var cafeId = Guid.NewGuid();
            var existingCafe = new Cafe { Id = cafeId, Name = "Old Name", Location = "Old Location", Employees = new List<Employee>() };
            var updateDto = new CreateOrUpdateCafeDto("New Name", "New Desc", null, "New Location");

            _mockCafeRepository.Setup(repo => repo.GetByIdAsync(cafeId)).ReturnsAsync(existingCafe);

            // Act
            var result = await _cafeService.UpdateCafeAsync(cafeId, updateDto);

            // Assert
            Assert.That(result.CafeDto, Is.Not.Null);
            Assert.That(result.CafeDto.Name, Is.EqualTo("New Name"));

            // Verify that the repository's Get, Update, and Save methods were called.
            _mockCafeRepository.Verify(repo => repo.GetByIdAsync(cafeId), Times.Once);
            _mockCafeRepository.Verify(repo => repo.Update(It.Is<Cafe>(c => c.Name == "New Name")), Times.Once);
            _mockCafeRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task UpdateCafeAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var invalidId = Guid.NewGuid();
            var updateDto = new CreateOrUpdateCafeDto("Name", "Desc", null, "Location");
            _mockCafeRepository.Setup(repo => repo.GetByIdAsync(invalidId)).ReturnsAsync((Cafe)null);

            // Act
            var result = await _cafeService.UpdateCafeAsync(invalidId, updateDto);

            // Assert
            Assert.That(result.CafeDto, Is.Null);
            _mockCafeRepository.Verify(repo => repo.Update(It.IsAny<Cafe>()), Times.Never); // Ensure Update is never called
        }

        [Test]
        public async Task DeleteCafeAsync_WithValidId_DeletesCafeAndReturnsTrue()
        {
            // Arrange
            var cafeId = Guid.NewGuid();
            var cafeToDelete = new Cafe { Id = cafeId };
            _mockCafeRepository.Setup(repo => repo.GetByIdAsync(cafeId)).ReturnsAsync(cafeToDelete);
            _mockCafeRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true); // Simulate successful save

            // Act
            var result = await _cafeService.DeleteCafeAsync(cafeId);

            // Assert
            Assert.That(result, Is.True);
            _mockCafeRepository.Verify(repo => repo.Delete(cafeToDelete), Times.Once);
            _mockCafeRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task DeleteCafeAsync_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            var invalidId = Guid.NewGuid();
            _mockCafeRepository.Setup(repo => repo.GetByIdAsync(invalidId)).ReturnsAsync((Cafe)null);

            // Act
            var result = await _cafeService.DeleteCafeAsync(invalidId);

            // Assert
            Assert.That(result, Is.False);
            _mockCafeRepository.Verify(repo => repo.Delete(It.IsAny<Cafe>()), Times.Never);
        }
    }
}
