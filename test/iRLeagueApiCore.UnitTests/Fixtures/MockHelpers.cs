// From https://github.com/dotnet/aspnetcore/blob/main/src/Identity/test/Shared/MockHelpers.cs

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FluentValidation;
using iRLeagueApiCore.Services.ResultService.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq.Protected;
using System.Linq.Expressions;
using System.Text;

namespace Microsoft.AspNetCore.Identity.Test;

public static class MockHelpers
{
    public static StringBuilder LogMessage = new StringBuilder();

    public static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
    {
        var store = new Mock<IUserStore<TUser>>();
        var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
        mgr.Object.UserValidators.Add(new UserValidator<TUser>());
        mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());
        return mgr;
    }

    public static Mock<RoleManager<TRole>> MockRoleManager<TRole>(IRoleStore<TRole>? store = null) where TRole : class
    {
        store ??= new Mock<IRoleStore<TRole>>().Object;
        var roles = new List<IRoleValidator<TRole>>();
        roles.Add(new RoleValidator<TRole>());
        return new Mock<RoleManager<TRole>>(store, roles, MockLookupNormalizer(),
            new IdentityErrorDescriber(), null);
    }

    public static UserManager<TUser> TestUserManager<TUser>(IUserStore<TUser>? store = null) where TUser : class
    {
        store ??= new Mock<IUserStore<TUser>>().Object;
        var options = new Mock<IOptions<IdentityOptions>>();
        var idOptions = new IdentityOptions();
        idOptions.Lockout.AllowedForNewUsers = false;
        options.Setup(o => o.Value).Returns(idOptions);
        var userValidators = new List<IUserValidator<TUser>>();
        var validator = new Mock<IUserValidator<TUser>>();
        userValidators.Add(validator.Object);
        var pwdValidators = new List<PasswordValidator<TUser>>
            {
                new PasswordValidator<TUser>()
            };
        var userManager = new UserManager<TUser>(store, options.Object, new PasswordHasher<TUser>(),
            userValidators, pwdValidators, MockLookupNormalizer(),
            new IdentityErrorDescriber(), null,
            new Mock<ILogger<UserManager<TUser>>>().Object);
        validator.Setup(v => v.ValidateAsync(userManager, It.IsAny<TUser>()))
            .Returns(Task.FromResult(IdentityResult.Success)).Verifiable();
        return userManager;
    }

    public static IUserStore<T> TestUserStore<T>(List<T>? users = null) where T : IdentityUser
    {
        var userStoreList = users ?? new();
        var normalizer = MockLookupNormalizer();
        userStoreList.ForEach(x => x.NormalizedUserName = normalizer.NormalizeName(x.UserName));
        var store = new Mock<IUserStore<T>>();
        store.Setup(x => x.FindByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string id, CancellationToken cancellationToken) =>
                userStoreList.FirstOrDefault(x => x.Id == id)!);
        store.Setup(x => x.CreateAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((T user, CancellationToken cancellationToken) =>
            {
                var normalizedName = normalizer.NormalizeName(user.UserName);
                if (userStoreList.Any(x => x.UserName == user.UserName || x.NormalizedUserName == normalizedName))
                {
                    return IdentityResult.Failed(new[] { new IdentityErrorDescriber().DuplicateUserName(user.UserName) });
                }
                userStoreList.Add(user);
                return IdentityResult.Success;
            });
        store.Setup(x => x.GetUserNameAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((T User, CancellationToken cancellationToken) => User.UserName);
        store.Setup(x => x.SetNormalizedUserNameAsync(It.IsAny<T>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback((T user, string normalizedName, CancellationToken cancellationToken) =>
            {
                var storeUser = userStoreList.FirstOrDefault(x => x == user);
                if (storeUser is null)
                {
                    return;
                }
                storeUser.NormalizedUserName = normalizedName;
            });
        store.Setup(x => x.UpdateAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
            .Returns(async (T user, CancellationToken cancellationToken) =>
            {
                var storeUserIndex = userStoreList.FindIndex(x => x.Id == user.Id);
                if (storeUserIndex == -1)
                {
                    return await store.Object.CreateAsync(user, cancellationToken);
                }
                userStoreList[storeUserIndex] = user;
                return IdentityResult.Success;
            });
        return store.Object;
    }

    public static IUserRoleStore<TUser> TestUserRoleStore<TUser, TRole>(
        IUserStore<TUser>? userStore = null, 
        IRoleStore<TRole>? roleStore = null, 
        IDictionary<TUser, List<TRole>>? userRoles = null) 
        where TUser : IdentityUser where TRole : IdentityRole
    {
        userStore ??= TestUserStore<TUser>();
        roleStore ??= TestRoleStore<TRole>();
        userRoles ??= new Dictionary<TUser, List<TRole>>();
        return new UserRoleStore<TUser, TRole>(userStore, roleStore, userRoles);
    }

    public static RoleManager<TRole> TestRoleManager<TRole>(IRoleStore<TRole>? store = null) where TRole : class
    {
        store ??= new Mock<IRoleStore<TRole>>().Object;
        var roles = new List<IRoleValidator<TRole>>
            {
                new RoleValidator<TRole>()
            };
        return new RoleManager<TRole>(store, roles,
            MockLookupNormalizer(),
            new IdentityErrorDescriber(),
            null);
    }

    public static IRoleStore<TRole> TestRoleStore<TRole>(IDictionary<string, TRole>? roles = null) where TRole : IdentityRole<string>
    {
        var roleStoreDict = roles ?? new Dictionary<string, TRole>();
        var normalizer = MockLookupNormalizer();
        roleStoreDict.ForEach(x => x.Value.NormalizedName = normalizer.NormalizeName(x.Value.Name));
        var store = new Mock<IRoleStore<TRole>>();
        store.Setup(x => x.FindByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string id, CancellationToken cancellationToken) => roleStoreDict.GetOrDefault(id)!);
        store.Setup(x => x.FindByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string name, CancellationToken cancellationToken) => roleStoreDict.FirstOrDefault(x => x.Value.NormalizedName == name).Value!);
        store.Setup(x => x.CreateAsync(It.IsAny<TRole>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TRole role, CancellationToken cancellationToken) =>
            {
                var errorDescriber = new IdentityErrorDescriber();
                if (roleStoreDict.Any(x => x.Value.Name == role.Name))
                {
                    return IdentityResult.Failed(errorDescriber.DuplicateRoleName(role.Name));
                }
                role.NormalizedName = normalizer.NormalizeName(role.Name);
                roleStoreDict.Add(role.Id, role);
                return IdentityResult.Success;
            });
        store.Setup(x => x.UpdateAsync(It.IsAny<TRole>(), It.IsAny<CancellationToken>()))
            .Returns(async (TRole role, CancellationToken cancellationToken) =>
            {
                if (roleStoreDict.ContainsKey(role.Id) == false)
                {
                    return await store.Object.CreateAsync(role, cancellationToken);
                }
                roleStoreDict[role.Id] = role;
                return IdentityResult.Success;
            });
        store.Setup(x => x.DeleteAsync(It.IsAny<TRole>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TRole role, CancellationToken cancellationToken) =>
            {
                if (roleStoreDict.ContainsKey(role.Id))
                {
                    roleStoreDict.Remove(role.Id);
                }
                return IdentityResult.Success;
            });
        store.Setup(x => x.GetRoleIdAsync(It.IsAny<TRole>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TRole role, CancellationToken cancellationToken) => role.Id);
        store.Setup(x => x.GetRoleNameAsync(It.IsAny<TRole>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TRole role, CancellationToken cancellationToken) => role.Name);
        store.Setup(x => x.GetNormalizedRoleNameAsync(It.IsAny<TRole>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TRole role, CancellationToken cancellationToken) => role.NormalizedName);
        return store.Object;
    }

    public static ILookupNormalizer MockLookupNormalizer()
    {
        var normalizerFunc = new Func<string, string>(i =>
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(i)).ToUpperInvariant();
        });
        var lookupNormalizer = new Mock<ILookupNormalizer>();
        lookupNormalizer.Setup(i => i.NormalizeName(It.IsAny<string>())).Returns(normalizerFunc);
        lookupNormalizer.Setup(i => i.NormalizeEmail(It.IsAny<string>())).Returns(normalizerFunc);
        return lookupNormalizer.Object;
    }

    public static Mock<IValidator<T>> MockValidator<T>()
    {
        return new Mock<IValidator<T>>();
    }

    public static IValidator<T> TestValidator<T>()
    {
        var mockValidator = MockValidator<T>();
        mockValidator.Setup(x => x.Validate(It.IsAny<T>()))
            .Returns(new FluentValidation.Results.ValidationResult()).Verifiable();
        mockValidator.Setup(x => x.ValidateAsync(It.IsAny<T>(), default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult()).Verifiable();
        return mockValidator.Object;
    }

    public static IEnumerable<IValidator<T>> TestValidators<T>()
    {
        return new IValidator<T>[] { TestValidator<T>() };
    }

    /// <summary>
    /// Get a test mediator that returns the given result for specified reuqest type 
    /// </summary>
    /// <typeparam name="TRequest">Request type</typeparam>
    /// <typeparam name="TResult">Result type</typeparam>
    /// <param name="match">Predicate to check if request contains correct information. <para/>Returns true by default</param>
    /// <param name="result">Result that should be returned from <see cref="IMediator.Send(object, System.Threading.CancellationToken)"/></param>
    /// <param name="throws">If set a call to <see cref="IMediator.Send(object, System.Threading.CancellationToken)"/> will throw the provided Exception instead</param>
    /// <returns>Configured <see cref="IMediator"/></returns>
    public static IMediator TestMediator<TRequest, TResult>(Expression<Func<TRequest, bool>>? match = default,
        TResult? result = default, Exception? throws = default)
        where TRequest : IRequest<TResult?>
    {
        match ??= x => true;
        var mockMediator = new Mock<IMediator>();
        mockMediator.Setup(x => x.Send(It.Is(match), default))
            .ReturnsAsync(() =>
            {
                if (throws != null)
                {
                    throw throws;
                }
                return result;
            })
            .Verifiable();
        return mockMediator.Object;
    }

    public static HttpMessageHandler TestMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> result)
    {
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage message, CancellationToken cancellationToken) =>
            {
                return result.Invoke(message);
            });
        return mockHttpMessageHandler.Object;
    }

    public class UserRoleStore<TUser, TRole> : IUserRoleStore<TUser>, IUserStore<TUser>, IRoleStore<TRole> where TUser : IdentityUser where TRole : IdentityRole
    {
        private readonly IUserStore<TUser> userStore;
        private readonly IRoleStore<TRole> roleStore;

        private readonly IDictionary<TUser, List<TRole>> userRolesStore;

        public UserRoleStore(IUserStore<TUser> userStore, IRoleStore<TRole> roleStore, IDictionary<TUser, List<TRole>> userRoles)
        {
            this.userStore = userStore;
            this.roleStore = roleStore;
            userRolesStore = userRoles;
        }

        public async Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            var userRoles = userRolesStore.GetOrDefault(user);
            if (userRoles is null)
            {
                userRoles = new();
                userRolesStore.Add(user, userRoles);
            }
            if (userRoles.Any(x => x.Name == roleName))
            {
                // user already in role
                return;
            }
            var role = await roleStore.FindByNameAsync(roleName, cancellationToken);
            if (role is null)
            {
                // role does not exist
                return;
            }
            userRoles.Add(role);
        }

        public Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            return userStore.CreateAsync(user, cancellationToken);
        }

        public Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            return roleStore.CreateAsync(role, cancellationToken);
        }

        public Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            return userStore.DeleteAsync(user, cancellationToken);
        }

        public Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            return roleStore.DeleteAsync(role, cancellationToken);
        }

        public void Dispose()
        {
            userStore.Dispose();
        }

        public Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return userStore.FindByIdAsync(userId, cancellationToken);
        }

        public Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return userStore.FindByNameAsync(normalizedUserName, cancellationToken);
        }

        public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            return roleStore.GetNormalizedRoleNameAsync(role, cancellationToken);
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            return userStore.GetNormalizedUserNameAsync(user, cancellationToken);
        }

        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
        {
            return roleStore.GetRoleIdAsync(role, cancellationToken);
        }

        public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            return roleStore.GetRoleNameAsync(role, cancellationToken);
        }

        public async Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
        {
            var userRoles = userRolesStore.GetOrDefault(user);
            if (userRoles is null)
            {
                return new List<string>();
            }
            return await Task.FromResult(userRoles.Select(x => x.Name).ToList());
        }

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
        {
            return userStore.GetUserIdAsync(user, cancellationToken);
        }

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            return userStore.GetUserNameAsync(user, cancellationToken);
        }

        public async Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            var role = await roleStore.FindByNameAsync(roleName, cancellationToken);
            if (role is null)
            {
                return new List<TUser>();
            }
            return userRolesStore
                .Where(x => x.Value.Contains(role))
                .Select(x => x.Key)
                .ToList();
        }

        public async Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            var userRoles = userRolesStore.GetOrDefault(user);
            var role = await roleStore.FindByNameAsync(roleName, cancellationToken);
            if (userRoles is null || role is null)
            {
                return false;
            }
            return userRoles.Contains(role);
        }

        public async Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            var userRoles = userRolesStore.GetOrDefault(user);
            var role = await roleStore.FindByNameAsync(roleName, cancellationToken);
            if (userRoles is null || role is null)
            {
                return;
            }
            if (userRoles.Contains(role) == false)
            {
                return;
            }
            userRoles.Remove(role);
        }

        public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
        {
            return roleStore.SetNormalizedRoleNameAsync(role, normalizedName, cancellationToken);
        }

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            return userStore.SetNormalizedUserNameAsync(user, normalizedName, cancellationToken);
        }

        public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            return roleStore.SetRoleNameAsync(role, roleName, cancellationToken);
        }

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            return userStore.SetUserNameAsync(user, userName, cancellationToken);
        }

        public Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            return userStore.UpdateAsync(user, cancellationToken);
        }

        public Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            return roleStore.UpdateAsync(role, cancellationToken);
        }

        Task<TRole> IRoleStore<TRole>.FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            return roleStore.FindByIdAsync(roleId, cancellationToken);
        }

        Task<TRole> IRoleStore<TRole>.FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            return roleStore.FindByNameAsync(normalizedRoleName, cancellationToken);
        }
    }
}
