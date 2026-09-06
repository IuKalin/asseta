using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Asseta.Api.Controllers;
using Asseta.Api.Models;
using Asseta.Application.Features.TrustedPeople.Commands.UpdateScopedPermissions;
using Asseta.Application.Features.TrustedPeople.DTOs;
using Asseta.Application.Features.TrustedPeople.Queries.GetMyDelegatedRoles;
using Asseta.Domain.Entities;
using Asseta.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Asseta.IntegrationTests;

public class TrustedPeopleApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public TrustedPeopleApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateTrustedPerson_Returns201_WithPairingCode()
    {
        var ownerId = Guid.NewGuid();
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/trusted-people")
        {
            Content = JsonContent.Create(new CreateTrustedPersonRequest(
                "Luật sư Trần D",
                "trand@example.com",
                "0909998877",
                "Luật sư",
                2,
                "Phụ trách pháp lý"))
        };
        request.Headers.Add("X-Owner-Id", ownerId.ToString());

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<TrustedPersonDto>>(JsonOpts);

        Assert.NotNull(envelope);
        Assert.True(envelope.Success);
        Assert.NotNull(envelope.Data);
        Assert.Equal("Luật sư Trần D", envelope.Data.FullName);
        Assert.Equal("Invited", envelope.Data.Status);
        Assert.NotNull(envelope.Data.ActivePairingCode);
        Assert.Equal(6, envelope.Data.ActivePairingCode.Length);
    }

    [Fact]
    public async Task CreateTrustedPerson_DuplicateContact_Returns409Conflict()
    {
        var ownerId = Guid.NewGuid();
        var clientReq1 = new HttpRequestMessage(HttpMethod.Post, "/api/v1/trusted-people")
        {
            Content = JsonContent.Create(new CreateTrustedPersonRequest("A1", "dup@example.com", "0901111111", "Bạn", 1))
        };
        clientReq1.Headers.Add("X-Owner-Id", ownerId.ToString());
        await _client.SendAsync(clientReq1);

        var clientReq2 = new HttpRequestMessage(HttpMethod.Post, "/api/v1/trusted-people")
        {
            Content = JsonContent.Create(new CreateTrustedPersonRequest("A2", "dup@example.com", "0902222222", "Em", 1))
        };
        clientReq2.Headers.Add("X-Owner-Id", ownerId.ToString());
        var response2 = await _client.SendAsync(clientReq2);

        Assert.Equal(HttpStatusCode.Conflict, response2.StatusCode);
        var envelope = await response2.Content.ReadFromJsonAsync<ApiResponse<object>>(JsonOpts);
        Assert.NotNull(envelope);
        Assert.False(envelope.Success);
        Assert.Equal("DUPLICATE_TRUSTED_PERSON_CONTACT", envelope.Error?.Code);
    }

    [Fact]
    public async Task ClaimPairingCode_And_GetMyDelegatedRoles_Flow()
    {
        var ownerId = Guid.NewGuid();
        var delegateUserId = Guid.NewGuid();

        // 1. Owner tạo Người Ủy Thác
        var createReq = new HttpRequestMessage(HttpMethod.Post, "/api/v1/trusted-people")
        {
            Content = JsonContent.Create(new CreateTrustedPersonRequest(
                "Cộng sự Hoàng E",
                "hoange@example.com",
                "0933334444",
                "Đồng sáng lập",
                2,
                "Quản lý kỹ thuật và vận hành"))
        };
        createReq.Headers.Add("X-Owner-Id", ownerId.ToString());
        var createRes = await _client.SendAsync(createReq);
        var createdDto = (await createRes.Content.ReadFromJsonAsync<ApiResponse<TrustedPersonDto>>(JsonOpts))!.Data!;

        // 2. Delegate nhập mã Pairing Code
        var claimReq = new HttpRequestMessage(HttpMethod.Post, "/api/v1/trusted-people/pairing/claim")
        {
            Content = JsonContent.Create(new ClaimPairingCodeRequest(createdDto.ActivePairingCode!))
        };
        claimReq.Headers.Add("X-Owner-Id", delegateUserId.ToString());
        var claimRes = await _client.SendAsync(claimReq);

        Assert.Equal(HttpStatusCode.OK, claimRes.StatusCode);
        var claimEnvelope = await claimRes.Content.ReadFromJsonAsync<ApiResponse<ClaimPairingResultDto>>(JsonOpts);
        Assert.NotNull(claimEnvelope);
        Assert.True(claimEnvelope.Success);
        Assert.Equal("Active", claimEnvelope.Data?.Status);

        // 3. Delegate truy vấn các vai trò của mình (Zero-Disclosure view)
        var rolesReq = new HttpRequestMessage(HttpMethod.Get, "/api/v1/trusted-people/my-delegated-roles");
        rolesReq.Headers.Add("X-Owner-Id", delegateUserId.ToString());
        var rolesRes = await _client.SendAsync(rolesReq);

        Assert.Equal(HttpStatusCode.OK, rolesRes.StatusCode);
        var rolesEnvelope = await rolesRes.Content.ReadFromJsonAsync<ApiResponse<List<DelegatedRoleDto>>>(JsonOpts);
        Assert.NotNull(rolesEnvelope);
        Assert.True(rolesEnvelope.Success);
        Assert.Single(rolesEnvelope.Data!);
        Assert.Equal("Quản lý kỹ thuật và vận hành", rolesEnvelope.Data![0].RoleDescription);
    }

    [Fact]
    public async Task UpdateScopedPermissions_Returns200_WithUpdatedPermissions()
    {
        var ownerId = Guid.NewGuid();

        var createReq = new HttpRequestMessage(HttpMethod.Post, "/api/v1/trusted-people")
        {
            Content = JsonContent.Create(new CreateTrustedPersonRequest("CFO Nam", "nam@example.com", "0944445555", "CFO", 2))
        };
        createReq.Headers.Add("X-Owner-Id", ownerId.ToString());
        var createRes = await _client.SendAsync(createReq);
        var personDto = (await createRes.Content.ReadFromJsonAsync<ApiResponse<TrustedPersonDto>>(JsonOpts))!.Data!;

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AssetaDbContext>();
        var category = db.ContinuityCategories.First();

        var permReq = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/trusted-people/{personDto.Id}/permissions")
        {
            Content = JsonContent.Create(new UpdateScopedPermissionsRequest(
                CategoryPermissions: new List<CategoryPermissionInput> { new(category.Id, true) }))
        };
        permReq.Headers.Add("X-Owner-Id", ownerId.ToString());
        var permRes = await _client.SendAsync(permReq);

        Assert.Equal(HttpStatusCode.OK, permRes.StatusCode);
        var permEnvelope = await permRes.Content.ReadFromJsonAsync<ApiResponse<List<ScopedPermissionDto>>>(JsonOpts);
        Assert.NotNull(permEnvelope);
        Assert.True(permEnvelope.Success);
        Assert.Single(permEnvelope.Data!);
        Assert.Equal(category.Id, permEnvelope.Data![0].TargetCategoryId);
    }

    [Fact]
    public async Task RevokeTrustedPerson_Returns200_AndUnassignsFromList()
    {
        var ownerId = Guid.NewGuid();

        var createReq = new HttpRequestMessage(HttpMethod.Post, "/api/v1/trusted-people")
        {
            Content = JsonContent.Create(new CreateTrustedPersonRequest("Tạm dừng", "tam@example.com", "0955556666", "Bạn", 1))
        };
        createReq.Headers.Add("X-Owner-Id", ownerId.ToString());
        var createRes = await _client.SendAsync(createReq);
        var personDto = (await createRes.Content.ReadFromJsonAsync<ApiResponse<TrustedPersonDto>>(JsonOpts))!.Data!;

        var deleteReq = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/trusted-people/{personDto.Id}");
        deleteReq.Headers.Add("X-Owner-Id", ownerId.ToString());
        var deleteRes = await _client.SendAsync(deleteReq);

        Assert.Equal(HttpStatusCode.OK, deleteRes.StatusCode);

        // Kiểm tra danh sách không còn người này
        var getReq = new HttpRequestMessage(HttpMethod.Get, "/api/v1/trusted-people");
        getReq.Headers.Add("X-Owner-Id", ownerId.ToString());
        var getRes = await _client.SendAsync(getReq);
        var listEnvelope = await getRes.Content.ReadFromJsonAsync<ApiResponse<List<TrustedPersonDto>>>(JsonOpts);

        Assert.NotNull(listEnvelope);
        Assert.DoesNotContain(listEnvelope.Data!, p => p.Id == personDto.Id);
    }
}
