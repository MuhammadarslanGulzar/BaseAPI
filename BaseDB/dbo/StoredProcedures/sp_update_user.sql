CREATE PROCEDURE [dbo].[sp_update_user]
	@Id int,
	@FirstName nvarchar(50),
	@LastName nvarchar(50)
AS
begin
	update dbo.[User]
	set FirstName = @FirstName, LastName = @LastName
	where Id = @Id;
end
