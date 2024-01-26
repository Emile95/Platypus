using PlatypusLogging;
using System.Runtime.Serialization;

namespace PlatypusApplicationFramework.Configuration.Application
{
    [DataContract]
    public class PlatypusApplicationLoggerConfiguration
    {
        [DataMember]
        [ParameterEditor(
            Name = "FileLoggers",
            DefaultValue = null
        )]
        public List<FileLoggerConfiguration> FileLoggers { get; set; }

        [DataContract]
        public class FileLoggerConfiguration
        {
            [DataMember]
            [ParameterEditor(
                Name = "DirectoryPath",
                DefaultValue = null
            )]
            public string DirectoryPath { get; set; }

            [DataMember]
            [ParameterEditor(
                Name = "FileName",
                DefaultValue = null
            )]
            public string FileName { get; set; }

            [DataMember]
            [ParameterEditor(
                Name = "Format",
                DefaultValue = "{dateTime} - {loggingLevel} : {message}"
            )]
            public string Format { get; set; }

            [DataMember]
            [ParameterEditor(
                Name = "MinimumLoggingLevel",
                DefaultValue = LoggingLevel.Info
            )]
            public LoggingLevel MinimumLoggingLevel { get; set; }

            [DataMember]
            [ParameterEditor(
                Name = "MaximumLoggingLevel",
                DefaultValue = LoggingLevel.Fatal
            )]
            public LoggingLevel MaximumLoggingLevel { get; set; }

            [DataMember]
            [ParameterEditor(
                Name = "FileRotation",
                DefaultValue = null
            )]
            public FileRotationConfiguration FileRotation { get; set; }

            [DataContract]
            public class FileRotationConfiguration
            {
                [DataMember]
                [ParameterEditor(
                    Name = "MaxSize",
                    DefaultValue = 2000
                )]
                public int MaxSize { get; set; }
            }
        }
    }

    
}
