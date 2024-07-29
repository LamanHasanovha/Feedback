using Main.Base.Instance;
using Main.Base.Pluralization;
using Main.Extensions;
using Main.Persistence.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace Main.Persistence.PersistenceBase.AdoNet;

public static class QueryBuilderUtils
{
    public static IPluralizationService PluralizationService { get; set; }

    public static void Initialize(ServiceFactory serviceFactory)
    {
        PluralizationService = serviceFactory.GetPluralizationService();
    }

    public static string GetTableNameFromType(Type type)
    {
        var tableAttribute = type.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() as TableAttribute;

        if (tableAttribute != null)
            return tableAttribute.Name;

        return PluralizationService.Pluralize(type.Name);
    }

    public static string GetColumnNamesForInsert(Type entityType)
    {
        var builder = new StringBuilder();

        var properties = entityType.GetProperties();

        if (IsIdIdentityForType(entityType))
            properties = RemoveKeyFromPropertiesArray(entityType, properties);

        foreach (var property in properties)
        {
            var columnAttribute = property.GetCustomAttribute(typeof(ColumnAttribute), true) as ColumnAttribute;

            if (columnAttribute != null)
            {
                builder.Append(columnAttribute.Name);
            }
            else
            {
                builder.Append(property.Name);
            }

            builder.Append(',');
        }

        return builder.ToString().TrimEnd(',');
    }

    public static List<string> GetParameterNamesFromEntity(Type entityType)
    {
        var properties = entityType.GetProperties();

        if (IsIdIdentityForType(entityType))
            properties = RemoveKeyFromPropertiesArray(entityType, properties);

        var parameterList = new List<string>();

        foreach (var property in properties)
        {
            var columnAttribute = property.GetCustomAttribute(typeof(ColumnAttribute), true) as ColumnAttribute;

            if (columnAttribute != null)
                parameterList.Add(columnAttribute.Name);
            else
                parameterList.Add(property.Name);
        }

        return parameterList;
    }

    public static bool IsIdIdentityForType(Type entityType)
    {
        var identityAttribute = entityType.GetCustomAttributes(typeof(IdIdentityAttribute), true).FirstOrDefault() as IdIdentityAttribute;

        if (identityAttribute == null)
            return false;

        return true;
    }

    public static string GenerateInsertQuery(Type entityType, string tableName, int index = -1)
    {
        return @$"INSERT INTO {tableName} ({GetColumnNamesForInsert(entityType)}) VALUES ({GetParametersForInsert(entityType, index)})";
    }

    public static string GetParametersForInsert(Type entityType, int index)
    {
        var builder = new StringBuilder();

        var properties = entityType.GetProperties();

        if (IsIdIdentityForType(entityType))
            properties = RemoveKeyFromPropertiesArray(entityType, properties);

        var indexValue = index == -1 ? "" : index.ToString();

        foreach (var property in properties)
        {
            var columnAttribute = property.GetCustomAttribute(typeof(ColumnAttribute), true) as ColumnAttribute;

            if (columnAttribute != null)
            {
                builder.Append($@"@{columnAttribute.Name}_{indexValue}");
            }
            else
                builder.Append($@"@{property.Name}_{indexValue}");

            builder.Append(',');
        }

        return builder.ToString().TrimEnd(',');
    }

    public static PropertyInfo[] RemoveKeyFromPropertiesArray(Type entityType, PropertyInfo[] properties)
    {
        PropertyInfo idColumn = null;

        foreach (var property in properties)
        {
            if (property.GetCustomAttribute(typeof(KeyAttribute), true) != null)
            {
                idColumn = property;
                break;
            }
        }

        if (idColumn == null)
            throw new Exception(@$"Couldn't get column names for insert. Identity attribute setted for type {entityType.FullName}, but Key attribute was not presented in the properties of the specified type.");

        return properties.Where(p => idColumn.Name != p.Name).ToArray();
    }

