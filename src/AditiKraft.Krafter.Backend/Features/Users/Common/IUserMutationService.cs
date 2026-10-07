using AditiKraft.Krafter.Contracts.Common.Models;
using AditiKraft.Krafter.Contracts.Contracts.Users;

namespace AditiKraft.Krafter.Backend.Features.Users.Common;

public interface IUserMutationService
{
    public Task<Response> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken);

    public Task<Response> UpdateAsync(string id, CreateUserRequest request, CancellationToken cancellationToken);

    public Task<Response> UpdateEmailAsync(string id, string email, CancellationToken cancellationToken);
}
