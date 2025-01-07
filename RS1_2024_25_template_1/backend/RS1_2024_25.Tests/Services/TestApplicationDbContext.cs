﻿using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.SharedTables;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul1_Auth;
using RS1_2024_25.API.Endpoints.DataSeedEndpoints;

public static class TestApplicationDbContext
{
    public static async Task<ApplicationDbContext> CreateAsync()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;

        var httpContextAccessor = TestHttpContextAccessorHelper.CreateWithAuthToken();

        var dbContext = new ApplicationDbContext(options, httpContextAccessor);

        // Pokretanje seeder-a
        var seeder = new DataSeedGenerateEndpoint(dbContext);
        await seeder.HandleAsync();

        // Dodavanje testnog tokena
        var user = await dbContext.MyAppUsersAll.FirstAsync();

        var token = new MyAuthenticationToken
        {
            MyAppUser = user,
            Value = TestHttpContextAccessorHelper.tokenValue,
            RecordedAt = DateTime.Now,
            TenantId = user.TenantId
        };

        await dbContext.MyAuthenticationTokensAll.AddAsync(token);
        await dbContext.SaveChangesAsync();

        return dbContext;
    }


}