    public static string GenerateBulkInsertQuery(Type entityType, string tableName, int count)
    {
        var builder = new StringBuilder();

        for (int i = 0; i < count; i++)
        {
            builder.AppendLine(GenerateInsertQuery(entityType, tableName, i));
            builder.AppendLine();
        }

        return builder.ToString();
    }

    public static SqlDbType GetSqlDbTypeFromProperty(PropertyInfo propertyInfo)
    {
        Type type = propertyInfo.PropertyType;
        bool isNullable = false;

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            type = Nullable.GetUnderlyingType(type);
            isNullable = true;
        }

        var columnAttribute = propertyInfo.GetCustomAttribute(typeof(ColumnAttribute), true) as ColumnAttribute;
        var sqlTypeName = string.Empty;

        if (columnAttribute != null)
            sqlTypeName = columnAttribute.TypeName;
        else
            sqlTypeName = "nvarchar";

        SqlDbType sqlDbType;

        switch (Type.GetTypeCode(type))
        {
            case TypeCode.Int32:
                sqlDbType = SqlDbType.Int;
                break;
            case TypeCode.Int64:
                sqlDbType = SqlDbType.BigInt;
                break;
            case TypeCode.Int16:
                sqlDbType = SqlDbType.SmallInt;
                break;
            case TypeCode.Byte:
                sqlDbType = SqlDbType.TinyInt;
                break;
            case TypeCode.Decimal:
                if (sqlTypeName.ToLower().Equals("money", StringComparison.OrdinalIgnoreCase))
                    sqlDbType = SqlDbType.Money;
                else
                    sqlDbType = SqlDbType.Decimal;
                break;
            case TypeCode.Single:
                sqlDbType = SqlDbType.Float;
                break;
            case TypeCode.Double:
                sqlDbType = SqlDbType.Float;
                break;
            case TypeCode.Boolean:
                sqlDbType = SqlDbType.Bit;
                break;
            case TypeCode.String:
                if (sqlTypeName.ToLower().Equals("char", StringComparison.OrdinalIgnoreCase))
                    sqlDbType = SqlDbType.Char;
                else if (sqlTypeName.ToLower().Equals("nchar", StringComparison.OrdinalIgnoreCase))
                    sqlDbType = SqlDbType.NChar;
                else if (sqlTypeName.ToLower().Equals("ntext", StringComparison.OrdinalIgnoreCase))
                    sqlDbType = SqlDbType.NText;
                else if (sqlTypeName.ToLower().Equals("text", StringComparison.OrdinalIgnoreCase))
                    sqlDbType = SqlDbType.Text;
                else if (sqlTypeName.ToLower().Equals("varchar", StringComparison.OrdinalIgnoreCase))
                    sqlDbType = SqlDbType.VarChar;
                else
                    sqlDbType = SqlDbType.NVarChar;
                break;
            case TypeCode.DateTime:
                sqlDbType = SqlDbType.DateTime;
                break;
            case TypeCode.Object:
                if (type == typeof(TimeSpan))
                {
                    sqlDbType = SqlDbType.Time;
                }
                else if (type == typeof(byte[]))
                {
                    if (sqlTypeName.ToLower().Equals("image", StringComparison.OrdinalIgnoreCase))
                        sqlDbType = SqlDbType.Image;
                    else if (sqlTypeName.ToLower().Equals("timestamp", StringComparison.OrdinalIgnoreCase))
                        sqlDbType = SqlDbType.Timestamp;
                    else 
                        sqlDbType = SqlDbType.VarBinary;
                }
                else if (type == typeof(Guid))
                {
                    sqlDbType = SqlDbType.UniqueIdentifier;
                }
                else if (type == typeof(DateTimeOffset))
                {
                    sqlDbType = SqlDbType.DateTimeOffset;
                }
                else
                {
                    throw new NotSupportedException($"Type {type.Name} is not supported.");
                }
                break;
            default:
                throw new NotSupportedException($"Type {type.Name} is not supported.");
        }

