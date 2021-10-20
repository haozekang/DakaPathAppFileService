using SharpYaml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DakaPathAppFileService.ConfigModels
{
    public class MinIOYamlConfigModel
    {

        [YamlMember("EndPoint")]
        public string EndPoint { get; set; }

        [YamlMember("APIEndPoint")]
        public string APIEndPoint { get; set; }

        [YamlMember("UseSsl")]
        public bool? UseSsl { get; set; }

        [YamlMember("FileBucket")]
        public string FileBucket { get; set; }

        [YamlMember("ImageBucket")]
        public string ImageBucket { get; set; }

        [YamlMember("ReportBucket")]
        public string ReportBucket { get; set; }

        [YamlMember("AccessKey")]
        public string AccessKey { get; set; }

        [YamlMember("SecertKey")]
        public string SecertKey { get; set; }
    }
}
