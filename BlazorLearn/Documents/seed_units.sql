USE [onlinestore];
GO

INSERT INTO [dbo].[Units] (Id, Name, Symbol, IsActive, CreatedAt) VALUES
(NEWID(), N'عدد', N'عدد', 1, GETDATE()),
(NEWID(), N'بسته', N'بسته', 1, GETDATE()),
(NEWID(), N'جعبه', N'جعبه', 1, GETDATE()),
(NEWID(), N'کارتن', N'کارتن', 1, GETDATE()),
(NEWID(), N'کیلوگرم', N'kg', 1, GETDATE()),
(NEWID(), N'گرم', N'g', 1, GETDATE()),
(NEWID(), N'تن', N't', 1, GETDATE()),
(NEWID(), N'لیتر', N'L', 1, GETDATE()),
(NEWID(), N'میلی‌لیتر', N'mL', 1, GETDATE()),
(NEWID(), N'متر', N'm', 1, GETDATE()),
(NEWID(), N'سانتی‌متر', N'cm', 1, GETDATE()),
(NEWID(), N'میلی‌متر', N'mm', 1, GETDATE()),
(NEWID(), N'بسته ۱۲تایی', N'×12', 1, GETDATE()),
(NEWID(), N'بسته ۶تایی', N'×6', 1, GETDATE()),
(NEWID(), N'جفت', N'جفت', 1, GETDATE());
GO
