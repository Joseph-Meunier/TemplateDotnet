using Template.Modules.Sample.Data;
using Template.Modules.Sample.Domain;

namespace Template.Modules.Sample.Features.CreateSampleItem;

public sealed class Handler(
    SampleDbContext dbContext)
{
    public async Task<Response> Handle(
        Request request,
        CancellationToken cancellationToken)
    {
        var item = new SampleItem(request.Name);

        dbContext.SampleItems.Add(item);

        await dbContext.SaveChangesAsync(
            cancellationToken);

        return new Response(
            item.Id,
            item.Name);
    }
}