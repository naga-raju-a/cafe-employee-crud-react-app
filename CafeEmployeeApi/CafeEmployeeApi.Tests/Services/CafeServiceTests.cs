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

        /// <summary>
        /// This method runs before each test to ensure a clean, isolated environment.
        /// It initializes a mock of the ICafeRepository and creates an instance of
        /// CafeService with the mock object injected.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            // We create a mock of the repository to control its behavior during tests.
            _mockCafeRepository = new Mock<ICafeRepository>();
            // We instantiate the service we are testing, injecting the mock repository.
            _cafeService = new CafeService(_mockCafeRepository.Object);
        }

        /// <summary>
        /// A helper method to generate a consistent list of test cafes with varying
        /// locations and employee counts for use across multiple tests.
        /// </summary>
        /// <returns>A list of sample Cafe objects.</returns>
        private static List<Cafe> GetTestCafes()
        {
            return new List<Cafe>
            {
                new Cafe { Id = Guid.NewGuid(), Name = "Cafe A", Location = "Downtown", Employees = new List<Employee> { new(), new() } }, // 2 employees
                new Cafe { Id = Guid.NewGuid(), Name = "Cafe B", Location = "Chinatown", Employees = new List<Employee> { new() } }, // 1 employee
                new Cafe { Id = Guid.NewGuid(), Name = "Cafe C", Location = "Downtown", Employees = new List<Employee> { new(), new(), new() } } // 3 employees
            };
        }

        /// <summary>
        /// Verifies that when no location filter is provided, the service returns all cafes,
        /// and that the list is correctly sorted by the number of employees in descending order.
        /// </summary>
        [Test]
        public async Task GetCafesAsync_WhenNoLocationProvided_ReturnsAllCafesSortedByEmployeeCount()
        {
            // Arrange
            // Get our standard list of test cafes.
            var testCafes = GetTestCafes();
            // Configure the mock repository to return this list when GetAllAsync is called with a null filter.
            _mockCafeRepository.Setup(repo => repo.GetAllAsync(null)).ReturnsAsync(testCafes);

            // Act
            // Call the service method under test.
            var result = (await _cafeService.GetCafesAsync(null)).ToList();

            // Assert
            // Ensure the result is not null and contains all cafes.
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(3));
            // Verify the sorting is correct (Cafe C with 3 employees should be first).
            Assert.That(result[0].Name, Is.EqualTo("Cafe C"));
            Assert.That(result[0].EmployeesCount, Is.EqualTo(3));
            Assert.That(result[1].Name, Is.EqualTo("Cafe A"));
            Assert.That(result[1].EmployeesCount, Is.EqualTo(2));
            Assert.That(result[2].Name, Is.EqualTo("Cafe B"));
            Assert.That(result[2].EmployeesCount, Is.EqualTo(1));
        }

        /// <summary>
        /// Verifies that when a location filter is provided, the service returns only the cafes
        /// from that specific location, also sorted by employee count.
        /// </summary>
        [Test]
        public async Task GetCafesAsync_WhenLocationIsProvided_ReturnsFilteredAndSortedCafes()
        {
            // Arrange
            // Get our test data and filter it to simulate what the repository would return.
            var testCafes = GetTestCafes();
            var downtownCafes = testCafes.Where(c => c.Location == "Downtown").ToList();
            // Configure the mock repository to return the filtered list when called with "Downtown".
            _mockCafeRepository.Setup(repo => repo.GetAllAsync("Downtown")).ReturnsAsync(downtownCafes);

            // Act
            // Call the service method with the location filter.
            var result = (await _cafeService.GetCafesAsync("Downtown")).ToList();

            // Assert
            // Ensure we only get the 2 cafes from "Downtown".
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            // Verify that all returned cafes are indeed from "Downtown".
            Assert.That(result.All(c => c.Location == "Downtown"), Is.True);
            // Verify the sorting within the filtered list (Cafe C should still be first).
            Assert.That(result[0].Name, Is.EqualTo("Cafe C"));
        }

        /// <summary>
        /// Tests the successful creation of a new cafe. It verifies that the repository's
        /// Add and Save methods are called correctly.
        /// </summary>
        [Test]
        public async Task CreateCafeAsync_WhenCalled_AddsCafeAndSaves()
        {
            // Arrange
            // Define the input data for the new cafe.
            var newCafeDto = new CreateOrUpdateCafeDto("New Cafe", "Description", null, "New Location");
            var cafe = new Cafe { Id = Guid.NewGuid(), Name = newCafeDto.Name };
            // Configure the mock repository to return a cafe object when AddAsync is called.
            _mockCafeRepository.Setup(repo => repo.AddAsync(It.IsAny<Cafe>())).ReturnsAsync(cafe);

            // Act
            // Call the creation method.
            var result = await _cafeService.CreateCafeAsync(newCafeDto);

            // Assert
            // Check that the returned DTO contains the correct data.
            Assert.That(result.CafeDto, Is.Not.Null);
            Assert.That(result.CafeDto.Name, Is.EqualTo(newCafeDto.Name));
            // Verify that the repository's AddAsync and SaveChangesAsync methods were called exactly once.
            _mockCafeRepository.Verify(repo => repo.AddAsync(It.Is<Cafe>(c => c.Name == newCafeDto.Name)), Times.Once);
            _mockCafeRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        /// <summary>
        /// Tests the successful update of an existing cafe. It verifies that the service correctly
        /// finds, updates, and saves the cafe.
        /// </summary>
        [Test]
        public async Task UpdateCafeAsync_WithValidId_UpdatesCafeAndSaves()
        {
            // Arrange
            // Define an existing cafe and the DTO with the updated information.
            var cafeId = Guid.NewGuid();
            var existingCafe = new Cafe { Id = cafeId, Name = "Old Name", Location = "Old Location", Employees = new List<Employee>() };
            var updateDto = new CreateOrUpdateCafeDto("New Name", "New Desc", null, "New Location");
            // Configure the mock to return the existing cafe when searched by ID.
            _mockCafeRepository.Setup(repo => repo.GetByIdAsync(cafeId)).ReturnsAsync(existingCafe);

            // Act
            // Call the update method.
            var result = await _cafeService.UpdateCafeAsync(cafeId, updateDto);

            // Assert
            // Check that the returned DTO reflects the updated data.
            Assert.That(result.CafeDto, Is.Not.Null);
            Assert.That(result.CafeDto.Name, Is.EqualTo("New Name"));
            // Verify that the repository's Get, Update, and Save methods were called correctly.
            _mockCafeRepository.Verify(repo => repo.GetByIdAsync(cafeId), Times.Once);
            _mockCafeRepository.Verify(repo => repo.Update(It.Is<Cafe>(c => c.Name == "New Name")), Times.Once);
            _mockCafeRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        /// <summary>
        /// Tests the case where an update is attempted on a non-existent cafe.
        /// Verifies that the service returns a null result and does not attempt to update.
        /// </summary>
        [Test]
        public async Task UpdateCafeAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            // Use an ID that the mock repository won't recognize.
            var invalidId = Guid.NewGuid();
            var updateDto = new CreateOrUpdateCafeDto("Name", "Desc", null, "Location");
            // Configure the mock to return null, simulating that the cafe was not found.
            _mockCafeRepository.Setup(repo => repo.GetByIdAsync(invalidId)).ReturnsAsync((Cafe)null);

            // Act
            // Call the update method.
            var result = await _cafeService.UpdateCafeAsync(invalidId, updateDto);

            // Assert
            // The result should be null because the cafe was not found.
            Assert.That(result.CafeDto, Is.Null);
            // CRITICAL: Verify that the Update method was never called.
            _mockCafeRepository.Verify(repo => repo.Update(It.IsAny<Cafe>()), Times.Never);
        }

        /// <summary>
        /// Tests the successful deletion of a cafe when a valid ID is provided.
        /// Verifies that the method returns true and calls the correct repository methods.
        /// </summary>
        [Test]
        public async Task DeleteCafeAsync_WithValidId_DeletesCafeAndReturnsTrue()
        {
            // Arrange
            var cafeId = Guid.NewGuid();
            var cafeToDelete = new Cafe { Id = cafeId };
            // Configure mocks to find the cafe and simulate a successful save operation.
            _mockCafeRepository.Setup(repo => repo.GetByIdAsync(cafeId)).ReturnsAsync(cafeToDelete);
            _mockCafeRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

            // Act
            // Call the delete method.
            var result = await _cafeService.DeleteCafeAsync(cafeId);

            // Assert
            // The operation should return true on success.
            Assert.That(result, Is.True);
            // Verify that the repository's Delete and Save methods were called.
            _mockCafeRepository.Verify(repo => repo.Delete(cafeToDelete), Times.Once);
            _mockCafeRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        /// <summary>
        /// Tests the case where a delete is attempted for a cafe ID that does not exist.
        /// Verifies that the method returns false and does not attempt a delete operation.
        /// </summary>
        [Test]
        public async Task DeleteCafeAsync_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            var invalidId = Guid.NewGuid();
            // Configure the mock to return null, simulating that the cafe was not found.
            _mockCafeRepository.Setup(repo => repo.GetByIdAsync(invalidId)).ReturnsAsync((Cafe)null);

            // Act
            // Call the delete method.
            var result = await _cafeService.DeleteCafeAsync(invalidId);

            // Assert
            // The result should be false as no cafe was found.
            Assert.That(result, Is.False);
            // Verify that the Delete method was never called.
            _mockCafeRepository.Verify(repo => repo.Delete(It.IsAny<Cafe>()), Times.Never);
        }
    }
}