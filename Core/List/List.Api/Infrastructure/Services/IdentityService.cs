namespace RecAll.Core.List.Api.Infrastructure.Services;

public class IdentityService : IIdentityService {
    private IHttpContextAccessor _context;

    public IdentityService(IHttpContextAccessor context) {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public string GetUserIdentityGuid() =>
        _context.HttpContext.User.FindFirst("sub").Value;
}