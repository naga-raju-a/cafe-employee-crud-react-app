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

        [SetUp]
        public void Setup()
        {
            _mockEmployeeRepository = new Mock<IEmployeeRepository>();
            _mockCafeRepository = new Mock<ICafeRepository>();
            _employeeService = new EmployeeService(_mockEmployeeRepository.Object, _mockCafeRepository.Object);
        }

        private List<Employee> GetTestEmployees()
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

        [Test]
        public async Task GetEmployeesAsync_WhenNoCafeNameProvide_ReturnsAllEmployeesSortedByDaysWorked()
        {
            // Arrange
            var testEmployees = GetTestEmployees();
            _mockEmployeeRepository.Setup(repo => repo.GetAllAsync(null)).ReturnsAsync(testEmployees);

            // Act
            var result = (await _employeeService.GetEmployeesAsync(null)).ToList();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(3));
            // Check for correct sorting and DaysWorked calculation
            Assert.That(result[0].Name, Is.EqualTo("Senior Staff"));
            Assert.That(result[0].DaysWorked, Is.EqualTo(100));
            Assert.That(result[1].Name, Is.EqualTo("Worker Bee"));
            Assert.That(result[1].DaysWorked, Is.EqualTo(10));
            Assert.That(result[2].Name, Is.EqualTo("New Hire"));
            Assert.That(result[2].DaysWorked, Is.EqualTo(0));
        }

        [Test]
        public async Task GetEmployeesAsync_WhenCalled_ReturnsAllEmployeesSortedByDaysWorked()
        {
            // Arrange
            var testEmployees = GetTestEmployees();

            var cafeAemployees = testEmployees
                .Where(c => c.Cafe != null && c.Cafe.Name == "Cafe A")
                .ToList();
            _mockEmployeeRepository.Setup(repo => repo.GetAllAsync("Cafe A")).ReturnsAsync(cafeAemployees);           

            // Act
            var result = (await _employeeService.GetEmployeesAsync("Cafe A")).ToList();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            // Check for correct sorting and DaysWorked calculation
            Assert.That(result[0].Name, Is.EqualTo("Senior Staff"));
            Assert.That(result[0].DaysWorked, Is.EqualTo(100));
            Assert.That(result[1].Name, Is.EqualTo("Worker Bee"));
            Assert.That(result[1].DaysWorked, Is.EqualTo(10));
            
        }

        [Test]
        public async Task CreateEmployeeAsync_WithNewEmail_CreatesEmployee()
        {
            // Arrange
            var cafeId = Guid.NewGuid();
            var newEmployeeDto = new CreateOrUpdateEmployeeDto("John Wick", "john.wick@test.com", "91112222", "Male", cafeId, DateTime.UtcNow.AddDays(-50));

            // Setup mock to indicate email is not in use
            _mockEmployeeRepository.Setup(repo => repo.IsEmailInUse(newEmployeeDto.EmailAddress)).ReturnsAsync(false);
            _mockEmployeeRepository.Setup(repo => repo.AddAsync(It.IsAny<Employee>())).ReturnsAsync(new Employee()); // Return a dummy employee
            _mockCafeRepository.Setup(repo => repo.GetByIdAsync(cafeId)).ReturnsAsync(new Cafe { Name = "The Continental" });

            // Act
            var result = await _employeeService.CreateEmployeeAsync(newEmployeeDto);          

            // Assert
            Assert.That(result.EmployeeDto, Is.Not.Null);
            Assert.That(result.EmployeeDto.Name, Is.EqualTo(newEmployeeDto.Name));
            // Check that the ID was generated and starts with "UI"
            Assert.That(result.EmployeeDto.Id.StartsWith("UI"), Is.True);

            // Verify correct methods were called
            _mockEmployeeRepository.Verify(repo => repo.IsEmailInUse(newEmployeeDto.EmailAddress), Times.Once);
            _mockEmployeeRepository.Verify(repo => repo.AddAsync(It.IsAny<Employee>()), Times.Once);
            _mockEmployeeRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task CreateEmployeeAsync_WithExistingEmail_ReturnsNull()
        {
            // Arrange
            var newEmployeeDto = new CreateOrUpdateEmployeeDto("Imposter", "existing.email@test.com", "87654321", "Female", null, null);

            // Setup mock to indicate email IS in use
            _mockEmployeeRepository.Setup(repo => repo.IsEmailInUse(newEmployeeDto.EmailAddress)).ReturnsAsync(true);

            // Act
            var result = await _employeeService.CreateEmployeeAsync(newEmployeeDto);

            // Assert
            Assert.That(result.EmployeeDto, Is.Null);

            // Verify that the AddAsync method was NEVER called, enforcing the business rule.
            _mockEmployeeRepository.Verify(repo => repo.AddAsync(It.IsAny<Employee>()), Times.Never);
        }

        [Test]
        public async Task UpdateEmployeeAsync_WithValidId_UpdatesEmployeeCafeAssignment()
        {
            // Arrange
            var employeeId = "UI_EXISTING";
            var oldCafeId = Guid.NewGuid();
            var newCafeId = Guid.NewGuid();

            var existingEmployee = new Employee { Id = employeeId, Name = "Old Name", CafeId = oldCafeId, EmploymentDate = DateTime.UtcNow.AddDays(-50) };
            var updateDto = new CreateOrUpdateEmployeeDto("New Name", "email@test.com", "91234567", "Male", newCafeId, DateTime.UtcNow.AddDays(-50));

            _mockEmployeeRepository.Setup(repo => repo.GetByIdAsync(employeeId)).ReturnsAsync(existingEmployee);
            _mockCafeRepository.Setup(repo => repo.GetByIdAsync(newCafeId)).ReturnsAsync(new Cafe { Id = newCafeId, Name = "New Cafe" });

            // Act
            var result = await _employeeService.UpdateEmployeeAsync(employeeId, updateDto);

            // Assert
            Assert.That(result.EmployeeDto, Is.Not.Null);
            Assert.That(result.EmployeeDto.CafeName, Is.EqualTo("New Cafe"));
            Assert.That(result.EmployeeDto.DaysWorked, Is.Not.EqualTo(0));

            _mockEmployeeRepository.Verify(repo => repo.Update(It.Is<Employee>(e => e.CafeId == newCafeId)), Times.Once);
            _mockEmployeeRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task DeleteEmployeeAsync_WithValidId_ReturnsTrue()
        {
            // Arrange
            var employeeId = "UI_TO_DELETE";
            var employeeToDelete = new Employee { Id = employeeId };

            // Setup mocks to simulate finding the employee and a successful save
            _mockEmployeeRepository.Setup(repo => repo.GetByIdAsync(employeeId)).ReturnsAsync(employeeToDelete);
            _mockEmployeeRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

            // Act
            var result = await _employeeService.DeleteEmployeeAsync(employeeId);

            // Assert
            Assert.That(result, Is.True);
            // Verify that the Delete and Save methods were called on the repository
            _mockEmployeeRepository.Verify(repo => repo.Delete(It.Is<Employee>(e => e.Id == employeeId)), Times.Once);
            _mockEmployeeRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task DeleteEmployeeAsync_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            var invalidId = "UI_INVALID";
            // Setup mock to simulate NOT finding the employee
            _mockEmployeeRepository.Setup(repo => repo.GetByIdAsync(invalidId)).ReturnsAsync((Employee)null);

            // Act
            var result = await _employeeService.DeleteEmployeeAsync(invalidId);

            // Assert
            Assert.That(result, Is.False);
            // Verify that the Delete method was NEVER called
            _mockEmployeeRepository.Verify(repo => repo.Delete(It.IsAny<Employee>()), Times.Never);
        }
    }

}

