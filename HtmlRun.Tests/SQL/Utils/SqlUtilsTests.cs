using HtmlRun.SQL.NHibernate.Constants;
using HtmlRun.SQL.NHibernate.Utils;

public class SqlUtilsTests
{
  private const string SelectForTest = "SELECT A, B, C FROM Abecedary";

  private const string SelectDistinctForTest = "SELECT DISTINCT A FROM Abecedary";

  [Fact]
  public void SqlUtils_SelectLimitRows_ShouldCoverAllEngines()
  {
    var fields = TypeUtilities.GetAllPublicConstantValues<string?>(typeof(DatabaseEngines));
  
    Assert.NotEmpty(fields);
  
    foreach (string? field in fields)
    {
      Assert.NotEmpty(SqlUtils.SelectLimitRows(field!, "SELECT 1", 1));
    }
  }

  [Fact]
  public void SqlUtils_SelectLimitRows_SQLServer()
  {
    Assert.Equal("SELECT TOP 1 A, B, C FROM Abecedary", SqlUtils.SelectLimitRows(DatabaseEngines.SQLServer, SelectForTest, 1));
  }

  [Fact]
  public void SqlUtils_SelectLimitRows_SQLServer_Distinct()
  {
    Assert.Equal("SELECT DISTINCT TOP 1 A FROM Abecedary", SqlUtils.SelectLimitRows(DatabaseEngines.SQLServer, SelectDistinctForTest, 1));
  }

  [Fact]
  public void SqlUtils_SelectLimitRows_SQLite()
  {
    Assert.Equal("SELECT A, B, C FROM Abecedary LIMIT 1", SqlUtils.SelectLimitRows(DatabaseEngines.SQLite, SelectForTest, 1));
  }

  [Fact]
  public void SqlUtils_SelectLimitRows_MySQL()
  {
    Assert.Equal("SELECT A, B, C FROM Abecedary LIMIT 1", SqlUtils.SelectLimitRows(DatabaseEngines.MySQL, SelectForTest, 1));
  }

  [Fact]
  public void SqlUtils_SelectLimitRows_Oracle()
  {
    Assert.Equal("SELECT * FROM (SELECT A, B, C FROM Abecedary) WHERE ROWNUM<=1", SqlUtils.SelectLimitRows(DatabaseEngines.Oracle, SelectForTest, 1));
  }

  [Fact]
  public void SqlUtils_SqlCast_Bool()
  {
    Assert.True((bool)SqlUtils.SqlCast("1", "BIT"));
    Assert.True((bool)SqlUtils.SqlCast("true", "BOOL"));
    Assert.True((bool)SqlUtils.SqlCast("TRUE", "BOOLEAN"));
    Assert.False((bool)SqlUtils.SqlCast("0", "BIT"));
    Assert.False((bool)SqlUtils.SqlCast("false", "BOOL"));
    Assert.False((bool)SqlUtils.SqlCast("FALSE", "BOOLEAN"));
  }

  [Fact]
  public void SqlUtils_SqlCast_Short()
  {
    Assert.Equal((short)1, SqlUtils.SqlCast("1", "SHORT"));
    Assert.Equal((short)1, SqlUtils.SqlCast("1", "SMALLINT"));
    Assert.Equal((short)1, SqlUtils.SqlCast("1", "TINYINT"));
  }

  [Fact]
  public void SqlUtils_SqlCast_Long()
  {
    Assert.Equal(1L, SqlUtils.SqlCast("1", "LONG"));
    Assert.Equal(1L, SqlUtils.SqlCast("1", "BIGINT"));
  }

  [Fact]
  public void SqlUtils_SqlCast_Int()
  {
    Assert.Equal(1, SqlUtils.SqlCast("1", "NUMBER"));
    Assert.Equal(1, SqlUtils.SqlCast("1", "INT"));
    Assert.Equal(1, SqlUtils.SqlCast("1", "INTEGER"));
  }

  [Fact]
  public void SqlUtils_SqlCast_Decimal()
  {
    Assert.Equal(1m, SqlUtils.SqlCast("1", "FLOAT"));
    Assert.Equal(1m, SqlUtils.SqlCast("1", "DOUBLE"));
    Assert.Equal(1m, SqlUtils.SqlCast("1", "DECIMAL"));
    Assert.Equal(1m, SqlUtils.SqlCast("1", "REAL"));
  }

  [Fact]
  public void SqlUtils_SqlCast_Text()
  {
    Assert.Equal("1", SqlUtils.SqlCast("1", "CHAR"));
    Assert.Equal("1", SqlUtils.SqlCast("1", "VARCHAR"));
    Assert.Equal("1", SqlUtils.SqlCast("1", "VARCHAR(1)"));
    Assert.Equal("1", SqlUtils.SqlCast("1", "TEXT"));
    Assert.Equal("1", SqlUtils.SqlCast("1", "NVARCHAR"));
    Assert.Equal("1", SqlUtils.SqlCast("1", "NCHAR"));
    Assert.Equal("1", SqlUtils.SqlCast("1", "NTEXT"));
  }
}