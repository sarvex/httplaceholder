﻿namespace HttPlaceholder.Configuration
{
    public class StorageSettingsModel
    {
        public string InputFile { get; set; }

        public int OldRequestsQueueLength { get; set; } = 40;

        public bool EnableRequestLogging { get; set; }

        public string FileStorageLocation { get; set; }
    }
}
