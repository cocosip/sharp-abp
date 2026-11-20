using Microsoft.Extensions.Options;
using Volo.Abp.Auditing;
using Xunit;

namespace SharpAbp.Abp.DbConnectionsManagement;

public class DatabaseConnectionAuditingTest
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Connection_Changes_Audit_Metadata_Without_Connection_Strings(bool create)
    {
        const string connectionString = "Server=example;Password=secret-must-not-be-audited";
        object input = create
            ? new CreateDatabaseConnectionInfoDto { Name = "orders", DatabaseProvider = "SqlServer", ConnectionString = connectionString }
            : new UpdateDatabaseConnectionInfoDto { Name = "orders", DatabaseProvider = "SqlServer", ConnectionString = connectionString };
        var serializer = new JsonAuditSerializer(Options.Create(new AbpAuditingOptions()));

        var parameters = serializer.Serialize(new { input });

        Assert.DoesNotContain("secret-must-not-be-audited", parameters);
        Assert.DoesNotContain("connectionString", parameters);
        Assert.Contains("orders", parameters);
        Assert.Contains("SqlServer", parameters);
    }
}
