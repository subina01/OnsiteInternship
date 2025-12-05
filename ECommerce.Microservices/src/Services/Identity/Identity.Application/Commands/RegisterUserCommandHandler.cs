using Identity.Domain.Entities;
using Identity.Infrastructure.Repositories;
using Identity.Infrastructure.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Commands;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Check if user already exists
        if (await _userRepository.ExistsAsync(request.Email, cancellationToken))
            throw new InvalidOperationException($"User with email {request.Email} already exists");

        // Hash password
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        // Create user
        var user = new ApplicationUser(
            request.Email,
            request.UserName,
            passwordHash,
            request.FirstName,
            request.LastName,
            request.PhoneNumber
        );

        // If this is the first user ever, make them admin
        // Otherwise, assign default Customer role
        string roleToAssign = RoleNames.Customer;

        if (!await _userRepository.AnyUsersExistAsync(cancellationToken))
        {
            roleToAssign = RoleNames.Administrator;
        }

        // Ensure the role exists, create it if it doesn't
        var role = await _roleRepository.GetByNameAsync(roleToAssign, cancellationToken);
        if (role == null)
        {
            // Create the role if it doesn't exist
            role = new Role(roleToAssign,
                roleToAssign == RoleNames.Administrator ? "Full system access" :
                roleToAssign == RoleNames.Customer ? "Regular customer access" :
                "Management access");

            // Save the role to the database
            await _roleRepository.AddAsync(role, cancellationToken);
        }

        // Assign the role to the user
        user.AddRole(role);

        await _userRepository.AddAsync(user, cancellationToken);

        return user.Id;
    }
}
