using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;
using UserManagementAPI.Dtos;
using UserManagementAPI.Repositories;

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _repo;
        private readonly ILogger<UsersController> _logger;
        private readonly InMemoryUserRepository _inMemoryRepo;

        public UsersController(IUserRepository repo, ILogger<UsersController> logger)
        {
            _repo = repo;
            _logger = logger;
            _inMemoryRepo = repo as InMemoryUserRepository ?? throw new ArgumentException("Repository must be InMemoryUserRepository");
        }

        // GET /api/users
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
        [ProducesResponseType(500)]
        [Produces("application/json")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            var users = await _repo.GetAllAsync();
            return Ok(users.Select(MapToDto));
        }

        // GET /api/users/{id}
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(404)]
        [Produces("application/json")]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(MapToDto(user));
        }

        // POST /api/users
        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="dto">User data</param>
        /// <returns>Created user</returns>
        [HttpPost]
        [ProducesResponseType(typeof(UserDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [Produces("application/json")]
        public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);
            if (_inMemoryRepo.EmailExists(dto.Email))
                return Conflict(new { message = "Email already exists." });
            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Department = dto.Department,
                IsActive = dto.IsActive
            };
            var created = await _repo.CreateAsync(user);
            _logger.LogInformation("User created: {Email}", created.Email);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToDto(created));
        }

        // PUT /api/users/{id}
        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="id">User id</param>
        /// <param name="dto">User data</param>
        [HttpPut("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [Produces("application/json")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound();
            if (_inMemoryRepo.EmailExists(dto.Email, id))
                return Conflict(new { message = "Email already exists." });
            existing.FirstName = dto.FirstName;
            existing.LastName = dto.LastName;
            existing.Email = dto.Email;
            existing.Department = dto.Department;
            existing.IsActive = dto.IsActive;
            var updated = await _repo.UpdateAsync(existing);
            if (!updated) return NotFound();
            _logger.LogInformation("User updated: {Email}", existing.Email);
            return NoContent();
        }

        // DELETE /api/users/{id}
        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="id">User id</param>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [Produces("application/json")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repo.DeleteAsync(id);
            if (!deleted) return NotFound();
            _logger.LogInformation("User deleted: {Id}", id);
            return NoContent();
        }

        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Department = user.Department,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
