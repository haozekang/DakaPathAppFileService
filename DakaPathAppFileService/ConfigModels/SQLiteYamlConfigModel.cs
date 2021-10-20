using SharpYaml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace DakaPathAppFileService.ConfigModels
{
    public class SQLiteYamlConfigModel
    {
        [YamlMember("Path")]
        public string Path { get; set; }

        public string GetConnectionString()
        {
            SqliteConnectionStringBuilder builder = new SqliteConnectionStringBuilder();
            builder.DataSource = $"{Path}";
            builder.Mode = SqliteOpenMode.ReadWriteCreate;
            return builder.ToString();
        }
    }
}
