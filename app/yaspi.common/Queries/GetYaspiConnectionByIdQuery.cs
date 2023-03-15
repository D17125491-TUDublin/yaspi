using Microsoft.Data.SqlClient;

namespace yaspi.common;

public class GetYaspiConnectionByIdQuery : IQuery<YaspiConnection>
{
    private readonly int _id;
    private readonly string _connectionString;
    public GetYaspiConnectionByIdQuery(int id, string connectionString)
    {
        _id = id;
        _connectionString = connectionString;
    }
    public YaspiConnection Execute()
    {
        var  ret = new YaspiConnection();
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var command = new SqlCommand(@"SELECT * FROM [dbo].[YaspiConnection] c1
                                            left join YaspiConnector c2 on c1.YaspiConnectorId = c2.YaspiConnectorId 
                                        WHERE [YaspiConnectionId] = @id", connection);
            command.Parameters.AddWithValue("@id", _id);
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                
                
                    ret.YaspiConnectionId = (int)reader["YaspiConnectionId"];
                    ret.YaspiConnectorId = (int)reader["YaspiConnectorId"];
                    ret.YaspiConnectionName = (string)reader["YaspiConnectionName"];
                    ret.YaspiConnectorName = (string)reader["YaspiConnectorName"];
                    ret.Username = (string)reader["Username"];
                    ret.IsActive = (bool)reader["IsActive"];
                
            }
            else
            {
                return null;
            }
        }
        GetYaspiConnectionDataByConnectionQueryId query = new GetYaspiConnectionDataByConnectionQueryId(_id, _connectionString);
        ret.ConnectionData = query.Execute();
        return ret;
    }
}