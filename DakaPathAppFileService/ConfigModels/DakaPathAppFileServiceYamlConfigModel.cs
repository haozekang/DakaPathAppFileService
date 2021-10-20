using SharpYaml.Serialization;

namespace DakaPathAppFileService.ConfigModels
{
    public class DakaPathAppFileServiceYamlConfigModel
    {
        [YamlMember("Databases")]
        public DatabaseYamlConfigModel Databases { get; set; }

        [YamlMember("MinIO")]
        public MinIOYamlConfigModel MinIO { get; set; }

        [YamlMember("System")]
        public SystemYamlConfigModel System { get; set; }
    }
}
