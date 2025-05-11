--Archive tables
CREATE TABLE [dbo].[Archive_Air_Quality] (
    [ID]                       INT          IDENTITY(1,1) NOT NULL,
    [Date]                     DATE         NOT NULL,
    [Time]                     TIME (7)     NOT NULL,
    [Nitrogen_dioxide]         FLOAT (53)   NULL,
    [Sulphur_dioxide]          FLOAT (53)   NULL,
    [PM2_5_particulate_matter] FLOAT (53)   NULL,
    [PM10_particulate_matter]  FLOAT (53)   NULL,
    [LocationId]               INT          NOT NULL,
    CONSTRAINT PK_Archive_Air_Quality PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT UQ_Archive_Air_Quality_DateTimeLocation UNIQUE ([Date], [Time], [LocationId]),
    CONSTRAINT FK_Archive_Air_Quality_Locations FOREIGN KEY ([LocationId])
        REFERENCES [dbo].Locations
);


CREATE TABLE [dbo].[Archive_Water_Quality] (
    [ID]                         INT          IDENTITY (1, 1) NOT NULL,
    [Date]                       DATE         NOT NULL,
    [Time]                       TIME (7)     NOT NULL,
    [Nitrate_mg_l_1]             FLOAT (53)   NULL,
    [Nitrite_less_thank_mg_l_1]  FLOAT (53)   NULL,
    [Phosphate_mg_l_1]           FLOAT (53)   NULL,
    [EC_cfu_100ml]               FLOAT (53)   NULL,
    [LocationId]                 INT          NOT NULL,
    CONSTRAINT PK_ArchiveWaterQuality PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT FK_ArchiveWaterQuality_Locations FOREIGN KEY ([LocationId])
        REFERENCES [dbo].Locations
);
CREATE TABLE [dbo].[archive_weather_data] (
    [ID]                   INT          IDENTITY(1,1) NOT NULL,
    [Date_Time]            DATETIME     NOT NULL,
    [temperature_2m]       FLOAT (53)   NOT NULL,
    [relative_humidity_2m] FLOAT (53)   NOT NULL,
    [wind_speed_10m]       FLOAT (53)   NOT NULL,
    [wind_direction_10m]   FLOAT (53)   NOT NULL,
    [LocationId]           INT          NOT NULL,
    CONSTRAINT PK_archive_weather_data PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT UQ_archive_weather_data_DateTimeLocation UNIQUE ([Date_Time], [LocationId]),
    CONSTRAINT FK_archive_weather_data_Locations FOREIGN KEY ([LocationId])
        REFERENCES [dbo].Locations
);

-- data tables
CREATE TABLE [dbo].[weather_data] (
    [ID]                   INT          IDENTITY(1,1) NOT NULL,
    [Date_Time]            DATETIME     NOT NULL,
    [temperature_2m]       FLOAT (53)   NOT NULL,
    [relative_humidity_2m] FLOAT (53)   NOT NULL,
    [wind_speed_10m]       FLOAT (53)   NOT NULL,
    [wind_direction_10m]   FLOAT (53)   NOT NULL,
    [LocationId]           INT          NOT NULL,
    CONSTRAINT PK_weather_data PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT UQ_weather_data_DateTimeLocation UNIQUE ([Date_Time], [LocationId]),
    CONSTRAINT FK_weather_data_Locations FOREIGN KEY ([LocationId])
        REFERENCES [dbo].Locations
);

CREATE TABLE [dbo].[Water_Quality] (
    [ID]                         INT          IDENTITY(1,1) NOT NULL,
    [Date]                       DATE         NOT NULL,
    [Time]                       TIME (7)     NOT NULL,
    [Nitrate_mg_l_1]             FLOAT (53)   NULL,
    [Nitrite_less_thank_mg_l_1]  FLOAT (53)   NULL,
    [Phosphate_mg_l_1]           FLOAT (53)   NULL,
    [EC_cfu_100ml]               FLOAT (53)   NULL,
    [LocationId]                 INT          NOT NULL,
    CONSTRAINT PK_Water_Quality PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT UQ_Water_Quality_DateTimeLocation UNIQUE ([Date], [Time], [LocationId]),
    CONSTRAINT FK_Water_Quality_Locations FOREIGN KEY ([LocationId])
        REFERENCES [dbo].Locations
);


CREATE TABLE [dbo].[Air_Quality] (
    [ID]                       INT          IDENTITY(1,1) NOT NULL,
    [Date]                     DATE         NOT NULL,
    [Time]                     TIME (7)     NOT NULL,
    [Nitrogen_dioxide]         FLOAT (53)   NULL,
    [Sulphur_dioxide]          FLOAT (53)   NULL,
    [PM2_5_particulate_matter] FLOAT (53)   NULL,
    [PM10_particulate_matter]  FLOAT (53)   NULL,
    [LocationId]               INT          NOT NULL,
    CONSTRAINT PK_Air_Quality PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT UQ_Air_Quality_DateTimeLocation UNIQUE ([Date], [Time], [LocationId]),
    CONSTRAINT FK_Air_Quality_Locations FOREIGN KEY ([LocationId])
        REFERENCES [dbo].Locations
);


CREATE TABLE [dbo].[ErrorTable] (
    [ErrorID]       INT             IDENTITY (1, 1) NOT NULL,
    [ErrorDateTime] DATETIME        DEFAULT (getdate()) NULL,
    [ErrorMessage]  NVARCHAR (4000) NULL,
    PRIMARY KEY CLUSTERED ([ErrorID] ASC)
);
CREATE TABLE [dbo].[LogTable] (
    [LogID]       INT             IDENTITY (1, 1) NOT NULL,
    [LogDateTime] DATETIME        DEFAULT (getdate()) NULL,
    [LogMessage]  NVARCHAR (4000) NULL,
    PRIMARY KEY CLUSTERED ([LogID] ASC)
);
