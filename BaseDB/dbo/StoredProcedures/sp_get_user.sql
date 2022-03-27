CREATE PROCEDURE [dbo].[sp_get_user]
	@Id int
AS
begin
	select Id, FirstName, LastName
	from dbo.[User]
	where Id = @Id;
end
