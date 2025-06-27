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
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace CafeEmployeeApi.Tests.Services
{
    [TestFixture]
    public class EmployeeServiceTests
    {
        private Mock<IEmployeeRepository> _mockEmployeeRepository;
        private Mock<ICafeRepository> _mockCafeRepository; // EmployeeService depends on this too
        private IEmployeeService _employeeService;

        /// <summary>
        /// This method runs before each test. It initializes mock objects and the service instance,
        /// ensuring a clean state for every test case.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _mockEmployeeRepository = new Mock<IEmployeeRepository>();
            _mockCafeRepository = new Mock<ICafeRepository>();
            _employeeService = new EmployeeService(_mockEmployeeRepository.Object, _mockCafeRepository.Object);
        }

        /// <summary>
        /// A helper method to generate a consistent list of test employees with different
        /// employment dates and cafe assignments for use in multiple tests.
        /// </summary>
        /// <returns>A list of sample Employee objects.</returns>
        private static List<Employee> GetTestEmployees()
        {
            var cafeA = new Cafe { Id = Guid.NewGuid(), Name = "Cafe A" };
            return new List<Employee>
            {
                // Worked for 10 days
                new Employee { Id = "UI123", Name = "Worker Bee", EmploymentDate = DateTime.UtcNow.AddDays(-10), Cafe = cafeA },
                // Worked for 100 days
                new Employee { Id = "UI456", Name = "Senior Staff", EmploymentDate = DateTime.UtcNow.AddDays(-100), Cafe = cafeA },
                // Unassigned, 0 days worked
                new Employee { Id = "UI789", Name = "New Hire", EmploymentDate = null, Cafe = null }
            };
        }

        /// <summary>
        /// Verifies that calling GetEmployeesAsync with a null cafe name returns all employees,
        /// correctly sorted by the number of days they have worked in descending order.
        /// </summary>
        [Test]
        public async Task GetEmployeesAsync_WhenNoCafeNameProvide_ReturnsAllEmployeesSortedByDaysWorked()
        {
            // Arrange
            // Get our standard list of test employees.
            var testEmployees = GetTestEmployees();
            // Configure the mock repository to return this list when GetAllAsync is called with a null filter.
            _mockEmployeeRepository.Setup(repo => repo.GetAllAsync(null)).ReturnsAsync(testEmployees);

            // Act
            // Call the service method under test.
            var result = (await _employeeService.GetEmployeesAsync(null)).ToList();

            // Assert
            // Ensure the result is not null and contains all employees.
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(3));
            // Verify the sorting is correct (most days worked first) and the DaysWorked property is calculated correctly.
            Assert.That(result[0].Name, Is.EqualTo("Senior Staff"));
            Assert.That(result[0].DaysWorked, Is.EqualTo(100));
            Assert.That(result[1].Name, Is.EqualTo("Worker Bee"));
            Assert.That(result[1].DaysWorked, Is.EqualTo(10));
            Assert.That(result[2].Name, Is.EqualTo("New Hire"));
            Assert.That(result[2].DaysWorked, Is.EqualTo(0));
        }

        /// <summary>
        /// Verifies that calling GetEmployeesAsync with a specific cafe name returns only the employees
        /// from that cafe, sorted correctly by their tenure.
        /// </summary>
        [Test]
        public async Task GetEmployeesAsync_WhenCalled_ReturnsAllEmployeesSortedByDaysWorked()
        {
            // Arrange
            // Get our test data and then filter it, simulating what the repository would do.
            var testEmployees = GetTestEmployees();
            var cafeAemployees = testEmployees
                .Where(c => c.Cafe != null && c.Cafe.Name == "Cafe A")
                .ToList();
            // Configure the mock repository to return the filtered list when called with "Cafe A".
            _mockEmployeeRepository.Setup(repo => repo.GetAllAsync("Cafe A")).ReturnsAsync(cafeAemployees);

            // Act
            // Call the service method with the cafe name filter.
            var result = (await _employeeService.GetEmployeesAsync("Cafe A")).ToList();

            // Assert
            // Ensure we only get the 2 employees from "Cafe A".
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            // Verify the sorting and DaysWorked calculation for the filtered results.
            Assert.That(result[0].Name, Is.EqualTo("Senior Staff"));
            Assert.That(result[0].DaysWorked, Is.EqualTo(100));
            Assert.That(result[1].Name, Is.EqualTo("Worker Bee"));
            Assert.That(result[1].DaysWorked, Is.EqualTo(10));
        }

        /// <summary>
        /// Tests the happy path for creating an employee. Verifies that a new employee is
        /// created successfully when a unique email address is provided.
        /// </summary>
        [Test]
        public async Task CreateEmployeeAsync_WithNewEmail_CreatesEmployee()
        {
            // Arrange
            // Define the input data for the new employee.
            var cafeId = Guid.NewGuid();
            var newEmployeeDto = new CreateOrUpdateEmployeeDto("John Wick", "john.wick@test.com", "91112222", "Male", cafeId, DateTime.UtcNow.AddDays(-50));
            // Configure mock to indicate the email is not already in use.
            _mockEmployeeRepository.Setup(repo => repo.IsEmailInUse(newEmployeeDto.EmailAddress)).ReturnsAsync(false);
            // Configure mock to simulate adding an employee to the database.
            _mockEmployeeRepository.Setup(repo => repo.AddAsync(It.IsAny<Employee>())).ReturnsAsync(new Employee());
            // Configure mock to find the cafe the employee will be assigned to.
            _mockCafeRepository.Setup(repo => repo.GetByIdAsync(cafeId)).ReturnsAsync(new Cafe { Name = "The Continental" });

            // Act
            // Call the creation method.
            var result = await _employeeService.CreateEmployeeAsync(newEmployeeDto);

            // Assert
            // Verify the returned DTO contains the correct data and a generated ID.
            Assert.That(result.EmployeeDto, Is.Not.Null);
            Assert.That(result.EmployeeDto.Name, Is.EqualTo(newEmployeeDto.Name));
            Assert.That(result.EmployeeDto.Id.StartsWith("UI"), Is.True);
            // Verify that the correct repository methods were called to enforce business logic.
            _mockEmployeeRepository.Verify(repo => repo.IsEmailInUse(newEmployeeDto.EmailAddress), Times.Once);
            _mockEmployeeRepository.Verify(repo => repo.AddAsync(It.IsAny<Employee>()), Times.Once);
            _mockEmployeeRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        /// <summary>
        /// Tests the business rule that prevents creating an employee with a duplicate email address.
        /// Verifies that the method returns a null result and does not attempt to save.
        /// </summary>
        [Test]
        public async Task CreateEmployeeAsync_WithExistingEmail_ReturnsNull()
        {
            // Arrange
            var newEmployeeDto = new CreateOrUpdateEmployeeDto("Imposter", "existing.email@test.com", "87654321", "Female", null, null);
            // Configure mock to indicate the email IS in use.
            _mockEmployeeRepository.Setup(repo => repo.IsEmailInUse(newEmployeeDto.EmailAddress)).ReturnsAsync(true);

            // Act
            // Call the creation method.
            var result = await _employeeService.CreateEmployeeAsync(newEmployeeDto);

            // Assert
            // The primary result should be null as creation failed.
            Assert.That(result.EmployeeDto, Is.Null);
            // CRITICAL: Verify that the AddAsync method was NEVER called, proving the business rule was enforced.
            _mockEmployeeRepository.Verify(repo => repo.AddAsync(It.IsAny<Employee>()), Times.Never);
        }

        /// <summary>
        /// Verifies that an existing employee's details, including their cafe assignment, can be updated successfully.
        /// </summary>
        [Test]
        public async Task UpdateEmployeeAsync_WithValidId_UpdatesEmployeeCafeAssignment()
        {
            // Arrange
            // Define IDs and create the existing employee object and the update DTO.
            var employeeId = "UI_EXISTING";
            var oldCafeId = Guid.NewGuid();
            var newCafeId = Guid.NewGuid();
            var existingEmployee = new Employee { Id = employeeId, Name = "Old Name", CafeId = oldCafeId, EmploymentDate = DateTime.UtcNow.AddDays(-50) };
            var updateDto = new CreateOrUpdateEmployeeDto("New Name", "email@test.com", "91234567", "Male", newCafeId, DateTime.UtcNow.AddDays(-50));
            // Configure mocks to find the existing employee and the new cafe.
            _mockEmployeeRepository.Setup(repo => repo.GetByIdAsync(employeeId)).ReturnsAsync(existingEmployee);
            _mockCafeRepository.Setup(repo => repo.GetByIdAsync(newCafeId)).ReturnsAsync(new Cafe { Id = newCafeId, Name = "New Cafe" });

            // Act
            // Call the update method.
            var result = await _employeeService.UpdateEmployeeAsync(employeeId, updateDto);

            // Assert
            // Check that the returned DTO reflects the updated information.
            Assert.That(result.EmployeeDto, Is.Not.Null);
            Assert.That(result.EmployeeDto.CafeName, Is.EqualTo("New Cafe"));
            Assert.That(result.EmployeeDto.DaysWorked, Is.Not.EqualTo(0));
            // Verify that the Update and SaveChanges methods were called.
            _mockEmployeeRepository.Verify(repo => repo.Update(It.Is<Employee>(e => e.CafeId == newCafeId)), Times.Once);
            _mockEmployeeRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        /// <summary>
        /// Tests the successful deletion of an employee when a valid ID is provided.
        /// Verifies that the method returns true.
        /// </summary>
        [Test]
        public async Task DeleteEmployeeAsync_WithValidId_ReturnsTrue()
        {
            // Arrange
            var employeeId = "UI_TO_DELETE";
            var employeeToDelete = new Employee { Id = employeeId };
            // Configure mocks to simulate finding the employee and a successful save.
            _mockEmployeeRepository.Setup(repo => repo.GetByIdAsync(employeeId)).ReturnsAsync(employeeToDelete);
            _mockEmployeeRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

            // Act
            // Call the delete method.
            var result = await _employeeService.DeleteEmployeeAsync(employeeId);

            // Assert
            // The operation should return true on success.
            Assert.That(result, Is.True);
            // Verify that the repository's Delete and Save methods were called on the correct employee.
            _mockEmployeeRepository.Verify(repo => repo.Delete(It.Is<Employee>(e => e.Id == employeeId)), Times.Once);
            _mockEmployeeRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        /// <summary>
        /// Tests the scenario where a delete is attempted for an employee ID that does not exist.
        /// Verifies that the method returns false and does not attempt a delete operation.
        /// </summary>
        [Test]
        public async Task DeleteEmployeeAsync_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            var invalidId = "UI_INVALID";
            // Configure mock to simulate NOT finding the employee.
            _mockEmployeeRepository.Setup(repo => repo.GetByIdAsync(invalidId)).ReturnsAsync((Employee)null);

            // Act
            // Call the delete method.
            var result = await _employeeService.DeleteEmployeeAsync(invalidId);

            // Assert
            // The operation should return false as no employee was found.
            Assert.That(result, Is.False);
            // Verify that the Delete method was NEVER called.
            _mockEmployeeRepository.Verify(repo => repo.Delete(It.IsAny<Employee>()), Times.Never);
        }
    }
}