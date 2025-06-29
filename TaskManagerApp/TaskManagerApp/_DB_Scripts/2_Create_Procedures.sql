-- Tasks
CREATE OR ALTER PROCEDURE CreateTask
    @RequiredByDate DATETIME = NULL,
    @Description    NVARCHAR(MAX),
    @StatusId       SMALLINT,
    @TypeId         SMALLINT = NULL,
    @UserId         INT,
    @NewId          INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Tasks (RequiredByDate, [Description], StatusId, TypeId, UserId)
      VALUES (@RequiredByDate, @Description, @StatusId, @TypeId, @UserId);
    SET @NewId = CAST(SCOPE_IDENTITY() AS INT);
END;
GO

CREATE OR ALTER PROCEDURE UpdateTask
    @Id             INT,
    @RequiredByDate DATETIME = NULL,
    @Description    NVARCHAR(MAX),
    @StatusId       SMALLINT,
    @TypeId         SMALLINT,
    @UserId         INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Tasks
    SET RequiredByDate = @RequiredByDate,
        [Description] = @Description,
        StatusId = @StatusId,
        TypeId = @TypeId,
        UserId = @UserId
    WHERE Id = @Id;
END;
GO

CREATE OR ALTER PROCEDURE DeleteTask
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Tasks
    WHERE Id = @Id;
END;
GO

-- Comments
CREATE OR ALTER PROCEDURE SetNextActionDate
    @TaskId INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @NextActionDate DATETIME;

    SELECT @NextActionDate = MIN(ReminderDate)
    FROM Comments
    WHERE TaskId = @TaskId
      AND ReminderDate IS NOT NULL;

    IF @NextActionDate IS NOT NULL
      AND EXISTS (SELECT 1 FROM Tasks WHERE Id = @TaskId)
    BEGIN
        UPDATE Tasks
        SET NextActionDate = @NextActionDate
        WHERE Id = @TaskId;
    END
END;
GO

CREATE OR ALTER PROCEDURE CreateComment
    @TaskId       INT,
    @CommentText  NVARCHAR(MAX),
    @TypeId       SMALLINT,
    @ReminderDate DATETIME = NULL,
    @NewId        INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
      INSERT INTO Comments (TaskId, Comment, TypeId, ReminderDate)
        VALUES (@TaskId, @CommentText, @TypeId, @ReminderDate);
      
      EXEC SetNextActionDate @TaskId;
    COMMIT TRANSACTION;
    SET @NewId = CAST(SCOPE_IDENTITY() AS INT);
END;
GO

CREATE OR ALTER PROCEDURE UpdateComment
    @Id           INT,
    @CommentText  NVARCHAR(MAX),
    @TypeId       SMALLINT,
    @ReminderDate DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
      UPDATE Comments
      SET Comment = @CommentText,
          TypeId = @TypeId,
          ReminderDate = @ReminderDate
      WHERE Id = @Id;
      DECLARE @TaskId INT = (SELECT TaskId FROM Comments WHERE Id = @Id);
      EXEC SetNextActionDate @TaskId;
    COMMIT TRANSACTION;
END;
GO

CREATE OR ALTER PROCEDURE DeleteComment
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
      DECLARE @TaskId INT = (SELECT TaskId FROM Comments WHERE Id = @Id);
      DELETE FROM Comments
      WHERE Id = @Id;
      EXEC SetNextActionDate @TaskId;
    COMMIT TRANSACTION;
END;
GO

-- Users
CREATE OR ALTER PROCEDURE CreateUser
    @DisplayName NVARCHAR(100),
    @NewId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Users (DisplayName) VALUES (@DisplayName);
    SET @NewId = CAST(SCOPE_IDENTITY() AS INT);
END;
GO

CREATE OR ALTER PROCEDURE UpdateUser
    @Id INT,
    @DisplayName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Users
    SET DisplayName = @DisplayName
    WHERE Id = @Id;
END;
GO

CREATE OR ALTER PROCEDURE DeleteUser
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Users
    WHERE Id = @Id;
END;