        return sqlDbType;
    }

    public static string GenerateBulkUpdateQuery(Type entityType, string tableName, int count)
    {
        var builder = new StringBuilder();

        for (int i = 0; i < count; i++)
        {
            builder.AppendLine(GenerateUpdateQuery(entityType, tableName, i));
            builder.AppendLine();
        }

        return builder.ToString();
    }

    public static string GenerateUpdateQuery(Type entityType, string tableName, int index = -1)
    {
        return $@"UPDATE {tableName} SET {GetSetStatementsForUpdateQuery(entityType, index)} WHERE {GetConditionForUpdateQuery(entityType, index)}";
    }

    public static string GetSetStatementsForUpdateQuery(Type entityType, int index)
    {
        var builder = new StringBuilder();

        var properties = RemoveKeyFromPropertiesArray(entityType, entityType.GetProperties());

        var indexValue = index == -1 ? "" : index.ToString();

        foreach (var property in properties)
        {
            var columnAttribute = property.GetCustomAttribute(typeof(ColumnAttribute), true) as ColumnAttribute;

            if (columnAttribute != null)
            {
                builder.Append($@"{columnAttribute.Name} = @{columnAttribute.Name}_{indexValue}");
            }
            else
                builder.Append($@"{property.Name} = @{property.Name}_{indexValue}");

            builder.Append(',');
        }

        return builder.ToString().TrimEnd(',');
    }

    public static string GetConditionForUpdateQuery(Type entityType, int index)
    {
        PropertyInfo idColumn = null;

        var properties = entityType.GetProperties();

        foreach (var property in properties)
        {
            if (property.GetCustomAttribute(typeof(KeyAttribute), true) != null)
            {
                idColumn = property;
                break;
            }
        }

        var indexValue = index == -1 ? "" : index.ToString();

        var builder = new StringBuilder();

        builder.Append(@$"{idColumn.Name} = @{idColumn.Name}_{indexValue}");

        return builder.ToString();
    }

    public static string GenerateDeleteQuery(Type entityType, string tableName)
    {
        var builder = new StringBuilder(@$"delete from {tableName} where ");

        var properties = entityType.GetProperties();

        PropertyInfo idColumn = null;

        foreach (var property in properties)
        {
            if (property.GetCustomAttribute(typeof(KeyAttribute), true) != null)
            {
                idColumn = property;
                break;
            }
        }

        builder.Append(@$"{idColumn.Name} = @{idColumn.Name}_0");

        return builder.ToString();
    }

    public static string GenerateGetByQuery(string filter, string tableName)
    {
        var builder = new StringBuilder(@$"select top 1 * from {tableName} ");

        if (!string.IsNullOrWhiteSpace(filter))
            builder.Append(@$"where {filter}");

        return builder.ToString();
    }

    public static List<string> GetColumnNames(Type entityType)
    {
        var properties = entityType.GetProperties();
        var parameterList = new List<string>();

        foreach (var property in properties)
        {
            var columnAttribute = property.GetCustomAttribute(typeof(ColumnAttribute), true) as ColumnAttribute;

            if (columnAttribute != null)
                parameterList.Add(columnAttribute.Name);
            else
                parameterList.Add(property.Name);
        }

        return parameterList;
    }

    public static T GetDataFromReader<T>(SqlDataReader reader)
    {
        var properties = typeof(T).GetProperties();

        var result = Activator.CreateInstance<T>();

        foreach (var property in properties)
        {
            var columnAttribute = property.GetCustomAttribute(typeof(ColumnAttribute), true) as ColumnAttribute;

            var columnName = string.Empty;

            if (columnAttribute != null)
                columnName = columnAttribute.Name;
            else
                columnName = property.Name;

            if (!reader.HasColumn(columnName)) 
                continue;

            var value = reader[columnName];

            if (value != DBNull.Value)
            {
                property.SetValue(result, value);
            }
        }

        return result;
    }

    public static string GenerateGetAllQuery(string filter, string tableName)
    {
        var builder = new StringBuilder($@"select * from {tableName} ");

        if (!string.IsNullOrWhiteSpace(filter))
            builder.Append(@$"where {filter}");

        return builder.ToString();
    }
}
