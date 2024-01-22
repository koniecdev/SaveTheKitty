using Carter;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Riok.Mapperly.Abstractions;
using SaveTheKitty.API.Contracts;
using SaveTheKitty.API.Entities.Common.Enums;
using SaveTheKitty.API.Entities.Common.Helpers;
using SaveTheKitty.API.Entities.Users;
using System.Linq.Expressions;

namespace SaveTheKitty.API.Features.Users;

public static class GetUsers
{
    public sealed class Query(string? sortBy, string? sortOrder, int? page, int? size) : IRequest<PagedList<UserResponse>>, IQuery
    {
        public string? SortBy { get; init; } = sortBy;
        public string? SortOrder { get; init; } = sortOrder;
        public int? Page { get; init; } = page;
        public int? Size { get; init; } = size;
        public (FiltersMethods method, string value)? FilterFirstName { get; set; } = null;
        public (FiltersMethods method, string value)? FilterLastName { get; set; } = null;
        public (FiltersMethods method, string value)? FilterEmail { get; set; } = null;
        public (FiltersMethods method, string value)? FilterUsername { get; set; } = null;
        public (FiltersMethods method, string value)? FilterPhone { get; set; } = null;
    }
    internal sealed class Handler(IMainDbContext _db) : IRequestHandler<Query, PagedList<UserResponse>>
    {
        public async Task<PagedList<UserResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            IQueryable<ApplicationUser> query = _db.ApplicationUsers.AsNoTracking();

            query = HandleFiltering(request, query);

            query = request.SortOrder?.ToLower() == "desc" 
                ? query.OrderByDescending(GetSortProperty(request))
                : query.OrderBy(GetSortProperty(request));

            IQueryable<UserResponse> responseQuery = query
                .ProjectToDto();

            PagedList<UserResponse> results = await PagedList<UserResponse>.CreateAsync(responseQuery, request.Page, request.Size, cancellationToken);
            return results;
        }

        private static Expression<Func<ApplicationUser, object>> GetSortProperty(Query request) => request.SortBy?.ToLower() switch
        {
            "firstname" => m => m.FirstName,
            "lastname" => m => m.LastName,
            "email" => m => m.Email!,
            "username" => m => m.UserName!,
            _ => m => m.Email!
        };

        private static IQueryable<ApplicationUser> HandleFiltering(Query request, IQueryable<ApplicationUser> query)
        {
            if (request.FilterFirstName is not null)
            {
                query = request.FilterFirstName.Value.method switch
                {
                    FiltersMethods.filter_in => query.Where(m => m.FirstName.Contains(request.FilterFirstName.Value.value)),
                    _ => query.Where(m => m.FirstName.Equals(request.FilterFirstName.Value.value))
                };
            }
            if (request.FilterLastName is not null)
            {
                query = request.FilterLastName.Value.method switch
                {
                    FiltersMethods.filter_in => query.Where(m => m.LastName.Contains(request.FilterLastName.Value.value)),
                    _ => query.Where(m => m.LastName.Equals(request.FilterLastName.Value.value))
                };
            }
            if (request.FilterEmail is not null)
            {
                query = request.FilterEmail.Value.method switch
                {
                    FiltersMethods.filter_in => query.Where(m => m.Email!.Contains(request.FilterEmail.Value.value)),
                    _ => query.Where(m => m.Email!.Equals(request.FilterEmail.Value.value))
                };
            }
            if (request.FilterUsername is not null)
            {
                query = request.FilterUsername.Value.method switch
                {
                    FiltersMethods.filter_in => query.Where(m => m.UserName!.Contains(request.FilterUsername.Value.value)),
                    _ => query.Where(m => m.UserName!.Equals(request.FilterUsername.Value.value))
                };
            }
            if (request.FilterPhone is not null)
            {
                query = request.FilterPhone.Value.method switch
                {
                    FiltersMethods.filter_in => query.Where(m => m.PhoneNumber!.Contains(request.FilterPhone.Value.value)),
                    _ => query.Where(m => m.PhoneNumber!.Equals(request.FilterPhone.Value.value))
                };
            }
            return query;
        }
    }

}
public sealed class UserResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string Username { get; init; }
    public string? Phone { get; init; } = null;
}
public class GetUsersEndpoint : CarterModule
{
    public GetUsersEndpoint() : base("/application-users")
    {
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (ISender sender,
            string? sortProperty = null, string? sortOrder = null, string? filters = null, int? page = null, int? size = null) =>
        {
            GetUsers.Query query = new(sortProperty, sortOrder, page, size);
            if (!string.IsNullOrWhiteSpace(filters))
            {
                string[] propsToFilter = filters.Split('|');
                if(propsToFilter.Length > 0)
                {
                    query.FilterFirstName = FilteringHelper.MapFilter(propsToFilter, "firstname");
                    query.FilterLastName= FilteringHelper.MapFilter(propsToFilter, "lastname");
                    query.FilterEmail = FilteringHelper.MapFilter(propsToFilter, "email");
                    query.FilterUsername = FilteringHelper.MapFilter(propsToFilter, "username");
                    query.FilterPhone = FilteringHelper.MapFilter(propsToFilter, "phone");
                }
            }
            PagedList<UserResponse> result = await sender.Send(query);
            return TypedResults.Ok(result);
        });
    }
}

[Mapper]
public static partial class GetUsersMapper
{
    public static partial IQueryable<UserResponse> ProjectToDto(this IQueryable<ApplicationUser> q);

    [MapProperty(nameof(ApplicationUser.UserName), nameof(UserResponse.Username))]
    [MapProperty(nameof(ApplicationUser.PhoneNumber), nameof(UserResponse.Phone))]
    private static partial UserResponse Map(ApplicationUser x);
}