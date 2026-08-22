namespace Template.Modules.Sample.Features.GetSample;

public sealed class Handler
{
    public Response Handle()
    {
        return new Response("Sample module is working");
    }
}