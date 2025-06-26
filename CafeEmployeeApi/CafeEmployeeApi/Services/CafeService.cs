using CafeEmployeeApi.DTOs;
using CafeEmployeeApi.Models;
using CafeEmployeeApi.Repositories;

namespace CafeEmployeeApi.Services
{
    public class CafeService : ICafeService
    {
        private readonly ICafeRepository _cafeRepository;

        public CafeService(ICafeRepository cafeRepository)
        {
            _cafeRepository = cafeRepository;
        }

        public async Task<IEnumerable<CafeDto>> GetCafesAsync(string? location)
        {
            var cafes = await _cafeRepository.GetAllAsync(location);

            // Map the Cafe entities to CafeDto objects for the API response.
            // This includes counting the number of employees for each cafe.
            // Finally, sort the result by the number of employees in descending order.
            return cafes
                .Select(c => new CafeDto(c.Id, c.Name, c.Description, c.Logo, c.Location, c.Employees.Count))
                .OrderByDescending(c => c.EmployeesCount);
        }

        public async Task<(CafeDto? CafeDto, string? Error)> CreateCafeAsync(CreateOrUpdateCafeDto cafeDto)
        {
            // Map the DTO to a new Cafe entity.
            var cafe = new Cafe
            {
                Name = cafeDto.Name,
                Description = cafeDto.Description,
                Logo = cafeDto.Logo,
                Location = cafeDto.Location
            };

            var newCafe = await _cafeRepository.AddAsync(cafe);
            await _cafeRepository.SaveChangesAsync();

            // Return the newly created cafe as a DTO. Employee count is 0 initially.
            var newcafeDto = new CafeDto(newCafe.Id, newCafe.Name, newCafe.Description, newCafe.Logo, newCafe.Location, 0);
            return (newcafeDto,null);
        }

        public async Task<(CafeDto? CafeDto, string? Error)> UpdateCafeAsync(Guid id, CreateOrUpdateCafeDto cafeDto)
        {
            var existingCafe = await _cafeRepository.GetByIdAsync(id);
            if (existingCafe == null)
            {
                // Cafe not found, return null to indicate this to the controller.
                return (null, "Cafe not found.");
            }

            // Update the properties of the existing cafe from the DTO.
            existingCafe.Name = cafeDto.Name;
            existingCafe.Description = cafeDto.Description;
            existingCafe.Logo = cafeDto.Logo;
            existingCafe.Location = cafeDto.Location;

            _cafeRepository.Update(existingCafe);
            await _cafeRepository.SaveChangesAsync();

            // Return the updated cafe information as a DTO.
            var updatedCafeDto = new CafeDto(existingCafe.Id, existingCafe.Name, existingCafe.Description, existingCafe.Logo, existingCafe.Location, existingCafe.Employees.Count);
            return (updatedCafeDto, null);
        }

        public async Task<bool> DeleteCafeAsync(Guid id)
        {
            var cafe = await _cafeRepository.GetByIdAsync(id);
            if (cafe == null)
            {
                return false; // Cafe not found.
            }

            _cafeRepository.Delete(cafe);
            return await _cafeRepository.SaveChangesAsync();
        }
    }
}
