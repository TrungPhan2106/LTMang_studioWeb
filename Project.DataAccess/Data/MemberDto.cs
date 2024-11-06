using AutoMapper;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;

namespace MemberManagement.Data
{
    public class MemberDto
    {
        public int MemberId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool Gender { get; set; }
        public string Address { get; set; }
        public DateTime JoinedDate { get; set; }
    }

    public class MemberConfig
    {
        public static void CreateMap(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<Member, MemberDto>();
            cfg.CreateMap<MemberDto, Member>();
        }
    }
    public class AutoMappingProflie : Profile
    {
        public AutoMappingProflie()
        {
            CreateMap<Member, MemberDto>().ReverseMap();
        }

    }
}
