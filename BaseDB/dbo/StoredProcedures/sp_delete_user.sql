CREATE PROCEDURE [dbo].[sp_delete_user]
	@Id int
AS
begin
	delete
	from dbo.[User]
	where Id = @Id;
end
