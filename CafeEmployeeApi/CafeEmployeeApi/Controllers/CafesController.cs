using CafeEmployeeApi.DTOs;
using CafeEmployeeApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CafeEmployeeApi.Controllers
{

    /// <summary>
    /// API endpoints for managing cafes.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class CafesController : ControllerBase
    {
        private readonly ICafeService _cafeService;

        public CafesController(ICafeService cafeService)
        {
            _cafeService = cafeService;
        }

        /// <summary>
        /// Gets a list of cafes, optionally filtered by location.
        /// </summary>
        /// <param name="location">The location to filter by (e.g., "Downtown").</param>
        /// <returns>A list of cafes sorted by the number of employees.</returns>
        /// <response code="200">Returns the list of cafes.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CafeDto>>> GetCafes([FromQuery] string? location)
        {
            var cafes = await _cafeService.GetCafesAsync(location);
            return Ok(cafes);
        }

        /// <summary>
        /// Creates a new cafe.
        /// </summary>
        /// <param name="cafeDto">The data for the new cafe.</param>
        /// <returns>The newly created cafe.</returns>
        /// <response code="201">Returns the newly created cafe.</response>
        /// <response code="400">If the request body is invalid.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CafeDto>> PostCafe(CreateOrUpdateCafeDto cafeDto)
        {
            var (newCafe, error) = await _cafeService.CreateCafeAsync(cafeDto);
            if (error != null)
            {
                return BadRequest(new { message = error });
            }
            // Return a 201 Created status with a location header pointing to the new resource.
            return CreatedAtAction(nameof(GetCafes), new { id = newCafe.Id }, newCafe);
        }

        /// <summary>
        /// Updates an existing cafe.
        /// </summary>
        /// <param name="id">The unique ID of the cafe to update.</param>
        /// <param name="cafeDto">The updated data for the cafe.</param>
        /// <response code="204">If the update was successful.</response>
        /// <response code="404">If the cafe with the specified ID was not found.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutCafe(Guid id, CreateOrUpdateCafeDto cafeDto)
        {
            var (updatedCafe, error) = await _cafeService.UpdateCafeAsync(id, cafeDto);
            if (error != null)
            {
                // Differentiate between a "not found" error and other validation errors.
                return error.Contains("not found") ? NotFound(new { message = error }) : BadRequest(new { message = error });
            }
            return Ok(updatedCafe);
        }

        /// <summary>
        /// Deletes a cafe and all its associated employees.
        /// </summary>
        /// <param name="id">The unique ID of the cafe to delete.</param>
        /// <response code="204">If the deletion was successful.</response>
        /// <response code="404">If the cafe with the specified ID was not found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCafe(Guid id)
        {
            var success = await _cafeService.DeleteCafeAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
