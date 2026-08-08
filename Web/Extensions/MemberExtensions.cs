using Domain.Dto;
using Domain.Models;
using Web.Request;
using Web.Response;

namespace Web.Extensions;

public static class MemberExtensions
{
    public static MemberResponse? ToResponse(this Member member)
    {
        return new MemberResponse(
            member.Id,
            member.User?.FirstName,
            member.User?.LastName,
            member.DateOfBirth,
            member.Goal
        );
    }

    public static List<MemberResponse?> ToResponse(this List<Member> e)
    {
        return e.Select(x => x.ToResponse()).ToList();
    }

    public static MemberDto ToDto(this MemberRequest e)
    {
        return new MemberDto
        {
            UserId = e.UserId,
            DateOfBirth = e.DateOfBirth,
            Goal =  e.Goal
        };
    }
}