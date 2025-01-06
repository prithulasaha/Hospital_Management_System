using E_Commerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HMSphere.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NID { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public bool IsDeleted { get; set; } = false;

        public virtual Doctor? Doctor { get; set; }
		public virtual Patient? Patient { get; set; }
		public virtual Staff? Staff { get; set; }

		[JsonIgnore]
        public List<RefreshToken>? RefreshTokens { get; set; }
    }
}
