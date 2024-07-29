using Main.Domain.AppBase;
using Main.Persistence.Exceptions;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Text;

namespace Main.Persistence.PersistenceBase.AdoNet;

public class BaseRepository<TEntity, TKey> : IBaseRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>, new()
{
    public string ConnectionString { get; }

    public string TableName { get; }

    public List<string> GlobalFilters { get; set; }

    public bool IsIdIdentity { get; }

    public BaseRepository(IConfiguration configuration)
    {
        TableName = QueryBuilderUtils.GetTableNameFromType(typeof(TEntity));
        GlobalFilters = [];

        ConnectionString = ConnectionStringProvider.GetMainConnectionString(configuration);
        IsIdIdentity = QueryBuilderUtils.IsIdIdentityForType(typeof(TEntity));
    }

    protected SqlConnection CreateConnection()
    {
        return new SqlConnection(ConnectionString);
    }

    protected SqlTransaction BeginTransaction(SqlConnection connection)
    {
        if (connection.State != System.Data.ConnectionState.Open)
        {
            connection.Open();
        }
        return connection.BeginTransaction();
    }

    protected void CommitTransaction(SqlTransaction transaction)
    {
        transaction?.Commit();
    }

    protected void RollbackTransaction(SqlTransaction transaction)
    {
        try
        {
            transaction?.Rollback();
        }
        catch (Exception rollbackEx)
        {
            throw new DatabaseException(rollbackEx);
        }
    }

    public int BulkDelete(string filter = "")
    {
        var builder = new StringBuilder();

        builder.Append(@$"delete from {TableName}");

        if (filter != "")
            builder.Append($@"where {filter}");

        var query = builder.ToString();
        var affectedRows = 0;

        using (SqlConnection connection = CreateConnection())
        {
            connection.Open();

            using (SqlTransaction transaction = BeginTransaction(connection))
            {
                using (SqlCommand command = new SqlCommand(query, connection, transaction))
                {
                    try
                    {
                        affectedRows = command.ExecuteNonQuery();
                        CommitTransaction(transaction);
                    }
                    catch (Exception ex)
                    {
                        RollbackTransaction(transaction);

                        throw new DatabaseException(ex);
                    }
                }
            }
        }

        return affectedRows;
    }

