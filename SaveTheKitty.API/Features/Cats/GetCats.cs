using Carter;
using FluentValidation;
using MediatR;
using SaveTheKitty.API.Contracts;
using SaveTheKitty.API.Contracts.Responses;
using SaveTheKitty.API.Entities.Cats;
using SaveTheKitty.API.Entities.Common;
using SaveTheKitty.API.Entities.Common.Helpers;
using System.Linq.Expressions;

namespace SaveTheKitty.API.Features.Users;

public static class GetCats
{
    public sealed class Query(string? sortBy, string? sortOrder, int? page, int? size) : IRequest<PagedList<CatResponse>>, ICollectionQuery
    {
        public string? SortBy { get; init; } = sortBy;
        public string? SortOrder { get; init; } = sortOrder;
        public int? Page { get; init; } = page;
        public int? Size { get; init; } = size;
        public (FiltersMethods method, string value)? FilterName { get; set; } = null;
        public (FiltersMethods method, string value)? FilterHasTemporaryShelter { get; set; } = null;
        public (FiltersMethods method, string value)? FilterHasHealthBook { get; set; } = null;
        public (FiltersMethods method, string value)? FilterRequireMedicalHelp { get; set; } = null;
        public (FiltersMethods method, string value)? FilterIsMale { get; set; } = null;
        public (FiltersMethods method, string value)? FilterAge { get; set; } = null;
        public (FiltersMethods method, string value)? FilterAge_Bottom { get; set; } = null;
        public (FiltersMethods method, string value)? FilterAge_Top { get; set; } = null;
        public (FiltersMethods method, string value)? FilterApplicationUserId { get; set; } = null;
    }
    internal sealed class Handler(IMainDbContext _db) : IRequestHandler<Query, PagedList<CatResponse>>
    {
        public async Task<PagedList<CatResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            IQueryable<Cat> query = _db.Cats.AsNoTracking();

            query = HandleFiltering(request, query);

            query = request.SortOrder?.ToLower() == "desc" 
                ? query.OrderByDescending(GetSortProperty(request))
                : query.OrderBy(GetSortProperty(request));

            IQueryable<CatResponse> responseQuery = query
                .ProjectToDto();

            PagedList<CatResponse> results = await PagedList<CatResponse>.CreateAsync(responseQuery, request.Page, request.Size, cancellationToken);
            return results;
        }

        private static Expression<Func<Cat, object>> GetSortProperty(Query request) => request.SortBy?.ToLower() switch
        {
            "name" => m => m.Name,
            "hashealthbook" => m => m.HasHealthBook,
            "requiremedicalhelp" => m => m.RequireMedicalHelp,
            "ismale" => m => m.IsMale!,
            "age" => m => m.Age!,
            _ => m => m.Name
        };

        private static IQueryable<Cat> HandleFiltering(Query request, IQueryable<Cat> query)
        {
            if (request.FilterName is not null)
            {
                query = request.FilterName.Value.method switch
                {
                    FiltersMethods.filter_in => query.Where(m => m.Name.Contains(request.FilterName.Value.value)),
                    _ => query.Where(m => m.Name.Equals(request.FilterName.Value.value))
                };
            }
            if (request.FilterHasHealthBook is not null)
            {
                query = request.FilterHasHealthBook.Value.method switch
                {
                    _ => query.Where(m => m.HasHealthBook == Convert.ToBoolean(request.FilterHasHealthBook.Value.value))
                };
            }
            if (request.FilterRequireMedicalHelp is not null)
            {
                query = request.FilterRequireMedicalHelp.Value.method switch
                {
                    _ => query.Where(m => m.RequireMedicalHelp == Convert.ToBoolean(request.FilterRequireMedicalHelp.Value.value))
                };
            }
            if (request.FilterIsMale is not null)
            {
                query = request.FilterIsMale.Value.method switch
                {
                    _ => query.Where(m => m.IsMale == Convert.ToBoolean(request.FilterIsMale.Value.value))
                };
            }
            if (request.FilterAge is not null)
            {
                query = request.FilterAge.Value.method switch
                {
                    _ => query.Where(m => m.Age == Convert.ToInt32(request.FilterAge.Value.value))
                };
            }
            else
            {
                if (request.FilterAge_Bottom is not null)
                {
                    query = request.FilterAge_Bottom.Value.method switch
                    {
                        _ => query.Where(m => m.Age >= Convert.ToInt32(request.FilterAge_Bottom.Value.value))
                    };
                }
                if (request.FilterAge_Top is not null)
                {
                    query = request.FilterAge_Top.Value.method switch
                    {
                        _ => query.Where(m => m.Age <= Convert.ToInt32(request.FilterAge_Top.Value.value))
                    };
                }
            }
            if (request.FilterApplicationUserId is not null)
            {
                query = request.FilterApplicationUserId.Value.method switch
                {
                    _ => query.Where(m => m.ApplicationUserId == Guid.Parse(request.FilterApplicationUserId.Value.value))
                };
            }
            return query;
        }
    }
}
public class GetCatsEndpoint : CarterModule
{
    public GetCatsEndpoint() : base("/cats")
    {
        //RequireAuthorization();
        WithTags("Cats");
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (ISender sender,
            string? sortProperty = null, string? sortOrder = null, string? filters = null, int? page = null, int? size = null) =>
        {
            GetCats.Query query = new(sortProperty, sortOrder, page, size);
            if (!string.IsNullOrWhiteSpace(filters))
            {
                string[] propsToFilter = filters.Split('|');
                if(propsToFilter.Length > 0)
                {
                    query.FilterName = FilteringHelper.MapFilter(propsToFilter, "Name");
                    query.FilterHasTemporaryShelter = FilteringHelper.MapFilter(propsToFilter, "FilterHasTemporaryShelter");
                    query.FilterHasHealthBook = FilteringHelper.MapFilter(propsToFilter, "HasHealthBook");
                    query.FilterRequireMedicalHelp = FilteringHelper.MapFilter(propsToFilter, "RequireMedicalHelp");
                    query.FilterIsMale = FilteringHelper.MapFilter(propsToFilter, "IsMale");
                    query.FilterApplicationUserId = FilteringHelper.MapFilter(propsToFilter, "ApplicationUserId");
                    query.FilterAge = FilteringHelper.MapFilter(propsToFilter, "Age", FiltersMethods.filter_eq);
                    query.FilterAge_Bottom = FilteringHelper.MapFilter(propsToFilter, "Age", FiltersMethods.filter_gte);
                    query.FilterAge_Top = FilteringHelper.MapFilter(propsToFilter, "Age", FiltersMethods.filter_lte);
                }
            }
            PagedList<CatResponse> result = await sender.Send(query);
            return TypedResults.Ok(result);
        });
    }
}