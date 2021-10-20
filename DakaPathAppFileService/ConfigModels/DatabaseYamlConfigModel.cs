using SharpYaml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DakaPathAppFileService.ConfigModels
{
    public class DatabaseYamlConfigModel
    {
        [YamlMember("SQLite")]
        public SQLiteYamlConfigModel SQLite { get; set; }
    }
}
