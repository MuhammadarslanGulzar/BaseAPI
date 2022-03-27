CREATE PROCEDURE [dbo].[sp_insert_user]
	@FirstName nvarchar(50),
	@LastName nvarchar(50)
AS
begin
	insert into dbo.[User] (FirstName, LastName)
	values (@FirstName, @LastName);
end
