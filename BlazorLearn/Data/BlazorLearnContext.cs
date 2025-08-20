using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Identity
{
    public class BlazorLearnContext(DbContextOptions<BlazorLearnContext> options) : IdentityDbContext<IdentityUser>(options)
    {
    }
}
