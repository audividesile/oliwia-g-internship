CREATE TABLE public.Token (
    Id UUID PRIMARY KEY NOT NULL,
    Token UUID NOT NULL,
    UserId UUID NOT NULL,
    AdminId UUID NOT NULL,
    Name VARCHAR NOT NULL
);

CREATE TABLE public.ClientConfig (
    Id UUID PRIMARY KEY NOT NULL,
    ClientId UUID NOT NULL,
    TriggerLevel VARCHAR NOT NULL
);

CREATE TABLE public.AdminConfig (
    Id UUID PRIMARY KEY NOT NULL,
    PhoneNumber VARCHAR NOT NULL,
    Email VARCHAR NOT NULL,
    DefaultNotificationType VARCHAR NOT NULL
);

CREATE TABLE public.Log (
    Id UUID PRIMARY KEY NOT NULL,
    TokenId UUID NOT NULL,
    CpuUsage REAL NOT NULL,
    RamUsage REAL NOT NULL,
    Type VARCHAR NOT NULL,
    Timestamp TIMESTAMP NOT NULL
);