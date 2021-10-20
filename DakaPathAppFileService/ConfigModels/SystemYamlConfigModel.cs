using SharpYaml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DakaPathAppFileService.ConfigModels
{
    public class SystemYamlConfigModel
    {
        [YamlMember("Title")]
        public string Title { get; set; }

        [YamlMember("Company")]
        public string Company { get; set; }

        [YamlMember("TempPath")]
        public string TempPath { get; set; }

        [YamlMember("TempCleanJobCron")]
        public string TempCleanJobCron { get; set; }
    }
}
