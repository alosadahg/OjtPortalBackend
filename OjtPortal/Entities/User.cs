using AutoMapper.Configuration.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OjtPortal.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace OjtPortal.Entities
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        [Column(TypeName = "varchar(50)")]
        [ValueConverter(typeof(EnumToStringConverter<UserType>))]
        public UserType UserType { get; set; }
        [Column(TypeName = "varchar(50)")]
        [ValueConverter(typeof(EnumToStringConverter<AccountStatus>))]
        public AccountStatus AccountStatus { get; set; }

        public User() { }

        public User(string firstName, string lastName, UserType userType, AccountStatus accountStatus)
        {
            FirstName = firstName;
            LastName = lastName;
            UserType = userType;
            AccountStatus = accountStatus;
        }
    }
}