    public int BulkInsert(List<TEntity> entities)
    {
        if (entities == null || entities.Count == 0)
            return 0;

        var query = QueryBuilderUtils.GenerateBulkInsertQuery(typeof(TEntity), TableName, entities.Count);

        var sqlParameters = new List<SqlParameter>();

        var properties = typeof(TEntity).GetProperties();

        if (QueryBuilderUtils.IsIdIdentityForType(typeof(TEntity)))
            properties = QueryBuilderUtils.RemoveKeyFromPropertiesArray(typeof(TEntity), properties);

        int index = 0, propIndex = 0;

        foreach (var item in entities)
        {
            var parameterNames = QueryBuilderUtils.GetParameterNamesFromEntity(typeof(TEntity));

            foreach (var property in properties)
            {
                object value = property.GetValue(item, null);

                var sqlParameter = new SqlParameter(@$"@{parameterNames[propIndex]}_{index}", value) { SqlDbType = QueryBuilderUtils.GetSqlDbTypeFromProperty(property) };

                sqlParameters.Add(sqlParameter);
                propIndex++;
            }

            index++;
            propIndex = 0;
        }

        var affectedRows = 0;

        using (SqlConnection connection = CreateConnection())
        {
            connection.Open();

            using (SqlTransaction transaction = BeginTransaction(connection))
            {
                using (SqlCommand command = new SqlCommand(query, connection, transaction))
                {
                    try
                    {
                        command.Parameters.AddRange(sqlParameters.ToArray());
                        affectedRows = command.ExecuteNonQuery();
                        CommitTransaction(transaction);
                    }
                    catch (Exception ex)
                    {
                        RollbackTransaction(transaction);

                        throw new DatabaseException(ex);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        return affectedRows;
    }

    public int BulkUpdate(List<TEntity> entities)
    {
        if (entities == null || entities.Count == 0)
            return 0;

        var query = QueryBuilderUtils.GenerateBulkUpdateQuery(typeof(TEntity), TableName, entities.Count);

        var sqlParameters = new List<SqlParameter>();

        var properties = typeof(TEntity).GetProperties();

        int index = 0, propIndex = 0;

        foreach (var item in entities)
        {
            var parameterNames = QueryBuilderUtils.GetParameterNamesFromEntity(typeof(TEntity));

            foreach (var property in properties)
            {
                object value = property.GetValue(item, null);

                var sqlParameter = new SqlParameter(@$"@{parameterNames[propIndex]}_{index}", value) { SqlDbType = QueryBuilderUtils.GetSqlDbTypeFromProperty(property) };

                sqlParameters.Add(sqlParameter);
                propIndex++;
            }

            index++;
            propIndex = 0;
        }

        var affectedRows = 0;

        using (SqlConnection connection = CreateConnection())
        {
            connection.Open();

            using (SqlTransaction transaction = BeginTransaction(connection))
            {
                using (SqlCommand command = new SqlCommand(query, connection, transaction))
                {
                    try
                    {
                        command.Parameters.AddRange(sqlParameters.ToArray());
                        affectedRows = command.ExecuteNonQuery();
                        CommitTransaction(transaction);
                    }
                    catch (Exception ex)
                    {
                        RollbackTransaction(transaction);

                        throw new DatabaseException(ex);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        return affectedRows;
    }

    public void Delete(TEntity entity)
    {
        var query = QueryBuilderUtils.GenerateDeleteQuery(entity.GetType(), TableName);

        using (SqlConnection connection = CreateConnection())
        {
            connection.Open();

            using (SqlTransaction transaction = BeginTransaction(connection))
            {
                using (SqlCommand command = new SqlCommand(query, connection, transaction))
                {
                    try
                    {
                        command.ExecuteNonQuery();
                        CommitTransaction(transaction);
                    }
                    catch (Exception ex)
                    {
                        RollbackTransaction(transaction);

                        throw new DatabaseException(ex);
                    }
                }
            }
        }
    }

    public List<TEntity> GetAll(string filter = "")
    {
        var query = QueryBuilderUtils.GenerateGetAllQuery(filter, TableName);

        var result = new List<TEntity>();

        using (SqlConnection connection = CreateConnection())
        {
            connection.Open();

            using (SqlTransaction transaction = BeginTransaction(connection))
            {
                using (SqlCommand command = new SqlCommand(query, connection, transaction))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(QueryBuilderUtils.GetDataFromReader<TEntity>(reader));
                        }
                    }
                }
            }
        }

        return result;
    }

    public TEntity GetBy(string filter)
    {
        var query = QueryBuilderUtils.GenerateGetByQuery(filter, TableName);

        TEntity result = null;

        using (SqlConnection connection = CreateConnection())
        {
            connection.Open();

            using (SqlTransaction transaction = BeginTransaction(connection))
            {
                using (SqlCommand command = new SqlCommand(query, connection, transaction))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result = QueryBuilderUtils.GetDataFromReader<TEntity>(reader);
                        }
                    }
                }
            }
        }

        return result;
    }

    public TEntity Insert(TEntity entity)
    {
        var query = QueryBuilderUtils.GenerateInsertQuery(entity.GetType(), TableName);

        var properties = typeof(TEntity).GetProperties();

        if (QueryBuilderUtils.IsIdIdentityForType(typeof(TEntity)))
            properties = QueryBuilderUtils.RemoveKeyFromPropertiesArray(typeof(TEntity), properties);

        int propIndex = 0;

        var parameterNames = QueryBuilderUtils.GetParameterNamesFromEntity(typeof(TEntity));
        
        var sqlParameters = new List<SqlParameter>();

        foreach (var property in properties)
        {
            object value = property.GetValue(entity, null);

            var sqlParameter = new SqlParameter(@$"@{parameterNames[propIndex]}", value) { SqlDbType = QueryBuilderUtils.GetSqlDbTypeFromProperty(property) };

            sqlParameters.Add(sqlParameter);
            propIndex++;
        }

        using (SqlConnection connection = CreateConnection())
        {
            connection.Open();

            using (SqlTransaction transaction = BeginTransaction(connection))
            {
                using (SqlCommand command = new SqlCommand(query, connection, transaction))
                {
                    try
                    {
                        command.Parameters.AddRange(sqlParameters.ToArray());
                        entity.Id = (TKey)command.ExecuteScalar();
                        CommitTransaction(transaction);
                    }
                    catch (Exception ex)
                    {
                        RollbackTransaction(transaction);

                        throw new DatabaseException(ex);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        return entity;
    }

    public TEntity Update(TEntity entity)
    {
        var query = QueryBuilderUtils.GenerateUpdateQuery(entity.GetType(), TableName);

        var properties = typeof(TEntity).GetProperties();

        int propIndex = 0;

        var parameterNames = QueryBuilderUtils.GetParameterNamesFromEntity(typeof(TEntity));

        var sqlParameters = new List<SqlParameter>();

        foreach (var property in properties)
        {
            object value = property.GetValue(entity, null);

            var sqlParameter = new SqlParameter(@$"@{parameterNames[propIndex]}", value) { SqlDbType = QueryBuilderUtils.GetSqlDbTypeFromProperty(property) };

            sqlParameters.Add(sqlParameter);
            propIndex++;
        }

        using (SqlConnection connection = CreateConnection())
        {
            connection.Open();

            using (SqlTransaction transaction = BeginTransaction(connection))
            {
                using (SqlCommand command = new SqlCommand(query, connection, transaction))
                {
                    try
                    {
                        command.Parameters.AddRange(sqlParameters.ToArray());
                        entity.Id = (TKey)command.ExecuteScalar();
                        CommitTransaction(transaction);
                    }
                    catch (Exception ex)
                    {
                        RollbackTransaction(transaction);

                        throw new DatabaseException(ex);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        return entity;
    }
}
