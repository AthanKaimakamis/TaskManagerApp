INSERT INTO TaskStatuses ([Name]) VALUES
('Open'),('In Progress'),('Closed'),('On Hold');
GO

INSERT INTO TaskTypes ([Name]) VALUES
('Bug'),('Feature'),('Improvement'),('Task');
GO

INSERT INTO CommentTypes ([Name]) VALUES
('Note'),('Reminder'),('Discussion'),('Feedback'),('Other');

INSERT INTO [Users] (DisplayName) VALUES
('Alice'),('Bob'),('Charlie'),('Diana');