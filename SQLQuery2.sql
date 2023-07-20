create database StudentDB;

USE [StudentDB]
GO

/****** Object:  Table [dbo].[Student]    Script Date: 18-07-2023 17:55:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

select * from Student;

select * from Student where StudentName like 'n%' and IsDeleted = 0;

CREATE TABLE [dbo].[Student](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StudentName] [varchar](40) NULL,
	[StudentRollNo] [int] NULL,
	[StudentClass] [int] NULL,
	[StudentDOB] [varchar](40) NULL,
	[StudentEmail] [varchar](40) NULL,
	[ParentNo] [varchar](40) NULL,
	[CreatedOn] [datetime] NULL,
	[UpdatedOn] [datetime] NULL,
	[CreatedBy] [varchar](40) NULL,
	[UpdatedBy] [varchar](40) NULL,
	[IsDeleted] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Student] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO





USE [StudentDB]
GO

/****** Object:  StoredProcedure [dbo].[StudentSP]    Script Date: 18-07-2023 17:07:57 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[StudentSP] 
	@Query int = 0,
	@StudentName varchar(40) = ' ',
	@StudentRollNo int = 0,
	@StudentClass int = 0,
	@StudentDOB varchar(40) = ' ',
	@StudentEmail varchar(40) = ' ',
	@ParentNo varchar(40) = ' ',
	@CreatedOn DateTime = null,
	@UpdatedOn DateTime = null,
	@CreatedBy varchar(40) = ' ',
	@UpdatedBy varchar(40) = ' ' 

AS  
BEGIN  

IF (@Query = 1) -- Insert
BEGIN  

	insert into Student(StudentName, StudentRollNo, StudentClass, StudentDOB, StudentEmail, ParentNo, CreatedBy, CreatedOn, UpdatedOn, IsDeleted)
	values(@StudentName, @StudentRollNo, @StudentClass, @StudentDOB, @StudentEmail, @ParentNo, @CreatedBy, @CreatedOn, null, 0)
END

IF (@Query = 2) -- Update
BEGIN
	update Student set StudentName = @StudentName, StudentClass = @StudentClass, StudentDOB = @StudentDOB, StudentEmail = @StudentEmail, ParentNo = @ParentNo, UpdatedBy =@UpdatedBy, UpdatedOn = @UpdatedOn,IsDeleted = 0 where StudentRollNo = @StudentRollNo;
END


IF (@Query = 3) -- Delete
BEGIN  
	update Student set IsDeleted = 1 where studentRollNo = @StudentRollNo;
END

IF (@Query = 4) -- View All
BEGIN  

	select * from Student where IsDeleted = 0;
END

IF (@Query = 5) -- View One
BEGIN  

	select * from Student where StudentRollNo = @StudentRollNo;
END

END



GO

create procedure UpdateStudent
	@Query int = 0,
	@StudentName varchar(40) = ' ',
	@StudentRollNo int = 0,
	@StudentClass int = 0,
	@StudentDOB varchar(40) = ' ',
	@StudentEmail varchar(40) = ' ',
	@ParentNo varchar(40) = ' ',
	@CreatedOn DateTime = null,
	@UpdatedOn DateTime = null,
	@CreatedBy varchar(40) = ' ',
	@UpdatedBy varchar(40) = ' '
as
begin
	update Student set StudentName = @StudentName, StudentClass = @StudentClass, StudentDOB = @StudentDOB, StudentEmail = @StudentEmail, ParentNo = @ParentNo, UpdatedBy =@UpdatedBy, UpdatedOn = @UpdatedOn,IsDeleted = 0 where StudentRollNo = @StudentRollNo;
end

drop procedure UpdateStudent

select * from Student;
