IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Claims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [LecturerName] nvarchar(200) NOT NULL,
    [HoursWorked] decimal(18,2) NOT NULL,
    [HourlyRate] decimal(18,2) NOT NULL,
    [TotalAmount] AS [HoursWorked] * [HourlyRate],
    [AdditionalNotes] nvarchar(500) NOT NULL,
    [Status] int NOT NULL,
    [SubmissionDate] datetime2 NOT NULL,
    [RejectionNotes] nvarchar(1000) NOT NULL,
    [RejectedBy] nvarchar(200) NOT NULL,
    [VerifiedBy] nvarchar(200) NOT NULL,
    [ApprovedBy] nvarchar(200) NOT NULL,
    [VerificationDate] datetime2 NULL,
    [ApprovalDate] datetime2 NULL,
    CONSTRAINT [PK_Claims] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Users] (
    [Id] nvarchar(450) NOT NULL,
    [FirstName] nvarchar(100) NOT NULL,
    [LastName] nvarchar(100) NOT NULL,
    [Email] nvarchar(255) NOT NULL,
    [Role] nvarchar(50) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [PasswordHash] nvarchar(255) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [SupportingDocuments] (
    [Id] int NOT NULL IDENTITY,
    [ClaimId] int NOT NULL,
    [FileName] nvarchar(255) NOT NULL,
    [FilePath] nvarchar(500) NOT NULL,
    [ContentType] nvarchar(100) NOT NULL,
    [FileSize] bigint NOT NULL,
    [UploadDate] datetime2 NOT NULL,
    CONSTRAINT [PK_SupportingDocuments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SupportingDocuments_Claims_ClaimId] FOREIGN KEY ([ClaimId]) REFERENCES [Claims] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_SupportingDocuments_ClaimId] ON [SupportingDocuments] ([ClaimId]);
GO

CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251119134538_InitialCreate', N'8.0.0');
GO

COMMIT;
GO

