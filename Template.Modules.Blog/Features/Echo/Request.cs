using System.ComponentModel.DataAnnotations;

namespace Template.Modules.Sample.Features.Echo;

public sealed record Request(
    [Required]
    [MinLength(2)]
    [MaxLength(200)]
    string Message);