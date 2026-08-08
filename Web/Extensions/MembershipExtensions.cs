using Domain.Dto;
using Domain.Models;
using Web.Request;
using Web.Response;

namespace Web.Extensions;

public static class MembershipExtensions
{
    public static MembershipResponse? ToResponse(this Membership membership)
    {
        return new MembershipResponse(
            membership.Id,
            membership.StartDate,
            membership.EndDate,
            membership.Price,
            membership.IsActive,
            membership.Type,
            membership.Gym?.Name,
            membership.Member?.User?.FirstName
        );
    }

    public static List<MembershipResponse?> ToResponse(this List<Membership> membership)
    {
        return membership.Select(x => x.ToResponse()).ToList();
    }

    public static MembershipDto ToDto(this MembershipRequest membership)
    {
        return new MembershipDto
        {
            StartDate = membership.StartDate,
            EndDate = membership.EndDate,
            Price = membership.Price,
            IsActive = membership.IsActive,
            Type = membership.Type,
            GymId = membership.GymId,
            MemberId = membership.MemberId
        };
    }
}